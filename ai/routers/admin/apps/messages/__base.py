from pydantic import BaseModel
from models import ChatGroup, ChatMessage


class GetRequest(BaseModel):
    siteId: int
    page: int = 1
    pageSize: int = 10
    dateStart: str | None = None
    dateEnd: str | None = None
    title: str | None = None


class GetResponse(BaseModel):
    chatGroups: list[ChatGroup]


class MessageRequest(BaseModel):
    siteId: int
    sessionId: str


class MessageResponse(BaseModel):
    messages: list[ChatMessage]
