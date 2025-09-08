from abc import ABC, abstractmethod


class StorageBase(ABC):
    def __init__(self):
        pass

    @abstractmethod
    def get_type(self) -> str:
        raise NotImplementedError

    @abstractmethod
    def save_stream(self, filename: str, data: bytes, public: bool) -> int:
        raise NotImplementedError

    @abstractmethod
    def save_text(self, filename: str, text: str, public: bool) -> int:
        raise NotImplementedError

    @abstractmethod
    def get_file_url(self, filename: str, signed: bool) -> str:
        raise NotImplementedError

    @abstractmethod
    def load_bytes(self, filename: str) -> bytes:
        raise NotImplementedError

    @abstractmethod
    def load_text(self, filename: str) -> str:
        raise NotImplementedError

    @abstractmethod
    def exists(self, filename):
        raise NotImplementedError

    @abstractmethod
    def delete(self, filename):
        raise NotImplementedError

    @abstractmethod
    def scan(
        self, path: str, files: bool = True, directories: bool = False
    ) -> list[str]:
        raise NotImplementedError
