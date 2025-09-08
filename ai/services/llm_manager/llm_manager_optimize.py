import requests
import json
from dto import Message
from extensions import LLMFactory
from enums import ModelType
from repositories import config_repository
from repositories import model_provider_repository


def llm_manager_optimize(original_texts: list[str]) -> list[str]:
    messages: list[Message] = []
    messages.append(
        Message(
            role="system",
            content="""
    你是一位专业的内容优化专家。你的任务是将简短的输入文本优化成更加专业、生动的表达。

    **具体要求：**
    1. 输入文本会包含多个短句，每个短句都需要单独优化
    2. 每个优化后的文本应该：
      - 更加专业和具体
      - 保持原意
      - 字数控制在30字以内
      - 使用优美的语言表达
    3. 必须以指定的JSON格式返回结果，格式如下：
    {
        "optimized_texts": [
            "优化后的文本1",
            "优化后的文本2"
        ]
    }

    **示例：**
    输入：人工智能可以提高工作效率
    输出：
    {
        "optimized_texts": [
            "人工智能技术通过自动化流程和智能决策支持，显著提升工作效率"
        ]
    }

    请严格按照上述JSON格式返回结果，不要添加任何额外的内容。
    """,
        )
    )
    messages.append(
        Message(
            role="user",
            content="请优化以下语句：\n" + "\n".join(original_texts),
        )
    )

    payload = {
        "parameters": {
            "temperature": 0.3,  # 降低温度以获得更稳定的输出
            "top_p": 0.9,
            "max_tokens": 200,  # 增加最大token数
            "presence_penalty": 0.6,  # 增加多样性
            "frequency_penalty": 0.6,  # 减少重复
        },
    }

    configs = config_repository.get_values()
    if configs.defaultLLMProviderId is None or configs.defaultLLMModelId is None:
        raise Exception("未设置默认优化模型！")

    model_credentials = model_provider_repository.get_model_credentials(
        ModelType.LLM, configs.defaultLLMProviderId, configs.defaultLLMModelId
    )
    if model_credentials is None:
        raise Exception("模型不存在！")

    optimized_texts = []
    llm = LLMFactory.create(model_credentials)
    result = llm.json(messages, payload)
    if "choices" in result:
        outputs = [choice["message"]["content"] for choice in result["choices"]]
    for output in outputs:
        try:
            content = json.loads(output)
            if "optimized_texts" in content:
                optimized_texts.extend(content["optimized_texts"])
        except json.JSONDecodeError:
            raise Exception(f"Failed to parse JSON from output: {output}")

    return optimized_texts


# import requests
# import json
# from dto import Message
# from utils import page_utils
# from services import llm_manager
# from enums import ModelType


# def llm_manager_optimize(original_texts: list[str]) -> list[str]:
#     credentials = llm_manager.get_default_model_credentials(ModelType.LLM)
#     if credentials is None:
#         raise Exception("默认模型不存在！")

#     endpoint = "https://api.siliconflow.cn/v1"
#     apiKey = credentials.credentials["api_key"]
#     url = page_utils.combile(endpoint, "/chat/completions")

#     messages: list[Message] = []
#     messages.append(
#         Message(
#             role="system",
#             content="""
#     你是一位专业的内容优化专家。你的任务是将简短的输入文本优化成更加专业、生动的表达。

#     **具体要求：**
#     1. 输入文本会包含多个短句，每个短句都需要单独优化
#     2. 每个优化后的文本应该：
#       - 更加专业和具体
#       - 保持原意
#       - 字数控制在30字以内
#       - 使用优美的语言表达
#     3. 必须以指定的JSON格式返回结果，格式如下：
#     {
#         "optimized_texts": [
#             "优化后的文本1",
#             "优化后的文本2"
#         ]
#     }

#     **示例：**
#     输入：人工智能可以提高工作效率
#     输出：
#     {
#         "optimized_texts": [
#             "人工智能技术通过自动化流程和智能决策支持，显著提升工作效率"
#         ]
#     }

#     请严格按照上述JSON格式返回结果，不要添加任何额外的内容。
#     """,
#         )
#     )
#     messages.append(
#         Message(
#             role="user",
#             content="请优化以下语句：\n" + "\n".join(original_texts),
#         )
#     )

#     payload = {
#         "model": credentials.modelId,
#         "messages": [{"role": msg.role, "content": msg.content} for msg in messages],
#         "stream": False,
#         "enable_thinking": False,
#         "parameters": {
#             "temperature": 0.3,  # 降低温度以获得更稳定的输出
#             "top_p": 0.9,
#             "max_tokens": 200,  # 增加最大token数
#             "presence_penalty": 0.6,  # 增加多样性
#             "frequency_penalty": 0.6,  # 减少重复
#         },
#         "response_format": {"type": "json_object"},
#     }
#     headers = {
#         "Authorization": f"Bearer {apiKey}",
#         "Content-Type": "application/json",
#     }

#     response = requests.request("POST", url, json=payload, headers=headers)
#     optimized_texts = []

#     if response.status_code == 200:
#         result = response.json()
#         if "choices" in result:
#             outputs = [choice["message"]["content"] for choice in result["choices"]]
#         for output in outputs:
#             try:
#                 content = json.loads(output)
#                 if "optimized_texts" in content:
#                     optimized_texts.extend(content["optimized_texts"])
#             except json.JSONDecodeError:
#                 raise Exception(f"Failed to parse JSON from output: {output}")
#     else:
#         raise Exception(f"调用失败，状态码：{response.status_code}，{response.text}")

#     return optimized_texts
