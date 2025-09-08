from configs import app_configs
from extensions import TextEmbeddingModel
from models import Document, Segment
from dto import SearchScope
from enums import SearchType
from repositories import segment_repository
import uuid

from .vector_type import VectorType
from .vector_base import VectorBase
from .vector_factory_base import VectorFactoryBase
from .result import Result


class Vector:
    def __init__(self):
        self._text_embedding = TextEmbeddingModel()
        self._vector = self._create()

    def _create(self) -> VectorBase:
        vector_type = app_configs.VECTOR_STORE

        if not vector_type:
            raise ValueError("Vector store must be specified.")

        vector_factory_cls = self.get_vector_factory(vector_type)
        return vector_factory_cls().create(self._text_embedding)

    @staticmethod
    def get_vector_factory(vector_type: str) -> type[VectorFactoryBase]:
        match vector_type:
            case VectorType.PGVECTOR:
                from .pgvector import PGVectorFactory

                return PGVectorFactory
            case VectorType.WEAVIATE:
                from .weaviate import WeaviateFactory

                return WeaviateFactory
            case _:
                raise ValueError(f"Vector store {vector_type} is not supported.")

    def add_texts(self, document: Document, texts: list[str]):
        if not document or not texts:
            return
        segments: list[Segment] = []
        for index, text in enumerate(texts):
            taxis = index + 1
            segment = Segment(
                id=str(uuid.uuid4()),
                siteId=document.siteId,
                channelId=document.channelId,
                contentId=document.contentId,
                documentId=document.id,
                taxis=taxis,
                text=text,
            )
            segments.append(segment)

        self.add_segments(document, segments)

    def add_segments(self, document: Document, segments: list[Segment]):
        if not document or not segments:
            return
        for segment in segments:
            segment_repository.insert(segment)
        self._vector.add(document, segments)

    def update(self, uuid: str, text: str):
        if not uuid or not text:
            return
        segment_repository.update_content(uuid, text)
        self._vector.update(uuid, text)

    def exists(self, id: str) -> bool:
        return self._vector.exists(id)

    def delete_by_ids(self, ids: list[str]) -> None:
        self._vector.delete_by_ids(ids)

    def delete_by_site_id(self, site_id: int) -> None:
        self._vector.delete_by_site_id(site_id)

    def delete_by_channel_id(self, site_id: int, channel_id: int) -> None:
        self._vector.delete_by_channel_id(site_id, channel_id)

    def delete_by_content_id(
        self, site_id: int, channel_id: int, content_id: int
    ) -> None:
        self._vector.delete_by_content_id(site_id, channel_id, content_id)

    def delete_by_document_id(self, document_id: int) -> None:
        self._vector.delete_by_document_id(document_id)

    def delete_all(self) -> None:
        self._vector.delete_all()

    def search(
        self,
        searchScope: SearchScope,
        query: str,
        searchType: SearchType = SearchType.SEMANTIC,
        maxCount: int = 10,
        minScore: float = 0.5,
    ) -> list[Result]:
        results: list[Result] = []

        if searchType == SearchType.SEMANTIC:
            results = self._vector.search_by_semantic(
                searchScope, query, maxCount, minScore
            )
        elif searchType == SearchType.FULL_TEXT:
            results = self._vector.search_by_full_text(
                searchScope, query, maxCount, minScore
            )
        elif searchType == SearchType.HYBRID:
            results = self._vector.search_by_hybrid(
                searchScope, query, maxCount, minScore
            )

        return results

    def create_collection(self):
        self._vector.create_collection()

    # def __getattr__(self, name):
    #     if self._vector is not None:
    #         method = getattr(self._vector, name)
    #         if callable(method):
    #             return method

    #     raise AttributeError(f"'vector_processor' object has no attribute '{name}'")
