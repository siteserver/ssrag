from models import FlowNodeSettings
from dto import RunVariable
from utils import string_utils
from dto import Message
from extensions import LLMFactory
from enums import ModelType
from repositories import model_provider_repository


def llm_manager_run_invoke(
    settings: FlowNodeSettings, inVariables: list[RunVariable]
) -> str:
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
    return llm.chat(messages)

    # # model = llm_manager_get_model(settings.modelId)

    # credentials = llm_manager_get_model_credentials(
    #     settings.providerId, settings.modelId
    # )
    # if credentials is None:
    #     raise Exception("模型不存在！")

    # model = ChatOpenAI(
    #     api_key=credentials.credentials["api_key"],
    #     model=credentials.modelId,
    # )

    # system_template = SystemMessagePromptTemplate.from_template(
    #     settings.llmSystemPrompt
    # )
    # human_template = HumanMessagePromptTemplate.from_template(settings.llmUserPrompt)
    # chat_prompt = (
    #     ChatPromptTemplate.from_messages([system_template, human_template]) | model
    # )

    # dict = {}
    # for variable in inVariables:
    #     dict[variable.name] = variable.value

    # result = chat_prompt.invoke(dict)
    # content = result.content
    # return str(content) if isinstance(content, list) else content
