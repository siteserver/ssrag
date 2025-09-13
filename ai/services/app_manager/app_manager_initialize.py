from repositories import model_provider_repository, model_repository, config_repository
from configs import constants
import uuid
from vectors import Vector
from models import ModelProvider, Model, ConfigValues
from enums import ProviderType, ModelType
from utils import encrypt_utils, string_utils, yaml_utils
from configs import app_configs


def app_manager_initialize(config_values: ConfigValues) -> ConfigValues:
    if not app_configs.TENANT_ID:
        return config_values
    
    provider = model_provider_repository.get_by_provider_id(ProviderType.SSRAG)
    if not provider:
        provider = ModelProvider(
            uuid=str(uuid.uuid4()),
            providerId=ProviderType.SSRAG,
            providerName="SSRAG",
            iconUrl="ssrag_square.png",
            description="内置默认选项，将硅基流动模型服务内置在产品中，无需申请与填写 API Key 即可轻松使用。",
        )
        encrypted_credentials, salt = encrypt_utils.jwt_encrypt({})
        provider.credentials = encrypted_credentials
        provider.credentialsSalt = salt
        model_provider_repository.create(provider)

        models = model_repository.get_all_by_provider_id(ProviderType.SSRAG)
        llm_model = None
        for model in models:
            if model.modelType == ModelType.LLM:
                llm_model = model
                break

        defaultLLMModelId = None
        if not llm_model:
            file_model = yaml_utils.yaml_to_json(constants.DEFAULT_LLM_MODEL_PATH)
            defaultLLMModelId = file_model["model"]
            model_properties = file_model["model_properties"]

            model = Model(
                uuid=str(uuid.uuid4()),
                providerId=ProviderType.SSRAG,
                modelId=defaultLLMModelId,
                modelType=ModelType.LLM,
                extendValues=string_utils.to_json_str(model_properties),
            )
            model_repository.create(model)

        text_embedding_model = None
        for model in models:
            if model.modelType == ModelType.TEXT_EMBEDDING:
                text_embedding_model = model
                break

        defaultTextEmbeddingModelId = None
        if not text_embedding_model:
            file_model = yaml_utils.yaml_to_json(
                constants.DEFAULT_TEXT_EMBEDDING_MODEL_PATH
            )
            defaultTextEmbeddingModelId = file_model["model"]
            model_properties = file_model["model_properties"]

            model_repository.create(
                Model(
                    uuid=str(uuid.uuid4()),
                    providerId=ProviderType.SSRAG,
                    modelId=defaultTextEmbeddingModelId,
                    modelType=ModelType.TEXT_EMBEDDING,
                    extendValues=string_utils.to_json_str(model_properties),
                )
            )

    if (
        not config_values.defaultLLMProviderId
        or not config_values.defaultLLMModelId
    ):
        config_values.defaultLLMProviderId = ProviderType.SSRAG
        config_values.defaultLLMModelId = defaultLLMModelId
    if (
        not config_values.defaultTextEmbeddingProviderId
        or not config_values.defaultTextEmbeddingModelId
    ):
        config_values.defaultTextEmbeddingProviderId = ProviderType.SSRAG
        config_values.defaultTextEmbeddingModelId = defaultTextEmbeddingModelId
    
    config_values.init = True
    config_repository.update_values(config_values)
    
    vector = Vector()
    vector.create_collection()
    
    return config_values
