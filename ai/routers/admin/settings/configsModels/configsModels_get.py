from repositories import config_repository, model_provider_repository, model_repository
from .__base import GetResult
from utils import encrypt_utils, string_utils
from services import AppManager
from configs import app_configs


async def configsModels_get() -> GetResult:
    db_providers = model_provider_repository.get_all()
    providers = []
    for db_provider in db_providers:
        credentials = encrypt_utils.jwt_decrypt(
            db_provider.credentials, db_provider.credentialsSalt
        )
        db_models = model_repository.get_all_by_provider_id(db_provider.providerId)
        models = []
        for db_model in db_models:
            model = {
                "id": db_model.id,
                "providerId": db_model.providerId,
                "modelType": db_model.modelType,
                "modelId": db_model.modelId,
                "skills": db_model.skill_list,
                "extendValues": string_utils.to_json_object(db_model.extendValues),
            }
            models.append(model)
        providers.append(
            {
                "providerId": db_provider.providerId,
                "providerName": db_provider.providerName,
                "iconUrl": db_provider.iconUrl,
                "description": db_provider.description,
                "credentials": credentials,
                "models": models,
            }
        )

    config_values = config_repository.get_values()
    if not config_values.init:
        config_values = AppManager.initialize(config_values)

    return GetResult(providers=providers, configValues=config_values, tenantId=app_configs.TENANT_ID)
