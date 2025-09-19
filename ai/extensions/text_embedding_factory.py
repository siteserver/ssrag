from .text_embedding_base import TextEmbeddingBase
from dto import ModelCredentials
from enums import ProviderType
from configs import app_configs


class TextEmbeddingFactory:
    @staticmethod
    def create(model_credentials: ModelCredentials) -> TextEmbeddingBase:
        if model_credentials.providerId == ProviderType.SILICONFLOW:
            from providers.siliconflow.models.text_embedding.text_embedding import (
                TextEmbedding as SiliconflowTextEmbedding,
            )

            return SiliconflowTextEmbedding(model_credentials)
        elif model_credentials.providerId == ProviderType.BAILIAN:
            from providers.bailian.models.text_embedding.text_embedding import (
                TextEmbedding as BailianTextEmbedding,
            )

            return BailianTextEmbedding(model_credentials)
        elif model_credentials.providerId == ProviderType.SSRAG and app_configs.TENANT_ID:
            from providers.ssrag.models.text_embedding.text_embedding import (
                TextEmbedding as SSRAGTextEmbedding,
            )

            return SSRAGTextEmbedding(model_credentials)
        else:
            raise ValueError("未知模型提供者")
