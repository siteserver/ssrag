import os
import posixpath
from ..storage_base import StorageBase
from ..storage_type import StorageType
from utils import file_utils


class LocalStorage(StorageBase):
    def __init__(self):
        super().__init__()
        self.prefix = "./wwwroot/"

    def get_type(self) -> str:
        return StorageType.LOCAL

    def save_stream(self, filename: str, data: bytes, public: bool) -> int:
        with open(self._combine_path(filename), "wb") as f:
            f.write(data)
        return len(data)

    def save_text(self, filename: str, text: str, public: bool) -> int:
        return self.save_stream(filename, text.encode("utf-8"), public)

    def get_file_url(self, filename: str, signed: bool) -> str:
        return file_utils.combine_url("/api/ai/", filename)

    def load_bytes(self, filename: str) -> bytes:
        with open(self._combine_path(filename), "rb") as f:
            return f.read()

    def load_text(self, filename: str) -> str:
        with open(self._combine_path(filename), "r", encoding="utf-8") as f:
            return f.read()

    def exists(self, filename):
        return os.path.exists(self._combine_path(filename))

    def delete(self, filename):
        os.remove(self._combine_path(filename))
        
    def delete_by_prefix(self, prefix):
        for file in os.listdir(self._combine_path(prefix)):
            if file.endswith("/"):
                continue
            os.remove(self._combine_path(file))

    def scan(
        self, path: str, files: bool = True, directories: bool = False
    ) -> list[str]:
        result = []
        if files:
            for file in os.listdir(self._combine_path(path)):
                if file.endswith("/"):
                    continue
                result.append(file)
        elif directories:
            for file in os.listdir(self._combine_path(path)):
                if not file.endswith("/"):
                    continue
                result.append(file)
        return result

    def _combine_path(self, filename: str) -> str:
        return posixpath.join(self.prefix, filename) if self.prefix else filename
