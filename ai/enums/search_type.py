from enum import Enum


class SearchType(str, Enum):
    SEMANTIC = "Semantic"
    FULL_TEXT = "FullText"
    HYBRID = "Hybrid"
