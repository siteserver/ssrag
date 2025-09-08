from enum import Enum


class VariableType(str, Enum):
    """
    Enumeration of variable types.
    """

    INPUT = "Input"
    OUTPUT = "Output"
    GLOBAL = "Global"

    @classmethod
    def get_display_names(cls) -> dict[str, str]:
        return {
            cls.INPUT: "输入",
            cls.OUTPUT: "输出",
            cls.GLOBAL: "全局",
        }

    @property
    def display_name(self) -> str:
        """
        Returns the display name of the variable type.
        """
        return self.get_display_names().get(self.value, self.value)
