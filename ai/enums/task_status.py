from enum import Enum


class TaskStatus(str, Enum):
    """
    Enumeration of task status.
    """

    PENDING = "PENDING"
    PROGRESS = "PROGRESS"
    SUCCESS = "SUCCESS"
    FAILURE = "FAILURE"
