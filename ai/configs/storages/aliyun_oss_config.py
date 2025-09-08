from typing import Optional

from pydantic import Field
from pydantic_settings import BaseSettings


class AliyunOSSConfig(BaseSettings):
    """
    Configuration settings for Aliyun Object Storage Service (OSS)
    """

    ALIYUN_OSS_ENDPOINT: Optional[str] = Field(
        description="URL of the Aliyun OSS endpoint for your chosen region",
        default=None,
    )

    ALIYUN_OSS_REGION: Optional[str] = Field(
        description="Aliyun OSS region where your bucket is located (e.g., 'oss-cn-hangzhou')",
        default=None,
    )

    ALIYUN_OSS_ACCESS_KEY: Optional[str] = Field(
        description="Access key ID for authenticating with Aliyun OSS",
        default=None,
    )

    ALIYUN_OSS_SECRET_KEY: Optional[str] = Field(
        description="Secret access key for authenticating with Aliyun OSS",
        default=None,
    )

    ALIYUN_OSS_BUCKET_NAME: Optional[str] = Field(
        description="Name of the Aliyun OSS bucket to store and retrieve objects",
        default=None,
    )

    ALIYUN_OSS_BUCKET_URL: Optional[str] = Field(
        description="URL of the Aliyun OSS bucket to access, if not provided, the endpoint will be used",
        default=None,
    )

    ALIYUN_OSS_PREFIX: Optional[str] = Field(
        description="Base path within the bucket to store objects (e.g., 'my-app-data/')",
        default=None,
    )
