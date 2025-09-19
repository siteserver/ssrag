from .__base import DeleteModelRequest
from repositories import model_repository, config_repository
from dto import BoolResult
from enums import ModelType


async def configsModels_delete_model(request: DeleteModelRequest) -> BoolResult:
    model = model_repository.get(request.providerId, request.modelId)
    if not model:
        return BoolResult(value=False)

    model_repository.delete(request.providerId, request.modelId)

    config_values = config_repository.get_values()
    changed = False

    if model.modelType == ModelType.LLM:
        if (
            config_values.defaultLLMProviderId == request.providerId
            and config_values.defaultLLMModelId == request.modelId
        ):
            config_values.defaultLLMProviderId = None
            config_values.defaultLLMModelId = None
            changed = True
    elif model.modelType == ModelType.TEXT_EMBEDDING:
        if (
            config_values.defaultTextEmbeddingProviderId == request.providerId
            and config_values.defaultTextEmbeddingModelId == request.modelId
        ):
            config_values.defaultTextEmbeddingProviderId = None
            config_values.defaultTextEmbeddingModelId = None
            changed = True
    elif model.modelType == ModelType.RERANK:
        if (
            config_values.defaultRerankProviderId == request.providerId
            and config_values.defaultRerankModelId == request.modelId
        ):
            config_values.defaultRerankProviderId = None
            config_values.defaultRerankModelId = None
            changed = True
    elif model.modelType == ModelType.TO_IMAGE:
        if (
            config_values.defaultToImageProviderId == request.providerId
            and config_values.defaultToImageModelId == request.modelId
        ):
            config_values.defaultToImageProviderId = None
            config_values.defaultToImageModelId = None
            changed = True
    elif model.modelType == ModelType.SPEECH2TEXT:
        if (
            config_values.defaultSpeech2TextProviderId == request.providerId
            and config_values.defaultSpeech2TextModelId == request.modelId
        ):
            config_values.defaultSpeech2TextProviderId = None
            config_values.defaultSpeech2TextModelId = None
            changed = True
    elif model.modelType == ModelType.TTS:
        if (
            config_values.defaultTTSProviderId == request.providerId
            and config_values.defaultTTSModelId == request.modelId
        ):
            config_values.defaultTTSProviderId = None
            config_values.defaultTTSModelId = None
            changed = True

    if changed:
        config_repository.update_values(config_values)

    return BoolResult(value=True)
