from pydantic import BaseModel, model_validator


class WeaviateConfig(BaseModel):
    url: str
    api_key: str
    batch_size: int = 100

    @model_validator(mode="before")
    @classmethod
    def validate_config(cls, values: dict) -> dict:
        if not values["url"]:
            raise ValueError("config WEAVIATE_URL is required")
        return values
