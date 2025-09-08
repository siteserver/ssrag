from pydantic import BaseModel
from dto import Cascade, ChunkConfig, TaskDocumentProcess
from models import Document


class GetRequest(BaseModel):
    siteId: int
    channelId: int
    contentId: int


class GetResult(BaseModel):
    isModelReady: bool
    siteUrl: str
    channels: list[Cascade]


class ListRequest(BaseModel):
    siteId: int
    channelId: int
    contentId: int
    search: str = ""
    page: int = 1


class ListResult(BaseModel):
    documents: list[Document]
    total: int
    channelIds: list[int]


class UploadResult(BaseModel):
    task: TaskDocumentProcess


class RemoveRequest(BaseModel):
    documentId: int


class GetMarkdownRequest(BaseModel):
    documentId: int


class DownloadRequest(BaseModel):
    documentId: int


class ProcessRequest(BaseModel):
    tasks: list[TaskDocumentProcess]


class ProcessResult(BaseModel):
    taskIds: list[str]


class StatusRequest(BaseModel):
    taskIds: list[str]


class StatusResult(BaseModel):
    results: list[dict]


class ChunkAndEmbedRequest(BaseModel):
    documentId: int
    config: ChunkConfig


class ChunkAndEmbedResult(BaseModel):
    document: Document


class UploadRemoveRequest(BaseModel):
    task: TaskDocumentProcess
