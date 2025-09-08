from pydantic import BaseModel
from enums import SearchType
from dto import Cascade
from vectors import Result


class GetRequest(BaseModel):
    siteId: int


class GetResult(BaseModel):
    siteUrl: str
    channels: list[Cascade]


class SubmitRequest(BaseModel):
    siteId: int
    channelId: int
    contentId: int
    searchType: SearchType
    maxCount: int
    minScore: float
    query: str


class SubmitResult(BaseModel):
    cuts: list[str]
    results: list[Result]
    channelIds: list[int]
