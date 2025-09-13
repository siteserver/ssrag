from extensions.llm_base import LLMBase
from dto import ModelCredentials
from enums import ProviderType
from configs import app_configs


class LLMFactory:
    @staticmethod
    def create(model_credentials: ModelCredentials) -> LLMBase:
        if model_credentials.providerId == ProviderType.SILICONFLOW:
            #     LLM = importlib.import_module("providers.siliconflow.models.llm.llm").LLM
            #     return LLM(model_credentials)
            from providers.siliconflow.models.llm.llm import LLM as SiliconflowLLM

            return SiliconflowLLM(model_credentials)
        elif model_credentials.providerId == ProviderType.SSRAG and app_configs.TENANT_ID:
            from providers.ssrag.models.llm.llm import LLM as SSRAGLLM

            return SSRAGLLM(model_credentials)
        else:
            raise ValueError("未知模型提供者")
