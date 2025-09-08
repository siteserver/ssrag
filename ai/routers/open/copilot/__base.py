from pydantic import BaseModel
from models import ChatGroup, Site, SiteValues, SiteSummary, ChatMessage


class GetResult(BaseModel):
    site: Site
    values: SiteValues
    sites: list[SiteSummary]
    chatGroups: list[ChatGroup]
    sessionId: str
    messages: list[ChatMessage] = []
