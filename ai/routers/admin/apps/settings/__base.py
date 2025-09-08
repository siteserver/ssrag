from pydantic import BaseModel
from models import Site, SiteSummary, SiteValues, Prompt
from dto import Model
from enums import SearchType


class PromptRequest(BaseModel):
    uuid: str | None = None
    title: str | None = None
    iconUrl: str | None = None
    text: str


class GetResult(BaseModel):
    site: Site
    values: SiteValues
    models: list[Model]
    defaultModel: Model | None
    datasetSites: list[SiteSummary]
    hotPrompts: list[Prompt]
    functionPrompts: list[Prompt]
    inputPrompts: list[Prompt]


class StylesRequest(BaseModel):
    siteId: int

    displayType: str
    headerText: str
    footerText: str

    welcomeTitle: str
    welcomeVariant: str
    welcomePosition: str
    iconUrl: str | None = None
    description: str

    isHotPrompts: bool
    hotPromptsTitle: str | None = None
    hotPrompts: list[PromptRequest]
    isFunctionPrompts: bool
    functionPromptsTitle: str | None = None
    functionPrompts: list[PromptRequest]
    isInputPrompts: bool
    inputPrompts: list[PromptRequest]

    senderPlaceholder: str
    senderAllowSpeech: bool


class AiRequest(BaseModel):
    siteId: int
    providerModelId: str
    llmSystemPrompt: str | None = None
    datasetSearchType: SearchType
    datasetMaxCount: int
    datasetMinScore: float
