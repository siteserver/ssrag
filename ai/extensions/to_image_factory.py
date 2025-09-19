from .to_image_base import ToImageBase
from dto import ModelCredentials
from enums import ProviderType


class ToImageFactory:
    @staticmethod
    def create(model_credentials: ModelCredentials) -> ToImageBase:
        if model_credentials.providerId == ProviderType.SILICONFLOW:
            from providers.siliconflow.models.to_image.to_image import ToImage as SiliconflowToImage

            return SiliconflowToImage(model_credentials)
        else:
            raise ValueError("未知模型提供者")
