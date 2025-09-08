from typing import Optional
from pydantic import BaseModel
import json
from utils import string_utils
from enums import SiteType


class SiteSummary(BaseModel):
    id: int
    siteName: Optional[str] = None
    siteType: Optional[SiteType] = None
    iconUrl: Optional[str] = None
    siteDir: Optional[str] = None
    description: Optional[str] = None
    tableName: Optional[str] = None
    root: Optional[bool] = None
    disabled: Optional[bool] = None
    taxis: Optional[int] = None

    @classmethod
    def load_summaries(cls, json_str: str | None) -> list["SiteSummary"]:
        if json_str is None:
            return []
        return json.loads(json_str, object_hook=cls.model_validate)

    @classmethod
    def json_summaries(cls, summaries: list["SiteSummary"] | None) -> str:
        if summaries is None:
            return "[]"
        return string_utils.to_json_str(summaries)
