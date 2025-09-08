import requests
from pydantic import BaseModel
import json


class Message(BaseModel):
    role: str
    content: str


class Intent:
    def __init__(self, intention: str, reason: str):
        self.intention = intention
        self.reason = reason


url = "https://api.siliconflow.cn/v1/chat/completions"

intentOptions = [
    "售前咨询",
    "售后咨询",
    "产品咨询",
    "价格咨询",
    "功能咨询",
    "操作咨询",
    "使用咨询",
    "故障咨询",
    "其他",
]

messages: list[Message] = []
messages.append(
    Message(
        role="system",
        content="""
**任务描述：**  
请根据用户输入的内容，自动判断用户意图并返回一个符合以下格式的 JSON 对象：

**JSON 格式要求：**
```json
{
  "intention": "用户意图",
  "reason": "判断的依据"
}
```

**字段说明：**
1. `intention`：表示用户意图，必须是以下选项之一：`["售前咨询","售后咨询","产品咨询","价格咨询","功能咨询","操作咨询","使用咨询","故障咨询","其他"]`。
2. `reason`：表示形成此判断的依据。  

**示例 1：**  
用户输入：“iPhone 15 的价格是多少？”  
期望输出：
```json
{
  "intention": "与产品价格、费用、折扣等相关的问题",
  "reason": "用户询问iPhone 15 的费用是多少，此问题围绕价格提问，所以属于价格问题"
}
```

**示例 2：**  
用户输入：“如何设置 iPhone 15 的屏幕亮度？”  
期望输出：
```json
{
  "intention": "与产品功能、操作方法、故障排除等相关的问题",
  "reason": "用户询问的是如何设置iPhone 15的屏幕亮度，属于产品功能操作方法相关的问题"
}
```

**请根据以下用户输入生成 JSON：**  

请严格按照上述JSON格式返回结果，不要添加任何额外的内容。
""",
    )
)
messages.append(
    Message(
        role="user",
        content="帮我订一张明天从北京到上海的机票",
    )
)

payload = {
    "model": "Qwen/Qwen3-30B-A3B",
    "messages": [{"role": msg.role, "content": msg.content} for msg in messages],
    "stream": False,
    "enable_thinking": False,
    "parameters": {
        "temperature": 0.3,  # 降低温度以获得更稳定的输出
        "top_p": 0.9,
        "max_tokens": 200,  # 增加最大token数
        "presence_penalty": 0.6,  # 增加多样性
        "frequency_penalty": 0.6,  # 减少重复
    },
    "response_format": {"type": "json_object"},
}
headers = {
    "Authorization": "Bearer sk-xxx",
    "Content-Type": "application/json",
}


try:
    response = requests.request("POST", url, json=payload, headers=headers)
    response.raise_for_status()
    content = response.json()["choices"][0]["message"]["content"]
    jsonObject = json.loads(content)
    print(jsonObject)
    intention = jsonObject["intention"]
    reason = jsonObject["reason"]
    print(intention, reason)
    intent = Intent(intention, reason)
    print(intent.reason)

except Exception as e:
    print(e)
