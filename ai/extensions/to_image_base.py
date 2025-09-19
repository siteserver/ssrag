from abc import ABC, abstractmethod


class ToImageBase(ABC):
    @abstractmethod
    def generate(self, prompt: str, image_size: str, batch_size: int) -> list[str]:
        """图片生成"""
        pass