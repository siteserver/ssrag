from models import FlowNodeSettings
from fastapi.responses import StreamingResponse
from dto import RunVariable
from utils import string_utils
from dto import Message
from extensions import LLMFactory
from enums import ModelType
from repositories import model_provider_repository


def llm_manager_run_stream(
    settings: FlowNodeSettings, inVariables: list[RunVariable], thinking: bool
) -> StreamingResponse:
    messages = []

    dict = {}
    for variable in inVariables:
        dict[variable.name] = variable.value

    userMessage = string_utils.parse_jinja2(settings.llmUserPrompt or "{{input}}", dict)
    messages.append(Message(role="user", content=userMessage))

    if settings.llmSystemPrompt:
        systemMessage = string_utils.parse_jinja2(settings.llmSystemPrompt, dict)
        messages.append(Message(role="system", content=systemMessage))

    provider_id, model_id = string_utils.extract_provider_model_id(
        settings.providerModelId
    )
    if not provider_id or not model_id:
        raise Exception("Provider model id is required")

    model_credentials = model_provider_repository.get_model_credentials(
        ModelType.LLM, provider_id, model_id
    )
    if model_credentials is None:
        raise Exception("模型不存在！")

    llm = LLMFactory.create(model_credentials)
    return llm.chat_stream(messages, thinking)
