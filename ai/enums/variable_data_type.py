from enum import Enum


class VariableDataType(str, Enum):
    """
    Enumeration of variable data types.
    """

    STRING = "String"
    INTEGER = "Integer"
    NUMBER = "Number"
    BOOLEAN = "Boolean"
    OBJECT = "Object"
    ARRAY_STRING = "ArrayString"
    ARRAY_INTEGER = "ArrayInteger"
    ARRAY_NUMBER = "ArrayNumber"
    ARRAY_BOOLEAN = "ArrayBoolean"
    ARRAY_OBJECT = "ArrayObject"

    @classmethod
    def get_display_names(cls) -> dict[str, str]:
        return {
            cls.STRING: "字符串",
            cls.INTEGER: "整数",
            cls.NUMBER: "数字",
            cls.BOOLEAN: "布尔值",
            cls.OBJECT: "对象",
            cls.ARRAY_STRING: "字符串数组",
            cls.ARRAY_INTEGER: "整数数组",
            cls.ARRAY_NUMBER: "数字数组",
            cls.ARRAY_BOOLEAN: "布尔值数组",
            cls.ARRAY_OBJECT: "对象数组",
        }

    @property
    def display_name(self) -> str:
        """
        Returns the display name of the variable data type.
        """
        return self.get_display_names().get(self.value, self.value)
