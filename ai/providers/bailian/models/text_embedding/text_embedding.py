from extensions import TextEmbeddingBase
from dto import ModelCredentials
import httpx


class TextEmbedding(TextEmbeddingBase):
    def __init__(self, model_credentials: ModelCredentials):
        self.api_key = model_credentials.credentials["dashscope_api_key"]
        self.use_international_endpoint = model_credentials.credentials.get("use_international_endpoint", "false")
        self.endpoint = self._get_endpoint()
        self.model_id = model_credentials.modelId
        
    def _get_endpoint(self):
        if self.use_international_endpoint == "true":
            return "https://dashscope-intl.aliyuncs.com/api/v1/services/embeddings/text-embedding/text-embedding"
        return "https://dashscope.aliyuncs.com/api/v1/services/embeddings/text-embedding/text-embedding"

    def embedding(self, inputs: list[str]) -> list[list[float]]:
        embeddings = []
        payload = {
            "model": self.model_id,
            "input": {
              "texts": inputs,
            },
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
