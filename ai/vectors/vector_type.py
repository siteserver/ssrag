from enum import Enum


class VectorType(str, Enum):
    """
    Enumeration of vector store types.
    """

    PGVECTOR = "pgvector"
    WEAVIATE = "weaviate"
