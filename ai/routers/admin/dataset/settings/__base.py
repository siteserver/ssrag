from pydantic import BaseModel
from models import SiteValues


class GetResult(BaseModel):
    settings: SiteValues | None


class SubmitRequest(BaseModel):
    siteId: int
    separators: list[str]
    chunkSize: int
    chunkOverlap: int
    isChunkReplaces: bool
    isChunkDeletes: bool
    isRechunk: bool
