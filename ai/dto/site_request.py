from pydantic import BaseModel


class SiteRequest(BaseModel):
    siteId: int
