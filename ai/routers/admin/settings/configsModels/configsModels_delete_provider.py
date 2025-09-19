from .__base import DeleteProviderRequest
from repositories import model_provider_repository
from repositories import config_repository
from dto import BoolResult


async def configsModels_delete_provider(request: DeleteProviderRequest) -> BoolResult:
    model_provider_repository.delete(request.providerId)

    config_values = config_repository.get_values()
    changed = False
    if config_values.defaultLLMProviderId == request.providerId:
        config_values.defaultLLMProviderId = None
        config_values.defaultLLMModelId = None
        changed = True
    if config_values.defaultTextEmbeddingProviderId == request.providerId:
        config_values.defaultTextEmbeddingProviderId = None
        config_values.defaultTextEmbeddingModelId = None
        changed = True
    if config_values.defaultRerankProviderId == request.providerId:
        config_values.defaultRerankProviderId = None
        config_values.defaultRerankModelId = None
        changed = True
    if config_values.defaultToImageProviderId == request.providerId:
        config_values.defaultToImageProviderId = None
        config_values.defaultToImageModelId = None
        changed = True
    if config_values.defaultSpeech2TextProviderId == request.providerId:
        config_values.defaultSpeech2TextProviderId = None
        config_values.defaultSpeech2TextModelId = None
        changed = True
    if config_values.defaultTTSProviderId == request.providerId:
        config_values.defaultTTSProviderId = None
        config_values.defaultTTSModelId = None
        changed = True

    if changed:
        config_repository.update_values(config_values)

    return BoolResult(value=True)
