from typing import Any, Dict, TypeVar, Type
from enum import Enum

T = TypeVar("T", bound=Enum)


def to_int(int_str: str | int | None, default_value: int = 0) -> int:
    if not int_str:
        return default_value

    try:
        i = int(int_str)
        return i if i >= 0 else default_value
    except ValueError:
        return default_value


def to_value(value: Any, default: Any) -> Any:
    if value is None:
        return default
    if isinstance(value, type(default)):
        return value
    return default


def to_enum(value: Any, enum_type: Type[T], default: T) -> T:
    if value is None:
        return default
    try:
        return enum_type(value)
    except ValueError:
        return default


def get_value(dict: Dict[str, Any], key: str, default: Any) -> Any:
    value = dict.get(key)
    return to_value(value, default)
