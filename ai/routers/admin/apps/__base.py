from pydantic import BaseModel
from models import SiteSummary


class GetResult(BaseModel):
    sites: list[SiteSummary]
    rootSiteId: int


class InitRequest(BaseModel):
    siteId: int


class DisableRequest(BaseModel):
    siteId: int


class OrderRequest(BaseModel):
    siteId: int
    isUp: bool
    rows: int


class DeleteAIRequest(BaseModel):
    siteId: int
    siteDir: str
