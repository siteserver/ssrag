from .__base import InitRequest
from repositories import (
    flow_node_repository,
    flow_edge_repository,
    flow_variable_repository,
    site_repository,
    prompt_repository,
)
from models import FlowEdge, FlowVariable
from models.flow_node import NodeType, FlowNodeSettings
from enums.variable_type import VariableType
from enums.variable_data_type import VariableDataType
import uuid
from dto import BoolResult
from enums import SiteType, PromptPosition
from models import Prompt


async def apps_init(request: InitRequest) -> BoolResult:
    site = site_repository.get(request.siteId)
    if site is None or not SiteType.is_app_site(SiteType(site.siteType)):
        return BoolResult(value=False)

    prompts = []
    prompts.append(
        Prompt(
            uuid=str(uuid.uuid4()),
            title="",
            iconUrl="",
            text="🔥 最近有哪些热门的新潮流？",
            position=PromptPosition.INPUT,
            siteId=site.id,
            taxis=len(prompts) + 1,
        )
    )
    prompts.append(
        Prompt(
            uuid=str(uuid.uuid4()),
            title="",
            iconUrl="",
            text="🤔 有啥新奇好玩的东西推荐吗？",
            position=PromptPosition.INPUT,
            siteId=site.id,
            taxis=len(prompts) + 1,
        )
    )
    prompts.append(
        Prompt(
            uuid=str(uuid.uuid4()),
            title="",
            iconUrl="",
            text="🔍 怎么发现新奇的事物呢？",
            position=PromptPosition.INPUT,
            siteId=site.id,
            taxis=len(prompts) + 1,
        )
    )
    for prompt in prompts:
        prompt_repository.create(prompt)

    if site.siteType == SiteType.CHATFLOW:
        start_guid = str(uuid.uuid4())
        flow_node_repository.insert(
            start_guid,
            FlowNodeSettings(
                siteId=site.id,
                nodeType=NodeType.START,
                title="流程开始",
                positionX=100,
                positionY=100,
            ),
        )

        dataset_guid = str(uuid.uuid4())
        flow_node_repository.insert(
            dataset_guid,
            FlowNodeSettings(
                siteId=site.id,
                nodeType=NodeType.DATASET,
                title="知识库",
                positionX=500,
                positionY=100,
            ),
        )

        flow_variable_repository.insert(
            FlowVariable(
                uuid=str(uuid.uuid4()),
                siteId=site.id,
                nodeId=dataset_guid,
                type=VariableType.INPUT.value,
                name="input",
                dataType=VariableDataType.STRING.value,
                isDisabled=False,
                isReference=True,
                referenceNodeId=start_guid,
                referenceName="output",
                value="",
            ),
        )

        llm_guid = str(uuid.uuid4())
        flow_node_repository.insert(
            llm_guid,
            FlowNodeSettings(
                siteId=site.id,
                nodeType=NodeType.LLM,
                title="大模型",
                llmSystemPrompt="参考资料：\n{{context}}",
                llmUserPrompt="{{input}}",
                llmIsReply=True,
                positionX=900,
                positionY=100,
            ),
        )

        flow_variable_repository.insert(
            FlowVariable(
                uuid=str(uuid.uuid4()),
                siteId=site.id,
                nodeId=llm_guid,
                type=VariableType.INPUT.value,
                name="input",
                dataType=VariableDataType.STRING.value,
                isDisabled=False,
                isReference=True,
                referenceNodeId=start_guid,
                referenceName="output",
                value="",
            ),
        )
        flow_variable_repository.insert(
            FlowVariable(
                uuid=str(uuid.uuid4()),
                siteId=site.id,
                nodeId=llm_guid,
                type=VariableType.INPUT.value,
                name="context",
                dataType=VariableDataType.STRING.value,
                isDisabled=False,
                isReference=True,
                referenceNodeId=dataset_guid,
                referenceName="output",
                value="",
            ),
        )

        flow_edge_repository.insert(
            FlowEdge(
                uuid=str(uuid.uuid4()),
                siteId=site.id,
                source=start_guid,
                sourceHandle="",
                target=dataset_guid,
                targetHandle="",
            ),
        )
        flow_edge_repository.insert(
            FlowEdge(
                uuid=str(uuid.uuid4()),
                siteId=site.id,
                source=dataset_guid,
                sourceHandle="",
                target=llm_guid,
                targetHandle="",
            ),
        )

    return BoolResult(value=True)
