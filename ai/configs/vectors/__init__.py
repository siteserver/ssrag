from .pgvector_config import PGVectorConfig
from .weaviate_config import WeaviateConfig
from pydantic import Field
from typing import Literal
from pydantic_settings import BaseSettings


class VectorStoreConfig(BaseSettings):
    VECTOR_STORE: Literal[
        "pgvector",
        "weaviate",
    ] = Field(
        description="Type of vector store to use."
        " Options: 'pgvector', 'weaviate'. Default is 'pgvector'.",
        default="pgvector",
    )


class VectorsConfig(
    VectorStoreConfig,
    # place the configs in alphabet order
    PGVectorConfig,
    WeaviateConfig,
):
    pass
