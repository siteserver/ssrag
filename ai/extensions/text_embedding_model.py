from repositories import config_repository, model_repository
from configs import constants
from repositories import model_provider_repository
from .text_embedding_factory import TextEmbeddingFactory
from enums import ModelType


class TextEmbeddingModel:
    def __init__(self):
        self.providerId = ""
        self.modelId = ""
        self.dimension = 0
        self.isReady = False

        configs = config_repository.get_values()
        model = model_repository.get(
            provider_id=configs.defaultTextEmbeddingProviderId,
            model_id=configs.defaultTextEmbeddingModelId,
        )
        if model and constants.MODEL_PROPERTY_VECTOR_DIMENSION in model.properties:
            self.providerId = model.providerId
            self.modelId = model.modelId
            self.dimension = int(
                model.properties[constants.MODEL_PROPERTY_VECTOR_DIMENSION]
            )
            self.isReady = True

    def embedding_inputs(self, inputs: list[str]) -> list[list[float]]:
        if not self.isReady:
            raise Exception("未设置默认文本嵌入模型！")

        model_credentials = model_provider_repository.get_model_credentials(
            ModelType.TEXT_EMBEDDING,
            self.providerId,
            self.modelId,
        )
        if model_credentials is None:
            raise Exception("模型不存在！")

        text_embedding = TextEmbeddingFactory.create(model_credentials)
        return text_embedding.embedding(inputs)

    def embedding_input(self, input: str) -> list[float]:
        return self.embedding_inputs([input])[0]
