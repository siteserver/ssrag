import json
from urllib.parse import urljoin
from extensions import LLMBase
from dto import Message, ModelCredentials
from fastapi.responses import StreamingResponse
import httpx


class LLM(LLMBase):
    def __init__(self, model_credentials: ModelCredentials):
        self.endpoint = model_credentials.credentials["base_url"]
        if not self.endpoint.endswith("/"):
            self.endpoint += "/"
        self.model_id = model_credentials.modelId
        
    def chat(self, messages: list[Message], payload: dict | None = None) -> str:
        payload_submit = {
            "model": self.model_id,
            "messages": [
                {"role": msg.role, "content": msg.content} for msg in messages
            ],
            "stream": False,
        }
        # if "think" in payload and payload["think"] is not None:
        #     payload_submit["think"] = bool(payload["think"])
        #     del payload["think"]
        if payload is not None:
            payload_submit.update(payload)

        headers = {
            "Content-Type": "application/json",
        }
        api_url = urljoin(self.endpoint, "api/chat")

        # 直接输出响应
        with httpx.Client() as client:
            response = client.post(api_url, headers=headers, json=payload_submit)
            response.raise_for_status()
            return response.text
          
          
    def json(self, messages: list[Message], payload: dict | None = None) -> dict:
        payload_submit = {
            "model": self.model_id,
            "messages": [
                {"role": msg.role, "content": msg.content} for msg in messages
            ],
            "stream": False,
            "enable_thinking": False,
            "response_format": {"type": "json_object"},
        }
        if payload is not None:
            payload_submit.update(payload)

        headers = {
            "Content-Type": "application/json",
        }
        api_url = urljoin(self.endpoint, "api/chat")

        with httpx.Client() as client:
            response = client.post(api_url, json=payload_submit, headers=headers)
            response.raise_for_status()
            return response.json()

    def chat_stream(
        self, messages: list[Message], thinking: bool, payload: dict | None = None
    ) -> StreamingResponse:
        # 使用 httpx 访问 ollama 并返回 StreamingResponse
        payload_submit = {
            "model": self.model_id,
            "messages": [
                {"role": msg.role, "content": msg.content} for msg in messages
            ],
            "stream": True,
            "think": thinking,
        }
        if payload is not None:
            payload_submit.update(payload)

        headers = {
            "Content-Type": "application/json",
        }
        api_url = urljoin(self.endpoint, "api/chat")
        
        # 创建流式响应
        def generate_stream():
            with httpx.stream(
                "POST", url=api_url, headers=headers, json=payload_submit, timeout=600
            ) as response:
                for chunk in response.iter_lines():
                    # yield f"{chunk}\n\n"
                    # 将Ollama返回格式转换为OpenAI返回格式
                    if not chunk:
                        continue
                    try:
                        data = json.loads(chunk)
                        # Ollama流式返回格式通常为{"message": {"content": "..."}}
                        # 转换为OpenAI格式：{"choices":[{"delta":{"content": "..."}}]}
                        if "message" in data:
                            content = ""
                            reasoning_content = ""
                            if "content" in data["message"]:
                                content = data["message"]["content"]
                            if "thinking" in data["message"]:
                                reasoning_content = data["message"]["thinking"]
                            if content == "</think>":
                                content = ""
                                reasoning_content = ""
                            openai_chunk = {
                                "choices": [
                                    {
                                        "delta": {
                                            "content": content,
                                            "reasoning_content": reasoning_content
                                        }
                                    }
                                ]
                            }
                            yield f"data: {json.dumps(openai_chunk, ensure_ascii=False)}\n\n"
                        else:
                            # 其他情况直接原样输出
                            yield f"data: {chunk}\n\n"
                    except Exception:
                        # 解析失败时，原样输出
                        yield f"data: {chunk}\n\n"
                    
        # 返回流式响应
        return StreamingResponse(
            generate_stream(),
            media_type="text/event-stream",
            headers={"Cache-Control": "no-cache"},
        )
