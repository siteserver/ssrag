import uuid
from fastapi import HTTPException
from repositories import (
    site_repository,
    chat_message_repository,
)
from .__base import GetResult
from models import Site
from enums import PromptPosition
from repositories import prompt_repository


async def chat_get(
    id: str | None = None, siteId: int | None = None, sessionId: str | None = None
) -> GetResult:
    site: Site | None = None

    if id:
        site = site_repository.get_by_uuid(id)
    elif siteId:
        site = site_repository.get(siteId)

    if site is None:
        raise HTTPException(status_code=404, detail="Site not found")

    values = site.site_values

    if sessionId:
        messages = chat_message_repository.get_all(site.id, sessionId)
    else:
        sessionId = str(uuid.uuid4())
        messages = []

    prompts = prompt_repository.get_all(site.id)
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
        sessionId=sessionId,
        messages=messages,
        hotPrompts=hotPrompts,
        functionPrompts=functionPrompts,
        inputPrompts=inputPrompts,
    )
