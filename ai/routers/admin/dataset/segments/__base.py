from pydantic import BaseModel
from models import ChannelSummary, Segment


class GetRequest(BaseModel):
    siteId: int
    channelId: int
    documentId: int
    page: int = 1
    pageSize: int = 10


class GetResult(BaseModel):
    segments: list[Segment]
    count: int
    breadcrumb: list[ChannelSummary]


class UpdateRequest(BaseModel):
    documentId: int
    segmentId: str
    content: str


class InsertRequest(BaseModel):
    documentId: int
    taxis: int
    content: str


class InsertResult(BaseModel):
    segment: Segment | None
