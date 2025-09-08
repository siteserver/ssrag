from pydantic import BaseModel


class IndexRequest(BaseModel):
    siteId: int
    channelId: int | None = None
    contentId: int | None = None


class RemoveRequest(BaseModel):
    siteId: int
    channelId: int | None = None
    contentId: int | None = None
