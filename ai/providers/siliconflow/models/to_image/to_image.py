from extensions import ToImageBase
from dto import ModelCredentials
import httpx


class ToImage(ToImageBase):
    def __init__(self, model_credentials: ModelCredentials):
        self.endpoint = "https://api.siliconflow.cn/v1/images/generations"
        self.api_key = model_credentials.credentials["api_key"]
        self.model_id = model_credentials.modelId

    def generate(self, prompt: str, image_size: str, batch_size: int) -> list[str]:
        payload_submit = {
            "model": self.model_id,
            "prompt": prompt,
            "image_size": image_size,
            "batch_size": batch_size,
            "num_inference_steps": 20,
            "guidance_scale": 7.5,
        }

        headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json",
        }

        json = ""
        with httpx.Client() as client:
            response = client.post(self.endpoint, json=payload_submit, headers=headers, timeout=600)
            response.raise_for_status()
            json = response.json()
        
        image_urls = []
        if json:
          if "images" in json and isinstance(json["images"], list):
              for img in json["images"]:
                  if "url" in img:
                      image_urls.append(img["url"])
        
        return image_urls