from pydantic import BaseModel, model_validator


class AliyunOssConfig(BaseModel):
    endpoint: str
    region: str
    access_key: str
    secret_key: str
    bucket_name: str
    bucket_url: str
    prefix: str

    @model_validator(mode="before")
    @classmethod
    def validate_config(cls, values: dict) -> dict:
        if not values["endpoint"]:
            raise ValueError("config ALIYUN_OSS_ENDPOINT is required")
        if not values["region"]:
            raise ValueError("config ALIYUN_OSS_REGION is required")
        if not values["access_key"]:
            raise ValueError("config ALIYUN_OSS_ACCESS_KEY is required")
        if not values["secret_key"]:
            raise ValueError("config ALIYUN_OSS_SECRET_KEY is required")
        if not values["bucket_name"]:
            raise ValueError("config ALIYUN_OSS_BUCKET_NAME is required")
        return values
