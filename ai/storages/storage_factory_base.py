from abc import ABC, abstractmethod
from .storage_base import StorageBase


class StorageFactoryBase(ABC):
    @abstractmethod
    def create(self) -> StorageBase:
        raise NotImplementedError
