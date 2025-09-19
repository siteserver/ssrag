from enums import ModelType
from repositories import config_repository, model_provider_repository, model_repository
from .__base import GetDefaultsResult


async def configsModels_get_default() -> GetDefaultsResult:
    config_values = config_repository.get_values()

    llmProviders = []
    textEmbeddingProviders = []
    rerankProviders = []
    toImageProviders = []
    speech2TextProviders = []
    ttsProviders = []

    db_providers = model_provider_repository.get_all()
    for db_provider in db_providers:
        llmModels = []
        textEmbeddingModels = []
        rerankModels = []
        toImageModels = []
        speech2TextModels = []
        ttsModels = []
        db_models = model_repository.get_all_by_provider_id(db_provider.providerId)
        for db_model in db_models:
            model = {
                "providerId": db_model.providerId,
                "modelId": db_model.modelId,
            }
            if db_model.modelType == ModelType.LLM:
                llmModels.append(model)
            elif db_model.modelType == ModelType.TEXT_EMBEDDING:
                textEmbeddingModels.append(model)
            elif db_model.modelType == ModelType.RERANK:
                rerankModels.append(model)
            elif db_model.modelType == ModelType.TO_IMAGE:
                toImageModels.append(model)
            elif db_model.modelType == ModelType.SPEECH2TEXT:
                speech2TextModels.append(model)
            elif db_model.modelType == ModelType.TTS:
                ttsModels.append(model)
        if len(llmModels) > 0:
            llmProviders.append(
                {
                    "providerId": db_provider.providerId,
                    "providerName": db_provider.providerName,
                    "iconUrl": db_provider.iconUrl,
                    "description": db_provider.description,
                    "models": llmModels,
                }
            )
        if len(textEmbeddingModels) > 0:
            textEmbeddingProviders.append(
                {
                    "providerId": db_provider.providerId,
                    "providerName": db_provider.providerName,
                    "iconUrl": db_provider.iconUrl,
                    "description": db_provider.description,
                    "models": textEmbeddingModels,
                }
            )
        if len(rerankModels) > 0:
            rerankProviders.append(
                {
                    "providerId": db_provider.providerId,
                    "providerName": db_provider.providerName,
                    "iconUrl": db_provider.iconUrl,
                    "description": db_provider.description,
                    "models": rerankModels,
                }
            )
        if len(toImageModels) > 0:
            toImageProviders.append(
                {
                    "providerId": db_provider.providerId,
                    "providerName": db_provider.providerName,
                    "iconUrl": db_provider.iconUrl,
                    "description": db_provider.description,
                    "models": toImageModels,
                }
            )
        if len(speech2TextModels) > 0:
            speech2TextProviders.append(
                {
                    "providerId": db_provider.providerId,
                    "providerName": db_provider.providerName,
                    "iconUrl": db_provider.iconUrl,
                    "description": db_provider.description,
                    "models": speech2TextModels,
                }
            )
        if len(ttsModels) > 0:
            ttsProviders.append(
                {
                    "providerId": db_provider.providerId,
                    "providerName": db_provider.providerName,
                    "iconUrl": db_provider.iconUrl,
                    "description": db_provider.description,
                    "models": ttsModels,
                }
            )

    return GetDefaultsResult(
        defaultLLMProviderId=config_values.defaultLLMProviderId or "",
        defaultLLMModelId=config_values.defaultLLMModelId or "",
        defaultTextEmbeddingProviderId=config_values.defaultTextEmbeddingProviderId
        or "",
        defaultTextEmbeddingModelId=config_values.defaultTextEmbeddingModelId or "",
        defaultRerankProviderId=config_values.defaultRerankProviderId or "",
        defaultRerankModelId=config_values.defaultRerankModelId or "",
        defaultToImageProviderId=config_values.defaultToImageProviderId or "",
        defaultToImageModelId=config_values.defaultToImageModelId or "",
        defaultSpeech2TextProviderId=config_values.defaultSpeech2TextProviderId or "",
        defaultSpeech2TextModelId=config_values.defaultSpeech2TextModelId or "",
        defaultTTSProviderId=config_values.defaultTTSProviderId or "",
        defaultTTSModelId=config_values.defaultTTSModelId or "",
        llmProviders=llmProviders,
        textEmbeddingProviders=textEmbeddingProviders,
        rerankProviders=rerankProviders,
        toImageProviders=toImageProviders,
        speech2TextProviders=speech2TextProviders,
        ttsProviders=ttsProviders,
    )
