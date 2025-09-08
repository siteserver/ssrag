from .aliyun_oss_storage import AliyunOssStorage
from .aliyun_oss_config import AliyunOssConfig
from configs import app_configs
from ..storage_base import StorageBase
from ..storage_factory_base import StorageFactoryBase


class AliyunOssStorageFactory(StorageFactoryBase):
    def create(self) -> StorageBase:
        return AliyunOssStorage(
            config=AliyunOssConfig(
                endpoint=app_configs.ALIYUN_OSS_ENDPOINT or "",
                region=app_configs.ALIYUN_OSS_REGION or "",
                access_key=app_configs.ALIYUN_OSS_ACCESS_KEY or "",
                secret_key=app_configs.ALIYUN_OSS_SECRET_KEY or "",
                bucket_name=app_configs.ALIYUN_OSS_BUCKET_NAME or "",
                bucket_url=app_configs.ALIYUN_OSS_BUCKET_URL or "",
                prefix=app_configs.ALIYUN_OSS_PREFIX or "",
            ),
        )
