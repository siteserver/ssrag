from enum import Enum


class ModelType(str, Enum):
    """
    Enumeration of model types.
    """

    LLM = "llm"
    TEXT_EMBEDDING = "text-embedding"
    RERANK = "rerank"
    TO_IMAGE = "to-image"
    SPEECH2TEXT = "speech2text"
    TTS = "tts"
    MODERATION = "moderation"

    @classmethod
    def get_display_names(cls) -> dict[str, str]:
        return {
            cls.LLM: "对话",
            cls.TEXT_EMBEDDING: "文本嵌入",
            cls.RERANK: "重排序",
            cls.TO_IMAGE: "图片生成",
            cls.SPEECH2TEXT: "语音转文本",
            cls.TTS: "文本转语音",
            cls.MODERATION: "内容审核",
        }

    @property
    def display_name(self) -> str:
        """
        Returns the display name of the model type.
        """
        return self.get_display_names().get(self.value, self.value)
