from .local_storage import LocalStorage
from ..storage_base import StorageBase
from ..storage_factory_base import StorageFactoryBase


class LocalStorageFactory(StorageFactoryBase):
    def create(self) -> StorageBase:
        return LocalStorage()
