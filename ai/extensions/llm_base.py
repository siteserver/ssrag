from abc import ABC, abstractmethod
from dto import Message
from fastapi.responses import StreamingResponse


class LLMBase(ABC):
    @abstractmethod
    def chat(self, messages: list[Message], payload: dict | None = None) -> str:
        """聊天"""
        pass

    @abstractmethod
    def json(self, messages: list[Message], payload: dict | None = None) -> dict:
        """JSON"""
        pass

    @abstractmethod
    def chat_stream(
        self, messages: list[Message], thinking: bool, payload: dict | None = None
    ) -> StreamingResponse:
        """聊天流"""
        pass
