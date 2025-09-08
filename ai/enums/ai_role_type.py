from enum import Enum


class AiRoleType(str, Enum):
    """
    Enumeration of ai role types.
    """

    SYSTEM = "system"
    USER = "user"
    ASSISTANT = "assistant"
