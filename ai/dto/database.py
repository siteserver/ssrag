from abc import ABC, abstractmethod
from enums import DataType


class Database(ABC):
    @abstractmethod
    def get_column_string(self, type: DataType, attributeName: str) -> str:
        """
        将数据类型转换为数据库列字符串
        """
        pass
