from extensions import LLMBase
from dto import Message, ModelCredentials
from fastapi.responses import StreamingResponse
import httpx


class LLM(LLMBase):
    def __init__(self, model_credentials: ModelCredentials):
        self.api_key = model_credentials.credentials["api_key"]
        self.endpoint_url = model_credentials.credentials.get("endpoint_url")
        self.endpoint = self.endpoint_url + "/chat/completions" if self.endpoint_url else "https://api.deepseek.com/chat/completions"

    def chat(self, messages: list[Message], payload: dict | None = None) -> str:
        payload_submit = {
            "model": "deepseek-chat",
            "messages": [
                {"role": msg.role, "content": msg.content} for msg in messages
            ],
            "stream": False,
            "response_format": {"type": "text"},
        }
        if payload is not None:
            payload_submit.update(payload)

        headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json",
        }

        # 直接输出响应
        with httpx.Client() as client:
            response = client.post(self.endpoint, headers=headers, json=payload_submit)
            response.raise_for_status()
            return response.text

    def json(self, messages: list[Message], payload: dict | None = None) -> dict:
        payload_submit = {
            "model": "deepseek-chat",
            "messages": [
                {"role": msg.role, "content": msg.content} for msg in messages
            ],
            "stream": False,
            "response_format": {"type": "json_object"},
        }
        if payload is not None:
            payload_submit.update(payload)

        headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json",
        }

        with httpx.Client() as client:
            response = client.post(self.endpoint, json=payload_submit, headers=headers)
            response.raise_for_status()
            return response.json()

    def chat_stream(
        self, messages: list[Message], thinking: bool, payload: dict | None = None
    ) -> StreamingResponse:
        payload_submit = {
            "model": "deepseek-reasoner" if thinking else "deepseek-chat",
            "messages": [
                {"role": msg.role, "content": msg.content} for msg in messages
            ],
            "stream": True,
            "response_format": {"type": "text"},
        }
        if payload is not None:
            payload_submit.update(payload)

        headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json",
        }

        # 创建流式响应
        def generate_stream():
            with httpx.stream(
                "POST", url=self.endpoint, headers=headers, json=payload_submit, timeout=600
            ) as response:
                for chunk in response.iter_lines():
                    yield f"{chunk}\n\n"

        # 返回流式响应
        return StreamingResponse(
            generate_stream(),
            media_type="text/event-stream",
            headers={"Cache-Control": "no-cache"},
        )
