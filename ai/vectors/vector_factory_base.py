from abc import ABC, abstractmethod
from extensions import TextEmbeddingModel
from .vector_base import VectorBase


class VectorFactoryBase(ABC):
    @abstractmethod
    def create(self, text_embedding: TextEmbeddingModel) -> VectorBase:
        raise NotImplementedError
