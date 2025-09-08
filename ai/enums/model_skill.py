from enum import Enum


class ModelSkill(str, Enum):
    """
    Enumeration of model skills.
    """

    REASONING = "reasoning"
    VLM = "vlm"
    TOOLS = "tools"
    FIM = "fim"
    MATH = "math"
    CODER = "coder"

    @classmethod
    def get_display_names(cls) -> dict[str, str]:
        return {
            cls.REASONING: "推理",
            cls.VLM: "视觉",
            cls.TOOLS: "工具",
            cls.FIM: "补全",
            cls.MATH: "数学",
            cls.CODER: "代码",
        }

    @property
    def display_name(self) -> str:
        """
        Returns the display name of the model skill.
        """
        return self.get_display_names().get(self.value, self.value)
