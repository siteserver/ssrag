import requests

url = "https://api.siliconflow.cn/v1/chat/completions"

payload = {
    "model": "deepseek-ai/DeepSeek-R1",
    "messages": [
        {
            "role": "user",
            "content": "请计算 3 + 5 的结果。",
        }
    ],
    "stream": True,
    "enable_thinking": True,
    "response_format": {"type": "text"},
    "tools": [
        {
            "name": "add_numbers",
            "description": "加法工具",
            "parameters": {
                "type": "object",
                "properties": {"a": {"type": "int"}, "b": {"type": "int"}},
                "required": ["a", "b"],
            },
        },
        {"name": "get_current_time", "description": "获取当前时间", "parameters": {}},
    ],
}
headers = {
    "Authorization": "Bearer sk-xxx",
    "Content-Type": "application/json",
}

response = requests.request("POST", url, json=payload, headers=headers)

print(response.text)
