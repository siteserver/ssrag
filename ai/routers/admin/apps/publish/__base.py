from pydantic import BaseModel
from models import Site, SiteValues


class GetRequest(BaseModel):
    siteId: int


class GetResult(BaseModel):
    site: Site
    values: SiteValues
