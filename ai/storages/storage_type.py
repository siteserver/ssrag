from enum import Enum


class StorageType(str, Enum):
    """
    Enumeration of storage types.
    """

    LOCAL = "local"
    ALIYUN_OSS = "aliyun-oss"
