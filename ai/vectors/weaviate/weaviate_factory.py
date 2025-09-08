from ..vector_factory_base import VectorFactoryBase
from .weaviate import WeaviateVector
from .weaviate_config import WeaviateConfig
from configs import app_configs
from extensions import TextEmbeddingModel
from ..vector_base import VectorBase


class WeaviateFactory(VectorFactoryBase):
    def create(
        self,
        text_embedding: TextEmbeddingModel,
    ) -> VectorBase:
        return WeaviateVector(
            text_embedding=text_embedding,
            config=WeaviateConfig(
                url=app_configs.WEAVIATE_URL or "",
                api_key=app_configs.WEAVIATE_API_KEY or "",
                batch_size=app_configs.WEAVIATE_BATCH_SIZE,
            ),
        )
