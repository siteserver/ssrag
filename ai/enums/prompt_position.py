from enum import Enum


class PromptPosition(str, Enum):
    HOT = "Hot"
    FUNCTION = "Function"
    INPUT = "Input"
