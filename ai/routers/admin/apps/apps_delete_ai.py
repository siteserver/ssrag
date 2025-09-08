from .__base import DeleteAIRequest
from repositories import (
    flow_node_repository,
    flow_edge_repository,
    flow_variable_repository,
    dataset_repository,
    document_repository,
    segment_repository,
    site_repository,
)
from dto import BoolResult
from fastapi import HTTPException
from vectors import Vector


async def apps_delete_ai(request: DeleteAIRequest) -> BoolResult:
    site = site_repository.get(request.siteId)
    if not site:
        raise HTTPException(status_code=404, detail="Site not found")

    if site.siteDir.strip().lower() != request.siteDir.strip().lower():
        raise HTTPException(status_code=400, detail="删除失败，请输入正确的文件夹名称")

    document_repository.delete_by_site_id(request.siteId)
    segment_repository.delete_by_site_id(request.siteId)
    vector = Vector()
    vector.delete_by_site_id(request.siteId)

    flow_node_repository.delete_all(request.siteId)
    flow_edge_repository.delete_all(request.siteId)
    flow_variable_repository.delete_all(request.siteId)
    dataset_repository.delete_all(request.siteId)

    site_repository.clear_cache()

    return BoolResult(value=True)
