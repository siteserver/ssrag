from enum import Enum


class StorageType(str, Enum):
    """
    Enumeration of file storage types.
    """

    ALIYUN_OSS = "aliyun-oss"
    OPENDAL = "opendal"
