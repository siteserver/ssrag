from .__base import SubmitRequest
from utils import string_utils
from repositories import site_repository, flow_node_repository
from fastapi import HTTPException
from fastapi.responses import StreamingResponse
from services import AppManager, llm_manager
from dto import RunVariable
from configs import constants
from utils import bocha_utils
from models import Site
from enums import SiteType


async def chat_with_llm(
    providerId: str, modelId: str, message: str, thinking: bool, searching: bool
) -> StreamingResponse:
    systemMessage: str | None = None
    if searching:
        response = bocha_utils.bocha_websearch_tool(
            api_key="sk-0156fc92f825461b98a1c05ad08941ba",
            query=message,
            freshness="noLimit",
            summary=True,
            include="",
            exclude="",
            count=10,
        )
        if response.success and response.webPages:
            formatted_results = ""
            for idx, page in enumerate(response.webPages, start=1):
                formatted_results += (
                    f"引用: {idx}\n"
                    f"标题: {page.name}\n"
                    f"URL: {page.url}\n"
                    f"摘要: {page.summary}\n"
                    f"网站名称: {page.siteName}\n"
                    f"网站图标: {page.siteIcon}\n"
                    f"发布时间: {page.dateLastCrawled}\n\n"
                )
            systemMessage = string_utils.parse_jinja2(
                "以下是搜索结果，请参考搜索结果回答问题：\n\n{{results}}",
                {"results": formatted_results.strip()},
            )
    return llm_manager.chat(providerId, modelId, message, systemMessage, thinking)


async def run_chat(
    site: Site, message: str, thinking: bool, searching: bool
) -> StreamingResponse:
    app_manager = AppManager(site)
    return app_manager.chat(message, thinking, searching)


async def run_chatflow(site: Site, message: str, thinking: bool) -> StreamingResponse:
    app_manager = AppManager(site)
    node = flow_node_repository.get_start_node(site.id)
    if not node:
        raise HTTPException(status_code=400, detail="Node not found")

    settings = app_manager.get_settings(node)
    inVariables = [RunVariable(name=constants.DEFAULT_INPUT_NAME, value=message)]
    inVariablesDict: dict[str, list[RunVariable]] = {}
    outVaraiblesDict: dict[str, list[RunVariable]] = {}
    inVariablesDict[node.uuid] = inVariables

    return app_manager.process(settings, thinking, inVariablesDict, outVaraiblesDict)


async def chat_submit(request: SubmitRequest) -> StreamingResponse:
    site = site_repository.get(request.siteId)
    if not site:
        raise HTTPException(status_code=400, detail="Site not found")

    if site.siteType == SiteType.CHAT:
        return await run_chat(
            site, request.message, request.thinking, request.searching
        )
    elif site.siteType == SiteType.CHATFLOW or site.siteType == SiteType.AGENT:
        return await run_chatflow(site, request.message, request.thinking)
    else:
        raise HTTPException(status_code=400, detail="Invalid site type")
