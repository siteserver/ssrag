from .__base import SubmitDefaultsRequest
from repositories import config_repository
from dto import StringResult
from celery_app import task_embedding_changed


async def configsModels_submit_default(request: SubmitDefaultsRequest) -> StringResult:
    config_values = config_repository.get_values()

    rebuild_embedding = (
        request.defaultTextEmbeddingProviderId
        != config_values.defaultTextEmbeddingProviderId
        or request.defaultTextEmbeddingModelId
        != config_values.defaultTextEmbeddingModelId
    )

    config_values.defaultLLMProviderId = request.defaultLLMProviderId
    config_values.defaultLLMModelId = request.defaultLLMModelId
    config_values.defaultTextEmbeddingProviderId = (
        request.defaultTextEmbeddingProviderId
    )
    config_values.defaultTextEmbeddingModelId = request.defaultTextEmbeddingModelId
    config_values.defaultRerankProviderId = request.defaultRerankProviderId
    config_values.defaultRerankModelId = request.defaultRerankModelId
    config_values.defaultToImageProviderId = request.defaultToImageProviderId
    config_values.defaultToImageModelId = request.defaultToImageModelId
    config_values.defaultSpeech2TextProviderId = request.defaultSpeech2TextProviderId
    config_values.defaultSpeech2TextModelId = request.defaultSpeech2TextModelId
    config_values.defaultTTSProviderId = request.defaultTTSProviderId
    config_values.defaultTTSModelId = request.defaultTTSModelId
    config_values.init = True
    config_repository.update_values(config_values)

    task_id = ""
    if rebuild_embedding:
        task = task_embedding_changed.delay()
        task_id = task.id

    return StringResult(value=task_id)
