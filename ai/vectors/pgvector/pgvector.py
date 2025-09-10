from models import Document, Segment
import hashlib
from extensions import TextEmbeddingModel
from utils import string_utils
from sqlalchemy import create_engine, text
from dto import SearchScope
from ..vector_base import VectorBase
from ..vector_type import VectorType
from ..result import Result
from .pgvector_config import PGVectorConfig
from configs import app_configs

ATTR_ID = "id"
ATTR_SITE_ID = "site_id"
ATTR_CHANNEL_ID = "channel_id"
ATTR_CONTENT_ID = "content_id"
ATTR_DOCUMENT_ID = "document_id"
ATTR_TEXT = "text"
ATTR_DOC_NAME = "doc_name"
ATTR_EXT_NAME = "ext_name"
ATTR_EMBEDDING = "embedding"
ATTR_SEARCHING = "searching"


class PGVector(VectorBase):
    def __init__(self, text_embedding: TextEmbeddingModel, config: PGVectorConfig):
        super().__init__(text_embedding)
        self._config = config
        if text_embedding.isReady:
            unique_id = hashlib.md5(
                f"{text_embedding.providerId}:{text_embedding.modelId}".encode()
            ).hexdigest()[:8]
            self._table_name = (
                f"ssrag_embedding_{app_configs.TENANT_ID}_{unique_id}"
                if app_configs.TENANT_ID
                else f"ssrag_embedding_{unique_id}"
            )
            self._table_name_idx_embedding = f"{self._table_name}_idx_embedding"
            self._table_name_idx_searching = f"{self._table_name}_idx_searching"
            database_url = f"postgresql://{config.user}:{config.password}@{config.host}:{config.port}/{config.database}"
            if config.schema:
                self._engine = create_engine(
                    str(database_url),
                    connect_args={"options": f"-csearch_path={config.schema}"},
                    pool_recycle=3600,  # 每小时主动回收连接（需小于wait_timeout）
                    pool_pre_ping=True,  # 每次取连接前验证有效性
                )
            else:
                self._engine = create_engine(
                    str(database_url),
                    pool_recycle=3600,  # 每小时主动回收连接（需小于wait_timeout）
                    pool_pre_ping=True,  # 每次取连接前验证有效性
                )

    def get_type(self) -> str:
        return VectorType.PGVECTOR

    def add(self, document: Document, segments: list[Segment]) -> None:
        inputs = [segment.text for segment in segments if segment.text is not None]
        if not inputs:
            return
        embeddings = self._text_embedding.embedding_inputs(inputs)
        values = []
        for i, segment in enumerate(segments):
            if segment.text is None:
                continue
            embedding = embeddings[i]
            searching = string_utils.jieba_cut_to_str(segment.text)
            values.append(
                {
                    ATTR_ID: segment.id,
                    ATTR_SITE_ID: segment.siteId,
                    ATTR_CHANNEL_ID: segment.channelId,
                    ATTR_CONTENT_ID: segment.contentId,
                    ATTR_DOCUMENT_ID: segment.documentId,
                    ATTR_TEXT: segment.text,
                    ATTR_DOC_NAME: (
                        document.fileName if document.fileName else document.title
                    ),
                    ATTR_EXT_NAME: document.extName,
                    ATTR_EMBEDDING: embedding,
                    ATTR_SEARCHING: searching,
                }
            )
        with self._engine.connect() as conn:
            conn.execute(
                text(
                    f"""
INSERT INTO {self._table_name} (
    {ATTR_ID}, {ATTR_SITE_ID}, {ATTR_CHANNEL_ID}, {ATTR_CONTENT_ID}, 
    {ATTR_DOCUMENT_ID}, {ATTR_TEXT}, {ATTR_DOC_NAME}, {ATTR_EXT_NAME}, {ATTR_EMBEDDING}, {ATTR_SEARCHING}
) VALUES (
    :{ATTR_ID}, :{ATTR_SITE_ID}, :{ATTR_CHANNEL_ID}, :{ATTR_CONTENT_ID}, 
    :{ATTR_DOCUMENT_ID}, :{ATTR_TEXT}, :{ATTR_DOC_NAME}, :{ATTR_EXT_NAME}, :{ATTR_EMBEDDING}, :{ATTR_SEARCHING}
)
                    """
                ),
                values,
            )
            conn.commit()

    def update(self, uuid: str, value: str):
        embedding = self._text_embedding.embedding_input(value)
        searching = string_utils.jieba_cut_to_str(value)
        with self._engine.connect() as conn:
            conn.execute(
                text(
                    f"""
UPDATE {self._table_name} SET 
    {ATTR_TEXT} = :{ATTR_TEXT}, 
    {ATTR_EMBEDDING} = :{ATTR_EMBEDDING}, 
    {ATTR_SEARCHING} = to_tsvector(:{ATTR_SEARCHING}) 
WHERE {ATTR_ID} = :{ATTR_ID}
                    """
                ),
                {
                    ATTR_TEXT: value,
                    ATTR_EMBEDDING: embedding,
                    ATTR_SEARCHING: searching,
                    ATTR_ID: uuid,
                },
            )
            conn.commit()

    def exists(self, id: str) -> bool:
        if not id:
            return False
        with self._engine.connect() as conn:
            result = conn.execute(
                text(f"SELECT {ATTR_ID} FROM {self._table_name} WHERE {ATTR_ID} = :id"),
                {"id": id},
            ).fetchone()
            return result is not None

    def delete_by_ids(self, ids: list[str]) -> None:
        if not ids:
            return
        with self._engine.connect() as conn:
            conn.execute(
                text(f"DELETE FROM {self._table_name} WHERE {ATTR_ID} IN :ids"),
                {"ids": tuple(ids)},
            )
            conn.commit()

    def delete_by_site_id(self, site_id: int) -> None:
        with self._engine.connect() as conn:
            conn.execute(
                text(f"DELETE FROM {self._table_name} WHERE {ATTR_SITE_ID} = :site_id"),
                {"site_id": site_id},
            )
            conn.commit()

    def delete_by_channel_id(self, site_id: int, channel_id: int) -> None:
        with self._engine.connect() as conn:
            conn.execute(
                text(
                    f"DELETE FROM {self._table_name} WHERE {ATTR_SITE_ID} = :site_id AND {ATTR_CHANNEL_ID} = :channel_id"
                ),
                {"site_id": site_id, "channel_id": channel_id},
            )
            conn.commit()

    def delete_by_content_id(
        self, site_id: int, channel_id: int, content_id: int
    ) -> None:
        with self._engine.connect() as conn:
            conn.execute(
                text(
                    f"DELETE FROM {self._table_name} WHERE {ATTR_SITE_ID} = :site_id AND {ATTR_CHANNEL_ID} = :channel_id AND {ATTR_CONTENT_ID} = :content_id"
                ),
                {
                    "site_id": site_id,
                    "channel_id": channel_id,
                    "content_id": content_id,
                },
            )
            conn.commit()

    def delete_by_document_id(self, document_id: int) -> None:
        with self._engine.connect() as conn:
            conn.execute(
                text(
                    f"DELETE FROM {self._table_name} WHERE {ATTR_DOCUMENT_ID} = :document_id"
                ),
                {"document_id": document_id},
            )
            conn.commit()

    def search_by_semantic(
        self,
        searchScope: SearchScope,
        query: str,
        maxCount: int = 10,
        minScore: float = 0.3,
    ) -> list[Result]:
        if not query:
            return []
        embedding = self._text_embedding.embedding_input(query)
        with self._engine.connect() as conn:
            where_clause = self._get_where_clause(searchScope)
            if where_clause:
                where_clause = f"WHERE {where_clause}"

            results = conn.execute(
                text(
                    f"""
WITH similarities AS (
    SELECT 
        {ATTR_ID},
        {ATTR_SITE_ID},
        {ATTR_CHANNEL_ID},
        {ATTR_CONTENT_ID},
        {ATTR_DOCUMENT_ID},
        {ATTR_TEXT},
        {ATTR_DOC_NAME},
        {ATTR_EXT_NAME},
        1 - (embedding <=> '{embedding}') AS similarity 
    FROM {self._table_name} {where_clause}
)
SELECT * FROM similarities
WHERE similarity > :min_score
ORDER BY similarity DESC
LIMIT :max_count;
                    """
                ),
                {"max_count": maxCount, "min_score": minScore},
            ).fetchall()

        search_results: list[Result] = []
        for result in results:
            search_results.append(
                Result(
                    id=str(result[0]),
                    siteId=result[1],
                    channelId=result[2],
                    contentId=result[3],
                    documentId=result[4],
                    text=result[5],
                    docName=result[6],
                    extName=result[7],
                    score=result[8],
                )
            )
        return search_results

    def search_by_full_text(
        self,
        searchScope: SearchScope,
        query: str,
        maxCount: int = 10,
        minScore: float = 0.3,
    ) -> list[Result]:
        if not query:
            return []
        cut_query = string_utils.jieba_cut_to_str(query, " | ")
        with self._engine.connect() as conn:
            where_clause = self._get_where_clause(searchScope)
            if where_clause:
                where_clause = f"AND {where_clause}"

            results = conn.execute(
                text(
                    f"""
WITH ranked_documents AS (
    SELECT 
        {ATTR_ID},
        {ATTR_SITE_ID},
        {ATTR_CHANNEL_ID},
        {ATTR_CONTENT_ID},
        {ATTR_DOCUMENT_ID},
        {ATTR_TEXT},
        {ATTR_DOC_NAME},
        {ATTR_EXT_NAME},
        ts_rank('{{1, 1, 1, 1}}'::float4[], searching, to_tsquery('{cut_query}')) AS rank 
    FROM {self._table_name} 
    WHERE 
        searching @@ to_tsquery('{cut_query}') {where_clause}
)
SELECT * 
FROM ranked_documents 
WHERE rank > :min_score 
ORDER BY rank DESC 
LIMIT :max_count;
                    """
                ),
                {"max_count": maxCount, "min_score": minScore},
            ).fetchall()

            search_results: list[Result] = []
            for result in results:
                search_results.append(
                    Result(
                        id=str(result[0]),
                        siteId=result[1],
                        channelId=result[2],
                        contentId=result[3],
                        documentId=result[4],
                        text=result[5],
                        docName=result[6],
                        extName=result[7],
                        score=result[8],
                    )
                )
            return search_results

    def search_by_hybrid(
        self,
        searchScope: SearchScope,
        query: str,
        maxCount: int = 10,
        minScore: float = 0.3,
    ) -> list[Result]:
        if not query:
            return []
        semantic_results = self.search_by_semantic(
            searchScope, query, maxCount, minScore
        )
        full_text_results = self.search_by_full_text(
            searchScope, query, maxCount, minScore
        )

        # 使用Reciprocal Rank Fusion (RRF)进行重排
        results = []
        semantic_rank_map = {}
        full_text_rank_map = {}

        # 构建语义搜索的排名映射
        for i, segment in enumerate(semantic_results):
            semantic_rank_map[segment.id] = i + 1

        # 构建全文搜索的排名映射
        for i, segment in enumerate(full_text_results):
            full_text_rank_map[segment.id] = i + 1

        # 合并所有唯一的segment
        all_results = {}
        for segment in semantic_results:
            all_results[segment.id] = segment
        for segment in full_text_results:
            all_results[segment.id] = segment

        # 计算RRF分数 (k=60是常用的参数)
        k = 60
        result_scores = []
        for segment_id, segment in all_results.items():
            semantic_rank = semantic_rank_map.get(segment_id, float("inf"))
            full_text_rank = full_text_rank_map.get(segment_id, float("inf"))

            # RRF公式: 1/(k + rank)
            semantic_rrf = 1 / (k + semantic_rank)
            full_text_rrf = 1 / (k + full_text_rank)

            # 总RRF分数
            total_rrf = semantic_rrf + full_text_rrf

            result_scores.append((segment, total_rrf))

        # 按RRF分数降序排序
        result_scores.sort(key=lambda x: x[1], reverse=True)

        # 取前maxCount个结果
        results = [result for result, _ in result_scores[:maxCount]]

        return results

    def delete_all(self) -> None:
        with self._engine.connect() as conn:
            conn.execute(text(f"DELETE FROM {self._table_name}"))
            conn.commit()

    def create_collection(self):
        with self._engine.connect() as conn:
            if self._config.schema:
                conn.execute(
                    text(
                        f"CREATE EXTENSION IF NOT EXISTS vector SCHEMA {self._config.schema}"
                    )
                )
            else:
                conn.execute(text("CREATE EXTENSION IF NOT EXISTS vector"))

            conn.execute(
                text(
                    f"""
CREATE TABLE IF NOT EXISTS {self._table_name} (
  {ATTR_ID} UUID PRIMARY KEY, 
  {ATTR_SITE_ID} INTEGER NOT NULL, 
  {ATTR_CHANNEL_ID} INTEGER NOT NULL, 
  {ATTR_CONTENT_ID} INTEGER NOT NULL, 
  {ATTR_DOCUMENT_ID} INTEGER NOT NULL,
  {ATTR_TEXT} TEXT NOT NULL,
  {ATTR_DOC_NAME} VARCHAR(255),
  {ATTR_EXT_NAME} VARCHAR(255),
  {ATTR_EMBEDDING} vector({self._text_embedding.dimension}), 
  {ATTR_SEARCHING} TSVECTOR
) using heap;
                    """
                )
            )
            # PG hnsw index only support 2000 dimension or less
            # ref: https://github.com/pgvector/pgvector?tab=readme-ov-file#indexing
            if self._text_embedding.dimension <= 2000:
                conn.execute(
                    text(
                        f"""
CREATE INDEX IF NOT EXISTS {self._table_name_idx_embedding} 
ON {self._table_name} 
USING hnsw ({ATTR_EMBEDDING} vector_cosine_ops) 
WITH (m = 16, ef_construction = 64);
                        """
                    )
                )
            conn.execute(
                text(
                    f"""
CREATE INDEX IF NOT EXISTS {self._table_name_idx_searching} 
ON {self._table_name} 
USING gin ({ATTR_SEARCHING});
                    """
                )
            )

            conn.commit()

    def _get_where_clause(self, searchScope: SearchScope) -> str:
        conditions = []
        if searchScope.siteIds:
            conditions.append(
                f"{ATTR_SITE_ID} IN ({','.join(map(str, searchScope.siteIds))})"
            )
        if searchScope.channelIds:
            conditions.append(
                f"{ATTR_CHANNEL_ID} IN ({','.join(map(str, searchScope.channelIds))})"
            )
        if searchScope.contentIds:
            conditions.append(
                f"{ATTR_CONTENT_ID} IN ({','.join(map(str, searchScope.contentIds))})"
            )
        return " OR ".join(conditions)
