from fastapi import APIRouter, UploadFile, Query, File
from .__base import (
    ChunkAndEmbedRequest,
    RemoveRequest,
    GetMarkdownRequest,
    ProcessRequest,
    StatusRequest,
    UploadResult,
    GetResult,
    GetRequest,
    ChunkAndEmbedResult,
    UploadRemoveRequest,
    ProcessResult,
    DownloadRequest,
    ListRequest,
    ListResult,
)
from configs import router_prefix
from dto import BoolResult, StringResult

router = APIRouter(
    prefix=router_prefix.ADMIN_DATASET_DOCUMENTS, tags=["dataset/documents"]
)


@router.get("")
async def get(siteId: int, channelId: int, contentId: int) -> GetResult:
    from .documents_get import documents_get

    request = GetRequest(siteId=siteId, channelId=channelId, contentId=contentId)
    return await documents_get(request)


@router.post("/actions/list")
async def list(request: ListRequest) -> ListResult:
    from .documents_list import documents_list

    return await documents_list(request)


@router.post("/actions/upload")
async def upload(
    siteId: int = Query(...),
    channelId: int = Query(...),
    contentId: int = Query(...),
    file: UploadFile = File(...),
) -> UploadResult:
    from .documents_upload import documents_upload

    return await documents_upload(siteId, channelId, contentId, file)


@router.post("/actions/remove")
async def remove(request: RemoveRequest) -> BoolResult:
    from .documents_remove import documents_remove

    return await documents_remove(request)


@router.post("/actions/get_markdown")
async def get_markdown(request: GetMarkdownRequest) -> StringResult:
    from .documents_get_markdown import documents_get_markdown

    return await documents_get_markdown(request)


@router.post("/actions/download")
async def download(request: DownloadRequest) -> StringResult:
    from .documents_download import documents_download

    return await documents_download(request)


@router.post("/actions/process")
def process(request: ProcessRequest) -> ProcessResult:
    from .documents_process import documents_process

    return documents_process(request)


@router.post("/actions/status")
def status(request: StatusRequest):
    """查询任务状态"""
    from .documents_status import documents_status

    return documents_status(request)


@router.post("/actions/chunk_and_embed")
async def chunk_and_embed(request: ChunkAndEmbedRequest) -> ChunkAndEmbedResult:
    from .documents_chunk_and_embed import documents_chunk_and_embed

    return await documents_chunk_and_embed(request)


@router.post("/actions/upload_remove")
async def upload_remove(request: UploadRemoveRequest) -> BoolResult:
    from .documents_upload_remove import documents_upload_remove

    return await documents_upload_remove(request)
