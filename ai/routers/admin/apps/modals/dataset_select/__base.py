from pydantic import BaseModel
from models import SiteSummary


class GetResult(BaseModel):
    sites: list[SiteSummary]
    selectedSiteIds: list[int]


class SubmitRequest(BaseModel):
    siteId: int
    nodeId: str
    datasetSiteId: int


class DeleteRequest(BaseModel):
    siteId: int
    nodeId: str
    datasetSiteId: int
