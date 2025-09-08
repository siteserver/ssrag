from pydantic import BaseModel
from models import Dataset
from dto import Cascade


class GetResult(BaseModel):
    dataset: Dataset
    channels: list[Cascade]
    datasetChannelIds: list[list[int]]


class SubmitRequest(BaseModel):
    siteId: int
    nodeId: str
    datasetSiteId: int
    datasetAllChannels: bool
    datasetChannelIds: list[list[int]]
