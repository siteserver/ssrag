from .__base import SubmitProviderRequest
from repositories import model_provider_repository, model_repository, config_repository
from utils import encrypt_utils
from dto import BoolResult
import uuid
from models import ModelProvider, Model
from enums import ProviderType, ModelType
from utils import string_utils
from configs import constants
from utils import yaml_utils


async def configsModels_submit_provider(request: SubmitProviderRequest) -> BoolResult:
    provider = model_provider_repository.get_by_provider_id(request.providerId)
    if provider:
        model_provider_repository.update_credentials(
            request.providerId, request.credentials
        )
    else:
        provider = ModelProvider(
            uuid=str(uuid.uuid4()),
            providerId=request.providerId,
            providerName=request.providerName,
            iconUrl=request.iconUrl,
            description=request.description,
        )
        encrypted_credentials, salt = encrypt_utils.jwt_encrypt(request.credentials)
        provider.credentials = encrypted_credentials
        provider.credentialsSalt = salt
        model_provider_repository.create(provider)

        if request.providerId == ProviderType.SSRAG:
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

            config_values = config_repository.get_values()
            changed = False
            if (
                not config_values.defaultLLMProviderId
                or not config_values.defaultLLMModelId
            ):
                config_values.defaultLLMProviderId = ProviderType.SSRAG
                config_values.defaultLLMModelId = defaultLLMModelId
                changed = True
            if (
                not config_values.defaultTextEmbeddingProviderId
                or not config_values.defaultTextEmbeddingModelId
            ):
                config_values.defaultTextEmbeddingProviderId = ProviderType.SSRAG
                config_values.defaultTextEmbeddingModelId = defaultTextEmbeddingModelId
                changed = True

            if changed:
                config_repository.update_values(config_values)

    return BoolResult(value=True)
