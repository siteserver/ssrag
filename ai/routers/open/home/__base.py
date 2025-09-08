from pydantic import BaseModel
from models import SiteSummary, ChatGroup, Site, SiteValues, ChatMessage


class GetResult(BaseModel):
    site: Site | None = None
    values: SiteValues | None = None
    sites: list[SiteSummary]
    chatGroups: list[ChatGroup]
    sessionId: str | None = None
    messages: list[ChatMessage] = []


class DeleteRequest(BaseModel):
    siteId: int
    sessionId: str


class RenameRequest(BaseModel):
    siteId: int
    sessionId: str
    title: str
