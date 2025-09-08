from .__base import AiRequest
from fastapi import HTTPException
from repositories import site_repository
from dto import BoolResult
from enums import SearchType


async def settings_ai(request: AiRequest) -> BoolResult:
    site = site_repository.get(request.siteId)
    if site is None:
        raise HTTPException(status_code=404, detail="Site not found")

    values = site.site_values

    values.providerModelId = request.providerModelId
    values.llmSystemPrompt = request.llmSystemPrompt or ""

    values.datasetSearchType = SearchType(request.datasetSearchType)
    values.datasetMaxCount = request.datasetMaxCount
    values.datasetMinScore = request.datasetMinScore

    site.site_values = values
    site_repository.update_site(site)

    return BoolResult(value=True)
