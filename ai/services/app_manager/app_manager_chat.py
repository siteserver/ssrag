from models import Site, FlowNodeSettings
from fastapi.responses import StreamingResponse
from repositories import config_repository
from enums import NodeType
from dto import RunVariable
from configs import constants
from .app_manager_process_dataset import app_manager_process_dataset
from .app_manager_process_llm import app_manager_process_llm
import uuid
from vectors import Vector


def app_manager_chat(
    site: Site, vector: Vector, message: str, thinking: bool, searching: bool
) -> StreamingResponse:
    config_values = config_repository.get_values()
    site_values = site.site_values

    providerModelId = site_values.providerModelId
    if not providerModelId:
        providerModelId = (
            f"{config_values.defaultLLMProviderId}:{config_values.defaultLLMModelId}"
        )

    settings = FlowNodeSettings(
        id=site.uuid,
        siteId=site.id,
        nodeType=NodeType.DATASET,
        datasetSearchType=site_values.datasetSearchType,
        datasetMaxCount=site_values.datasetMaxCount,
        datasetMinScore=site_values.datasetMinScore,
    )
    inVariables = [RunVariable(name=constants.DEFAULT_INPUT_NAME, value=message)]
    inVariablesDict: dict[str, list[RunVariable]] = {}
    outVariablesDict: dict[str, list[RunVariable]] = {}
    inVariablesDict[settings.id] = inVariables
    process = app_manager_process_dataset(vector, settings, inVariables)
    outVariables = process.outVariables
    outVariablesDict[settings.id] = outVariables

    llmSystemPrompt = f"""
        {site_values.llmSystemPrompt}
        
        You are a helpful assistant.
        You are given a context and a message.
        You need to answer the message based on the context.
        Context:
        {outVariables[0].value}
        Message:
        {message}
        """

    settings = FlowNodeSettings(
        id=str(uuid.uuid4()),
        siteId=site.id,
        nodeType=NodeType.LLM,
        llmSystemPrompt=llmSystemPrompt,
        llmUserPrompt=message,
        llmIsReply=True,
        providerModelId=providerModelId,
    )
    inVariables = [
        RunVariable(name=constants.DEFAULT_INPUT_NAME, value=message),
        RunVariable(name="context", value=outVariables),
    ]
    inVariablesDict[settings.id] = inVariables
    process = app_manager_process_llm(settings, inVariables, thinking)
    if process.response is None:
        raise Exception("Response not found")
    return process.response
