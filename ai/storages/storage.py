from configs import app_configs

from .storage_base import StorageBase
from .storage_factory_base import StorageFactoryBase
from .storage_type import StorageType


class Storage:
    def __init__(self):
        self._storage = self._create()

    def _create(self) -> StorageBase:
        storage_type = app_configs.STORAGE_TYPE

        if not storage_type:
            raise ValueError("Storage must be specified.")

        storage_factory_cls = self.get_storage_factory(storage_type)
        return storage_factory_cls().create()

    @staticmethod
    def get_storage_factory(storage_type: str) -> type[StorageFactoryBase]:
        match storage_type:
            case StorageType.LOCAL:
                from .local import LocalStorageFactory

                return LocalStorageFactory
            case StorageType.ALIYUN_OSS:
                from .aliyun_oss import AliyunOssStorageFactory

                return AliyunOssStorageFactory
            case _:
                raise ValueError(f"Storage {storage_type} is not supported.")

    def get_type(self) -> str:
        return self._storage.get_type()

    def save_stream(self, filename: str, data: bytes, public: bool = False):
        return self._storage.save_stream(filename, data, public)

    def save_text(self, filename: str, text: str, public: bool = False):
        return self._storage.save_text(filename, text, public)

    def get_file_url(self, filename: str, signed: bool = True) -> str:
        return self._storage.get_file_url(filename, signed)

    def load_bytes(self, filename: str) -> bytes:
        return self._storage.load_bytes(filename)

    def load_text(self, filename: str) -> str:
        return self._storage.load_text(filename)

    def exists(self, filename):
        return self._storage.exists(filename)

    def delete(self, filename):
        return self._storage.delete(filename)
      
    def delete_by_prefix(self, prefix):
        return self._storage.delete_by_prefix(prefix)

    def scan(
        self, path: str, files: bool = True, directories: bool = False
    ) -> list[str]:
        return self._storage.scan(path, files, directories)
