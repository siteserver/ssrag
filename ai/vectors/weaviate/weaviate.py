import weaviate
from weaviate.client import WeaviateClient
import weaviate.classes as wvc
import weaviate.classes.config as wvcc
from weaviate.classes.query import Filter
from extensions import TextEmbeddingModel
from models import Document, Segment
import hashlib
from dto import SearchScope
from ..vector_type import VectorType
from ..vector_base import VectorBase
from ..result import Result
from .weaviate_config import WeaviateConfig
from utils import string_utils
from weaviate.collections.classes.filters import _Filters

ATTR_SITE_ID = "site_id"
ATTR_CHANNEL_ID = "channel_id"
ATTR_CONTENT_ID = "content_id"
ATTR_DOCUMENT_ID = "document_id"
ATTR_TEXT = "text"
ATTR_DOC_NAME = "doc_name"
ATTR_EXT_NAME = "ext_name"


class WeaviateVector(VectorBase):
    def __init__(self, text_embedding: TextEmbeddingModel, config: WeaviateConfig):
        super().__init__(text_embedding)
        if text_embedding.isReady:
            unique_id = hashlib.md5(
                f"{text_embedding.providerId}:{text_embedding.modelId}".encode()
            ).hexdigest()[:8]
            self._collection_name = f"SSRAG_{unique_id}"
            self._config = config
            with self.connect() as client:
                if not client.is_ready():
                    raise ConnectionError("Weaviate database connection error")

    def connect(self):
        return weaviate.connect_to_local()

    def get_type(self) -> str:
        return VectorType.WEAVIATE

    def add(self, document: Document, segments: list[Segment]) -> None:
        inputs = [segment.text for segment in segments if segment.text is not None]
        if not inputs:
            return
        embeddings = self._text_embedding.embedding_inputs(inputs)
        objs = list()
        for i, segment in enumerate(segments):
            if segment.text is None:
                continue
            objs.append(
                wvc.data.DataObject(
                    properties={
                        ATTR_SITE_ID: segment.siteId,
                        ATTR_CHANNEL_ID: segment.channelId,
                        ATTR_CONTENT_ID: segment.contentId,
                        ATTR_DOCUMENT_ID: segment.documentId,
                        ATTR_TEXT: segment.text,
                        ATTR_DOC_NAME: (
                            document.fileName if document.fileName else document.title
                        ),
                        ATTR_EXT_NAME: document.extName,
                    },
                    uuid=segment.id,
                    vector=embeddings[i],
                )
            )

        with self.connect() as client:
            collection = client.collections.get(self._collection_name)
            collection.data.insert_many(objs)

    def update(self, id: str, text: str):
        if not id or not text:
            return
        embedding = self._text_embedding.embedding_input(text)

        with self.connect() as client:
            collection = client.collections.get(self._collection_name)
            collection.data.update(
                uuid=id,
                properties={
                    ATTR_TEXT: text,
                },
                vector=embedding,
            )

    def exists(self, id: str) -> bool:
        if not id:
            return False
        with self.connect() as client:
            collection = client.collections.get(self._collection_name)
            return collection.data.exists(id)

    def delete_by_ids(self, ids: list[str]) -> None:
        if not ids:
            return
        with self.connect() as client:
            collection = client.collections.get(self._collection_name)
            collection.data.delete_many(where=Filter.by_id().contains_any(ids))

    def delete_by_site_id(self, site_id: int) -> None:
        if not site_id:
            return
        with self.connect() as client:
            collection = client.collections.get(self._collection_name)
            collection.data.delete_many(where=Filter.by_property(ATTR_SITE_ID).equals(site_id))  # type: ignore

    def delete_by_channel_id(self, site_id: int, channel_id: int) -> None:
        if not site_id or not channel_id:
            return
        with self.connect() as client:
            collection = client.collections.get(self._collection_name)
            collection.data.delete_many(where=Filter.by_property(ATTR_SITE_ID).equals(site_id), Filter.by_property(ATTR_CHANNEL_ID).equals(channel_id))  # type: ignore

    def delete_by_content_id(self, site_id: int, channel_id: int, content_id: int) -> None:
        if not site_id or not channel_id or not content_id:
            return
        with self.connect() as client:
            collection = client.collections.get(self._collection_name)
            collection.data.delete_many(where=Filter.by_property(ATTR_SITE_ID).equals(site_id), Filter.by_property(ATTR_CHANNEL_ID).equals(channel_id), Filter.by_property(ATTR_CONTENT_ID).equals(content_id))  # type: ignore

    def delete_by_document_id(self, document_id: int) -> None:
        if not document_id:
            return
        with self.connect() as client:
            collection = client.collections.get(self._collection_name)
            collection.data.delete_many(where=Filter.by_property(ATTR_DOCUMENT_ID).equals(document_id))  # type: ignore

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
        with self.connect() as client:
            collection = client.collections.get(self._collection_name)
            response = collection.query.near_vector(
                near_vector=embedding,
                limit=maxCount,
                certainty=minScore,
                filters=self._get_filters(searchScope),
                return_metadata=wvc.query.MetadataQuery(certainty=True),
            )

        search_results: list[Result] = []
        for obj in response.objects:
            search_results.append(
                Result(
                    id=str(obj.uuid),
                    siteId=string_utils.to_int(obj.properties[ATTR_SITE_ID]),
                    channelId=string_utils.to_int(obj.properties[ATTR_CHANNEL_ID]),
                    contentId=string_utils.to_int(obj.properties[ATTR_CONTENT_ID]),
                    documentId=string_utils.to_int(obj.properties[ATTR_DOCUMENT_ID]),
                    text=str(obj.properties[ATTR_TEXT]),
                    docName=str(obj.properties[ATTR_DOC_NAME]),
                    extName=str(obj.properties[ATTR_EXT_NAME]),
                    score=obj.metadata.certainty or 0,
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
        with self.connect() as client:
            collection = client.collections.get(self._collection_name)
            response = collection.query.bm25(
                query=cut_query,
                filters=self._get_filters(searchScope),
                limit=maxCount,
                return_metadata=wvc.query.MetadataQuery(score=True),
            )
        search_results: list[Result] = []
        for obj in response.objects:
            search_results.append(
                Result(
                    id=str(obj.uuid),
                    siteId=string_utils.to_int(obj.properties[ATTR_SITE_ID]),
                    channelId=string_utils.to_int(obj.properties[ATTR_CHANNEL_ID]),
                    contentId=string_utils.to_int(obj.properties[ATTR_CONTENT_ID]),
                    documentId=string_utils.to_int(obj.properties[ATTR_DOCUMENT_ID]),
                    text=str(obj.properties[ATTR_TEXT]),
                    docName=str(obj.properties[ATTR_DOC_NAME]),
                    extName=str(obj.properties[ATTR_EXT_NAME]),
                    score=obj.metadata.score or 0,
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
        embedding = self._text_embedding.embedding_input(query)
        cut_query = string_utils.jieba_cut_to_str(query, " | ")
        with self.connect() as client:
            collection = client.collections.get(self._collection_name)
            response = collection.query.hybrid(
                query=cut_query,
                vector=embedding,
                filters=self._get_filters(searchScope),
                limit=maxCount,
                return_metadata=wvc.query.MetadataQuery(certainty=True, score=True),
            )
        search_results: list[Result] = []
        for obj in response.objects:
            search_results.append(
                Result(
                    id=str(obj.uuid),
                    siteId=string_utils.to_int(obj.properties[ATTR_SITE_ID]),
                    channelId=string_utils.to_int(obj.properties[ATTR_CHANNEL_ID]),
                    contentId=string_utils.to_int(obj.properties[ATTR_CONTENT_ID]),
                    documentId=string_utils.to_int(obj.properties[ATTR_DOCUMENT_ID]),
                    text=str(obj.properties[ATTR_TEXT]),
                    docName=str(obj.properties[ATTR_DOC_NAME]),
                    extName=str(obj.properties[ATTR_EXT_NAME]),
                    score=obj.metadata.score or 0,
                )
            )
        return search_results

    def delete_all(self) -> None:
        with self.connect() as client:
            client.collections.delete(self._collection_name)
            self.create_collection()

    def create_collection(self):
        with self.connect() as client:
            if not client.collections.exists(self._collection_name):
                client.collections.create(
                    self._collection_name,
                    vectorizer_config=wvc.config.Configure.Vectorizer.none(),
                    properties=[
                        wvcc.Property(name=ATTR_SITE_ID, data_type=wvcc.DataType.INT),
                        wvcc.Property(
                            name=ATTR_CHANNEL_ID, data_type=wvcc.DataType.INT
                        ),
                        wvcc.Property(
                            name=ATTR_CONTENT_ID, data_type=wvcc.DataType.INT
                        ),
                        wvcc.Property(
                            name=ATTR_DOCUMENT_ID, data_type=wvcc.DataType.INT
                        ),
                        wvcc.Property(name=ATTR_TEXT, data_type=wvcc.DataType.TEXT),
                        wvcc.Property(name=ATTR_DOC_NAME, data_type=wvcc.DataType.TEXT),
                        wvcc.Property(name=ATTR_EXT_NAME, data_type=wvcc.DataType.TEXT),
                    ],
                )

    def _get_filters(self, searchScope: SearchScope) -> _Filters | None:
        filters = None
        if searchScope.siteIds:
            if filters is None:
                filters = Filter.by_property(ATTR_SITE_ID).contains_any(
                    searchScope.siteIds
                )
            else:
                filters = filters | Filter.by_property(ATTR_SITE_ID).contains_any(
                    searchScope.siteIds
                )
        if searchScope.channelIds:
            if filters is None:
                filters = Filter.by_property(ATTR_CHANNEL_ID).contains_any(
                    searchScope.channelIds
                )
            else:
                filters = filters | Filter.by_property(ATTR_CHANNEL_ID).contains_any(
                    searchScope.channelIds
                )
        if searchScope.contentIds:
            if filters is None:
                filters = Filter.by_property(ATTR_CONTENT_ID).contains_any(
                    searchScope.contentIds
                )
            else:
                filters = filters | Filter.by_property(ATTR_CONTENT_ID).contains_any(
                    searchScope.contentIds
                )

        return filters
