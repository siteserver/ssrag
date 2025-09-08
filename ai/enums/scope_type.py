from enum import Enum


class ScopeType(str, Enum):
    SELF = "Self"
    CHILDREN = "Children"
    SELF_AND_CHILDREN = "SelfAndChildren"
    DESCENDANT = "Descendant"
    ALL = "All"
