from pydantic import BaseModel
from models import ConfigValues


class GetResult(BaseModel):
    providers: list[dict]
    configValues: ConfigValues


class GetManifestsResult(BaseModel):
    manifests: list[dict]


class SubmitProviderRequest(BaseModel):
    providerId: str
    providerName: str
    iconUrl: str
    description: str
    credentials: dict


class SubmitModelRequest(BaseModel):
    id: int
    providerId: str
    modelType: str
    modelId: str
    skills: list[str] = []
    extendValues: dict


class DeleteProviderRequest(BaseModel):
    providerId: str


class DeleteModelRequest(BaseModel):
    providerId: str
    modelId: str


class GetDefaultsResult(BaseModel):
    defaultLLMProviderId: str = ""
    defaultLLMModelId: str = ""
    defaultTextEmbeddingProviderId: str = ""
    defaultTextEmbeddingModelId: str = ""
    defaultRerankProviderId: str = ""
    defaultRerankModelId: str = ""
    defaultSpeech2TextProviderId: str = ""
    defaultSpeech2TextModelId: str = ""
    defaultTTSProviderId: str = ""
    defaultTTSModelId: str = ""
    llmProviders: list[dict] = []
    textEmbeddingProviders: list[dict] = []
    rerankProviders: list[dict] = []
    speech2TextProviders: list[dict] = []
    ttsProviders: list[dict] = []


class SubmitDefaultsRequest(BaseModel):
    defaultLLMProviderId: str = ""
    defaultLLMModelId: str = ""
    defaultTextEmbeddingProviderId: str = ""
    defaultTextEmbeddingModelId: str = ""
    defaultRerankProviderId: str = ""
    defaultRerankModelId: str = ""
    defaultSpeech2TextProviderId: str = ""
    defaultSpeech2TextModelId: str = ""
    defaultTTSProviderId: str = ""
    defaultTTSModelId: str = ""


class StatusRequest(BaseModel):
    taskId: str


class StatusResult(BaseModel):
    taskId: str
    state: str
    result: dict
    detail: str
