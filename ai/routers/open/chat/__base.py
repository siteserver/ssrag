from pydantic import BaseModel
from models import Site, SiteValues, ChatGroup, ChatMessage, Prompt


class GetResult(BaseModel):
    site: Site
    values: SiteValues
    sessionId: str
    messages: list[ChatMessage] = []
    hotPrompts: list[Prompt] = []
    functionPrompts: list[Prompt] = []
    inputPrompts: list[Prompt] = []


class SubmitRequest(BaseModel):
    siteId: int
    thinking: bool
    searching: bool
    message: str


class HistoryRequest(BaseModel):
    siteId: int
    thinking: bool
    searching: bool
    sessionId: str | None = None
    message: str
    reasoning: str
    content: str


class HistoryResult(BaseModel):
    chatGroup: ChatGroup
