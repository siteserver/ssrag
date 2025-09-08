from fastapi.responses import StreamingResponse
from repositories import model_provider_repository
from dto import Message
from enums import ModelType
from extensions import LLMFactory


def llm_manager_chat(
    providerId: str,
    modelId: str,
    userMessage: str,
    systemMessage: str | None = None,
    enable_thinking: bool = True,
) -> StreamingResponse:
    messages: list[Message] = []
    if systemMessage:
        messages.append(Message(role="system", content=systemMessage))
    if userMessage:
        messages.append(Message(role="user", content=userMessage))

    model_credentials = model_provider_repository.get_model_credentials(
        ModelType.LLM, providerId, modelId
    )
    if model_credentials is None:
        raise Exception("模型不存在！")

    llm = LLMFactory.create(model_credentials)
    payload = {"enable_thinking": enable_thinking}
    return llm.chat_stream(messages, enable_thinking, payload)
