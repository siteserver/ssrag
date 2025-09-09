from sqlmodel import Session, select
from utils.db_utils import engine
from models import Config, ConfigValues
from enums import ProviderType
from configs import constants
from .model_provider_repository import ModelProviderRepository
from .model_repository import ModelRepository
from utils import encrypt_utils
from models import ModelProvider, Model
from enums import ModelType
from utils import string_utils
import uuid
from utils import yaml_utils
from vectors import Vector


class ConfigRepository:

    def _get_config_values(self) -> ConfigValues:
        config_values = None
        with Session(engine) as session:
            statement = select(Config)
            config = session.exec(statement).first()
        if config:
            config_values = config.config_values

        if not config_values:
            config_values = ConfigValues(
                init=False,
                defaultLLMProviderId=ProviderType.SSRAG,
                defaultLLMModelId=None,
                defaultTextEmbeddingProviderId=ProviderType.SSRAG,
                defaultTextEmbeddingModelId=None,
            )

        return config_values

    def get_values(self) -> ConfigValues:
        config_values = self._get_config_values()
        if config_values.init:
            return config_values

        model_provider_repository = ModelProviderRepository()
        model_repository = ModelRepository()

        provider = model_provider_repository.get_by_provider_id(ProviderType.SSRAG)
        if not provider:
            provider = ModelProvider(
                uuid=str(uuid.uuid4()),
                providerId=ProviderType.SSRAG,
                providerName="SSRAG",
                iconUrl="ssrag_square.svg",
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
        self.update_values(config_values)
        
        vector = Vector()
        vector.create_collection()

        return config_values

    def update_values(self, values: ConfigValues):
        with Session(engine) as session:
            statement = select(Config)
            config = session.exec(statement).first()
            if config:
                config.config_values = values
                # config.configs = values.model_dump_json()
                session.add(config)
                session.commit()
