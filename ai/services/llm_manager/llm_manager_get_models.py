from enums import ModelType
from dto import Model
from repositories import model_repository, model_provider_repository, config_repository


def llm_manager_get_models(model_type: ModelType) -> tuple[list[Model], Model | None]:
    defaultModel = None
    models: list[Model] = []
    providers = {}
    for model in model_repository.get_all(model_type):
        provider = providers.get(model.providerId)
        if provider is None:
            provider = model_provider_repository.get_by_provider_id(model.providerId)
            if provider is None:
                continue
            providers[model.providerId] = provider
        models.append(
            Model(
                providerId=model.providerId,
                providerName=provider.providerName,
                iconUrl=provider.iconUrl,
                modelId=model.modelId,
            )
        )

    configs = config_repository.get_values()
    if configs is not None:
        defaultModel = next(
            (
                model
                for model in models
                if model.providerId == configs.defaultLLMProviderId
                and model.modelId == configs.defaultLLMModelId
            ),
            None,
        )

    return models, defaultModel
