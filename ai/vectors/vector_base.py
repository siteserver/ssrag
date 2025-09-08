from abc import ABC, abstractmethod
from models import Document, Segment
from extensions import TextEmbeddingModel
from dto import SearchScope
from .result import Result


class VectorBase(ABC):
    def __init__(self, text_embedding: TextEmbeddingModel):
        self._text_embedding = text_embedding

    @abstractmethod
    def get_type(self) -> str:
        raise NotImplementedError

    @abstractmethod
    def add(self, document: Document, segments: list[Segment]) -> None:
        raise NotImplementedError

    @abstractmethod
    def update(self, uuid: str, text: str) -> None:
        raise NotImplementedError

    @abstractmethod
    def exists(self, id: str) -> bool:
        raise NotImplementedError

    @abstractmethod
    def delete_by_ids(self, ids: list[str]) -> None:
        raise NotImplementedError

    @abstractmethod
    def delete_by_site_id(self, site_id: int) -> None:
        raise NotImplementedError

    @abstractmethod
    def delete_by_channel_id(self, site_id: int, channel_id: int) -> None:
        raise NotImplementedError

    @abstractmethod
    def delete_by_content_id(
        self, site_id: int, channel_id: int, content_id: int
    ) -> None:
        raise NotImplementedError

    @abstractmethod
    def delete_by_document_id(self, document_id: int) -> None:
        raise NotImplementedError

    @abstractmethod
    def delete_all(self) -> None:
        raise NotImplementedError

    @abstractmethod
    def search_by_semantic(
        self,
        searchScope: SearchScope,
        query: str,
        maxCount: int = 10,
        minScore: float = 0.3,
    ) -> list[Result]:
        raise NotImplementedError

    @abstractmethod
    def search_by_full_text(
        self,
        searchScope: SearchScope,
        query: str,
        maxCount: int = 10,
        minScore: float = 0.3,
    ) -> list[Result]:
        raise NotImplementedError

    @abstractmethod
    def search_by_hybrid(
        self,
        searchScope: SearchScope,
        query: str,
        maxCount: int = 10,
        minScore: float = 0.3,
    ) -> list[Result]:
        raise NotImplementedError

    @abstractmethod
    def create_collection(self):
        raise NotImplementedError
