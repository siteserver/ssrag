from repositories import config_repository
from dto import ModelCredentials
from enums import ModelType
from repositories import model_provider_repository


def llm_manager_get_default_model_credentials(
    model_type: ModelType,
) -> ModelCredentials | None:
    configs = config_repository.get_values()

    return model_provider_repository.get_model_credentials(
        model_type,
        configs.defaultLLMProviderId or "",
        configs.defaultLLMModelId or "",
    )
