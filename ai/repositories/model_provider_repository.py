from sqlmodel import Session, select, delete, asc
from models import ModelProvider
from utils.db_utils import engine
from utils import encrypt_utils
from dto import ModelCredentials
from enums import ModelType
from repositories import config_repository


class ModelProviderRepository:
    def get_by_provider_id(self, provider_id: str | None) -> ModelProvider | None:
        if not provider_id:
            return None

        statement = select(ModelProvider).where(ModelProvider.providerId == provider_id)
        with Session(engine) as session:
            model_provider = session.exec(statement).first()
        return model_provider

    def get_credentials(self, provider_id: str) -> dict:
        statement = select(
            ModelProvider.credentials, ModelProvider.credentialsSalt
        ).where(ModelProvider.providerId == provider_id)
        with Session(engine) as session:
            model_provider = session.exec(statement).first()
            if model_provider:
                credentials = model_provider[0]
                credentials_salt = model_provider[1]
                if credentials and credentials_salt:
                    return encrypt_utils.jwt_decrypt(credentials, credentials_salt)
                else:
                    return {}
            else:
                return {}

    def update_credentials(self, provider_id: str, credentials: dict) -> None:
        encrypted_credentials, salt = encrypt_utils.jwt_encrypt(credentials)
        provider = self.get_by_provider_id(provider_id)
        if provider:
            provider.credentials = encrypted_credentials
            provider.credentialsSalt = salt
            with Session(engine) as session:
                session.add(provider)
                session.commit()
                session.refresh(provider)

    def create(self, provider: ModelProvider) -> ModelProvider:
        with Session(engine) as session:
            session.add(provider)
            session.commit()
            session.refresh(provider)
        return provider

    def delete(self, provider_id: str):
        with Session(engine) as session:
            stmt = delete(ModelProvider).where(ModelProvider.providerId == provider_id)  # type: ignore
            session.exec(stmt)  # type: ignore
            session.commit()

    def get_all(self) -> list[ModelProvider]:
        statement = select(ModelProvider).order_by(asc(ModelProvider.id))
        with Session(engine) as session:
            providers = session.exec(statement).all()
        return list(providers)

    def get_model_credentials(
        self, model_type: ModelType, providerId: str, modelId: str
    ) -> ModelCredentials | None:
        provider = self.get_by_provider_id(providerId)
        if provider is None:
            configs = config_repository.get_values()
            if model_type == ModelType.LLM:
                providerId = configs.defaultLLMProviderId or ""
                modelId = configs.defaultLLMModelId or ""
            elif model_type == ModelType.TEXT_EMBEDDING:
                providerId = configs.defaultTextEmbeddingProviderId or ""
                modelId = configs.defaultTextEmbeddingModelId or ""
            elif model_type == ModelType.RERANK:
                providerId = configs.defaultRerankProviderId or ""
                modelId = configs.defaultRerankModelId or ""
            elif model_type == ModelType.SPEECH2TEXT:
                providerId = configs.defaultSpeech2TextProviderId or ""
                modelId = configs.defaultSpeech2TextModelId or ""
            elif model_type == ModelType.TTS:
                providerId = configs.defaultTTSProviderId or ""
                modelId = configs.defaultTTSModelId or ""

            provider = self.get_by_provider_id(providerId)

        if provider is None:
            raise Exception("模型不存在！")

        credentials = encrypt_utils.jwt_decrypt(
            provider.credentials, provider.credentialsSalt
        )

        return ModelCredentials(
            providerId=providerId,
            modelId=modelId,
            credentials=credentials,
        )
