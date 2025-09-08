from .__base import StylesRequest
from fastapi import HTTPException
from repositories import site_repository, prompt_repository
from dto import BoolResult
from enums import PromptPosition
from models import Prompt
import uuid


async def settings_styles(request: StylesRequest) -> BoolResult:
    site = site_repository.get(request.siteId)
    if site is None:
        raise HTTPException(status_code=404, detail="Site not found")

    site.iconUrl = request.iconUrl or ""
    site.description = request.description

    values = site.site_values

    values.displayType = request.displayType
    values.headerText = request.headerText
    values.footerText = request.footerText

    values.welcomeTitle = request.welcomeTitle
    values.welcomeVariant = request.welcomeVariant
    values.welcomePosition = request.welcomePosition

    values.isHotPrompts = request.isHotPrompts
    values.hotPromptsTitle = request.hotPromptsTitle or ""
    values.isFunctionPrompts = request.isFunctionPrompts
    values.functionPromptsTitle = request.functionPromptsTitle or ""
    values.isInputPrompts = request.isInputPrompts

    prompts = []
    if request.isHotPrompts and request.hotPrompts:
        for hotPrompt in request.hotPrompts:
            # 明确指定主键为None，确保数据库能自动生成主键
            prompt = Prompt(
                uuid=str(uuid.uuid4()),
                title=hotPrompt.title or "",
                iconUrl=hotPrompt.iconUrl or "",
                text=hotPrompt.text,
                position=PromptPosition.HOT,
                siteId=site.id,
                taxis=len(prompts) + 1,
            )
            prompts.append(prompt)
    if request.isFunctionPrompts and request.functionPrompts:
        for functionPrompt in request.functionPrompts:
            # 明确指定主键为None，确保数据库能自动生成主键
            prompt = Prompt(
                uuid=str(uuid.uuid4()),
                title=functionPrompt.title or "",
                iconUrl=functionPrompt.iconUrl or "",
                text=functionPrompt.text,
                position=PromptPosition.FUNCTION,
                siteId=site.id,
                taxis=len(prompts) + 1,
            )
            prompts.append(prompt)
    if request.isInputPrompts and request.inputPrompts:
        for inputPrompt in request.inputPrompts:
            # 明确指定主键为None，确保数据库能自动生成主键
            prompt = Prompt(
                uuid=str(uuid.uuid4()),
                title=inputPrompt.title or "",
                iconUrl=inputPrompt.iconUrl or "",
                text=inputPrompt.text,
                position=PromptPosition.INPUT,
                siteId=site.id,
                taxis=len(prompts) + 1,
            )
            prompts.append(prompt)

    prompt_repository.delete_all(site.id)
    for prompt in prompts:
        prompt_repository.create(prompt)

    values.senderPlaceholder = request.senderPlaceholder
    values.senderAllowSpeech = request.senderAllowSpeech

    site.site_values = values
    site_repository.update_site(site)

    return BoolResult(value=True)
