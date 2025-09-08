from enum import Enum


class DataType(str, Enum):
    """
    Enumeration of data types.
    """

    BOOLEAN = "Boolean"
    DATETIME = "DateTime"
    DECIMAL = "Decimal"
    INTEGER = "Integer"
    TEXT = "Text"
    VARCHAR = "VarChar"

    @property
    def display_name(self) -> str:
        """
        Returns the display name of the data type.
        """
        display_names = {
            self.BOOLEAN: "布尔值",
            self.DATETIME: "日期",
            self.DECIMAL: "小数",
            self.INTEGER: "整数",
            self.TEXT: "备注",
            self.VARCHAR: "字符串",
        }
        return display_names.get(self, self.value)
