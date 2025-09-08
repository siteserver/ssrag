from typing import Optional
from datetime import datetime
from pydantic import BaseModel
import json
from utils import string_utils


class ChannelSummary(BaseModel):
    id: int
    channelName: Optional[str] = None
    parentId: Optional[int] = None
    parentsPath: Optional[list[int]] = None
    indexName: Optional[str] = None
    knowledge: Optional[bool] = None
    tableName: Optional[str] = None
    taxis: Optional[int] = None
    addDate: Optional[datetime] = None

    @classmethod
    def load_summaries(cls, json_str: str | None) -> list["ChannelSummary"]:
        if json_str is None:
            return []
        return json.loads(json_str, object_hook=cls.model_validate)

    @classmethod
    def json_summaries(cls, summaries: list["ChannelSummary"] | None) -> str:
        if summaries is None:
            return "[]"
        return string_utils.to_json_str(summaries)
