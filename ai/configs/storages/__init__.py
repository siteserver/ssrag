from .aliyun_oss_config import AliyunOSSConfig
from pydantic import Field
from typing import Literal
from pydantic_settings import BaseSettings


class StorageConfig(BaseSettings):
    STORAGE_TYPE: Literal[
        "opendal",
        "s3",
        "aliyun-oss",
        "azure-blob",
        "baidu-obs",
        "google-storage",
        "huawei-obs",
        "oci-storage",
        "tencent-cos",
        "volcengine-tos",
        "supabase",
        "local",
    ] = Field(
        description="Type of storage to use."
        " Options: 'opendal', '(deprecated) local', 's3', 'aliyun-oss', 'azure-blob', 'baidu-obs', 'google-storage', "
        "'huawei-obs', 'oci-storage', 'tencent-cos', 'volcengine-tos', 'supabase'. Default is 'opendal'.",
        default="opendal",
    )


class StoragesConfig(
    StorageConfig,
    # place the configs in alphabet order
    AliyunOSSConfig,
):
    pass
