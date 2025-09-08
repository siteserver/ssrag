from fastapi import HTTPException
from repositories import dataset_repository, site_repository
from services import llm_manager
from enums import ModelType
from .__base import GetResult
from repositories import prompt_repository
from enums import PromptPosition


async def settings_get(siteId: int) -> GetResult:
    site = site_repository.get(siteId)
    if site is None:
        raise HTTPException(status_code=404, detail="Site not found")

    values = site.site_values
    models, defaultModel = llm_manager.get_models(ModelType.LLM)

    sites = site_repository.get_summaries()
    selected_site_ids = dataset_repository.get_selected_site_ids(siteId, site.uuid)
    datasetSites = [site for site in sites if site.id in selected_site_ids]

    prompts = prompt_repository.get_all(siteId)
    hotPrompts = [prompt for prompt in prompts if prompt.position == PromptPosition.HOT]
    functionPrompts = [
        prompt for prompt in prompts if prompt.position == PromptPosition.FUNCTION
    ]
    inputPrompts = [
        prompt for prompt in prompts if prompt.position == PromptPosition.INPUT
    ]

    return GetResult(
        site=site,
        values=values,
        models=models,
        defaultModel=defaultModel,
        datasetSites=datasetSites,
        hotPrompts=hotPrompts,
        functionPrompts=functionPrompts,
        inputPrompts=inputPrompts,
    )
