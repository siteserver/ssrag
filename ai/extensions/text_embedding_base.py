from abc import ABC, abstractmethod


class TextEmbeddingBase(ABC):
    @abstractmethod
    def embedding(self, inputs: list[str]) -> list[list[float]]:
        """文本嵌入"""
        pass
