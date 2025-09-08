from dto import Message, Intent
from utils import string_utils
from extensions import LLMFactory
from enums import ModelType
from repositories import model_provider_repository


def llm_manager_intent(
    providerId: str, modelId: str, intentions: list[str], text: str
) -> Intent | None:
    intentOptions = string_utils.join(intentions)

    messages: list[Message] = []

    messages.append(
        Message(
            role="system",
            content=f"""
    **任务描述：**  
    请根据用户输入的内容，自动判断用户意图并返回一个符合以下格式的 JSON 对象：

    **JSON 格式要求：**
    ```json
    {{
      "intention": "用户意图",
      "reason": "判断的依据"
    }}
    ```

    **字段说明：**
    1. `intention`：表示用户意图，必须是以下选项之一：`{intentOptions}`。
    2. `reason`：表示形成此判断的依据。  

    **示例 1：**  
    用户输入：“iPhone 15 的价格是多少？”  
    期望输出：
    ```json
    {{
      "intention": "与产品价格、费用、折扣等相关的问题",
      "reason": "用户询问iPhone 15 的费用是多少，此问题围绕价格提问，所以属于价格问题"
    }}
    ```

    **示例 2：**  
    用户输入：“如何设置 iPhone 15 的屏幕亮度？”  
    期望输出：
    ```json
    {{
      "intention": "与产品功能、操作方法、故障排除等相关的问题",
      "reason": "用户询问的是如何设置iPhone 15的屏幕亮度，属于产品功能操作方法相关的问题"
    }}
    ```

    **请根据以下用户输入生成 JSON：**  

    请严格按照上述JSON格式返回结果，不要添加任何额外的内容。
    """,
        )
    )
    messages.append(
        Message(
            role="user",
            content=text,
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

    try:
        model_credentials = model_provider_repository.get_model_credentials(
            ModelType.LLM, providerId, modelId
        )
        if model_credentials is None:
            raise Exception("模型不存在！")

        llm = LLMFactory.create(model_credentials)
        jsonObject = llm.json(messages, payload)
        content = jsonObject["choices"][0]["message"]["content"]
        intention = content["intention"]
        reason = content["reason"]
        if intention not in intentions:
            return None
        return Intent(intention, reason)

    except Exception as e:
        return None
