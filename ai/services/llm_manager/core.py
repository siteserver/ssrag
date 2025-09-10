from models import FlowNodeSettings
from dto import RunVariable
from dto import Intent
from fastapi.responses import StreamingResponse
from enums import ModelType
from dto import Model
from dto import ModelCredentials
from repositories import config_repository, model_provider_repository, model_repository
from configs import constants


class LLMManager:
    def __init__(self):
        pass

    # def get_image_description(self, save_path: str, ext_name: str) -> str | list[str]:
    #     from .llm_manager_get_image_description import llm_manager_get_image_description

    #     return llm_manager_get_image_description(save_path, ext_name)

    def run_invoke(self, settings: FlowNodeSettings, inVariables: list[RunVariable]):
        from .llm_manager_run_invoke import llm_manager_run_invoke

        return llm_manager_run_invoke(settings, inVariables)

    def run_stream(
        self, settings: FlowNodeSettings, thinking: bool, inVariables: list[RunVariable]
    ):
        from .llm_manager_run_stream import llm_manager_run_stream

        return llm_manager_run_stream(settings, thinking, inVariables)

    def chat(
        self,
        providerId: str,
        modelId: str,
        userMessage: str,
        systemMessage: str | None = None,
        enable_thinking: bool = True,
    ) -> StreamingResponse:
        from .llm_manager_chat import llm_manager_chat

        return llm_manager_chat(
            providerId, modelId, userMessage, systemMessage, enable_thinking
        )

    def intent(
        self, providerId: str, modelId: str, intentions: list[str], text: str
    ) -> Intent | None:
        from .llm_manager_intent import llm_manager_intent

        return llm_manager_intent(providerId, modelId, intentions, text)

    def optimize(self, original_texts: list[str]) -> list[str]:
        from .llm_manager_optimize import llm_manager_optimize

        return llm_manager_optimize(original_texts)

    def get_models(self, model_type: ModelType) -> tuple[list[Model], Model | None]:
        from .llm_manager_get_models import llm_manager_get_models

        return llm_manager_get_models(model_type)

    def get_model_credentials(
        self, model_type: ModelType, providerId: str, modelId: str
    ) -> ModelCredentials | None:
        return model_provider_repository.get_model_credentials(
            model_type, providerId, modelId
        )

    def get_default_model_credentials(
        self, model_type: ModelType
    ) -> ModelCredentials | None:
        from .llm_manager_get_default_model_credentials import (
            llm_manager_get_default_model_credentials,
        )

        return llm_manager_get_default_model_credentials(model_type)

    def get_vector_dimension(self) -> int:
        configs = config_repository.get_values()
        if (
            configs.defaultTextEmbeddingModelId is None
            or configs.defaultTextEmbeddingProviderId is None
        ):
            raise Exception("未设置默认文本嵌入模型！")

        model = model_repository.get(
            provider_id=configs.defaultTextEmbeddingProviderId,
            model_id=configs.defaultTextEmbeddingModelId,
        )
        if (
            model is None
            or constants.MODEL_PROPERTY_VECTOR_DIMENSION not in model.properties
        ):
            raise Exception("模型不存在！")

        return int(model.properties[constants.MODEL_PROPERTY_VECTOR_DIMENSION])
