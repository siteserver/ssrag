from ..vector_factory_base import VectorFactoryBase
from .pgvector import PGVector
from .pgvector_config import PGVectorConfig
from configs import app_configs
from extensions import TextEmbeddingModel
from ..vector_base import VectorBase


class PGVectorFactory(VectorFactoryBase):
    def create(
        self,
        text_embedding: TextEmbeddingModel,
    ) -> VectorBase:
        return PGVector(
            text_embedding=text_embedding,
            config=PGVectorConfig(
                host=app_configs.PGVECTOR_HOST or "localhost",
                port=app_configs.PGVECTOR_PORT,
                user=app_configs.PGVECTOR_USER or "postgres",
                password=app_configs.PGVECTOR_PASSWORD or "",
                database=app_configs.PGVECTOR_DATABASE or "postgres",
                schema=app_configs.PGVECTOR_SCHEMA or "",
            ),
        )
