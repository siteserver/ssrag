from enum import Enum


class NodeType(str, Enum):
    """
    Enumeration of node types.
    """

    # Chatflow
    START = "Start"
    INPUT = "Input"
    OUTPUT = "Output"
    DATASET = "Dataset"
    LLM = "LLM"
    INTENT = "Intent"
    ASK = "Ask"
    WEBSEARCH = "WebSearch"
    TEXT = "Text"
    SQL = "Sql"
    HTTP = "Http"

    @classmethod
    def get_display_names(cls) -> dict[str, str]:
        return {
            cls.START: "开始",
            cls.INPUT: "输入",
            cls.OUTPUT: "输出",
            cls.DATASET: "知识库",
            cls.LLM: "大模型",
            cls.INTENT: "意图识别",
            cls.ASK: "提问",
            cls.WEBSEARCH: "联网搜索",
            cls.TEXT: "文本处理",
            cls.SQL: "SQL 查询",
            cls.HTTP: "HTTP 调用",
        }

    @property
    def display_name(self) -> str:
        """
        Returns the display name of the node type.
        """
        return self.get_display_names().get(self.value, self.value)
