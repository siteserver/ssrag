from extensions import TextEmbeddingBase
from dto import ModelCredentials
import httpx


class TextEmbedding(TextEmbeddingBase):
    def __init__(self, model_credentials: ModelCredentials):
        self.endpoint = "https://api.siliconflow.cn/v1/embeddings"
        self.api_key = model_credentials.credentials["api_key"]
        self.model_id = model_credentials.modelId

    def embedding(self, inputs: list[str]) -> list[list[float]]:
        embeddings = []
        payload = {
            "model": self.model_id,
            "input": inputs,
        }
        headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json",
        }

        response = httpx.post(self.endpoint, json=payload, headers=headers)
        if response.status_code == 200:
            result = response.json()
            for item in result["data"]:
                embeddings.append(item["embedding"])
            return embeddings
        else:
            raise Exception(
                f"Embedding API 请求失败: {response.status_code} - {response.text}"
            )
