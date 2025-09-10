from typing import Dict, Any
import uuid
from hashlib import sha256
import json
from jinja2 import Template
import jieba
from pydantic import BaseModel


def get_short_uuid() -> str:
    return uuid.uuid4().hex[:16]


def generate_hash(text: str) -> str:
    if text is None or text == "":
        return ""
    hash_text = str(text)
    return sha256(hash_text.encode()).hexdigest()


def to_int(text) -> int:
    if text is None:
        return 0
    if isinstance(text, int):
        return text
    try:
        return int(text)
    except:
        return 0


def to_float(text: str | None) -> float:
    if text is None:
        return 0
    try:
        return float(text)
    except:
        return 0


def to_bool(text: str | None) -> bool:
    if text is None:
        return False
    try:
        return text.lower() == "true" or text.lower() == "1"
    except:
        return False


def default_serializer(obj):
    if hasattr(obj, "isoformat"):
        return obj.isoformat()
    raise TypeError(f"Object of type {type(obj).__name__} is not JSON serializable")


def to_json_str(
    obj: BaseModel | list[BaseModel] | object | dict | None,
) -> str:
    if obj is None:
        return "{}"
    if isinstance(obj, BaseModel):
        return json.dumps(obj.model_dump(), default=default_serializer)
    elif isinstance(obj, list) and len(obj) > 0 and isinstance(obj[0], BaseModel):
        # 处理BaseModel对象列表
        return json.dumps(
            [item.model_dump() for item in obj], default=default_serializer
        )

    return json.dumps(obj, ensure_ascii=False)


def to_json_object(json_value: str | dict | None) -> Dict[str, Any]:
    if json_value is None:
        return {}
    if isinstance(json_value, dict):
        return json_value
    try:
        return json.loads(json_value)
    except:
        return {}


def parse_jinja2(template: str, data: dict) -> str:
    jinja_template = Template(template)
    return jinja_template.render(data)


def jieba_cut_to_list(text: str | None) -> list[str]:
    if not text:
        return []
    return [cut for cut in jieba.cut_for_search(text) if len(cut) > 1]


def jieba_cut_to_str(text: str | None, separator: str = " ") -> str:
    if not text:
        return ""
    return separator.join(jieba_cut_to_list(text))


def split(text: str | None = None) -> list[str]:
    if not text:
        return []
    return text.split(",")


def split_to_int(text: str | None = None) -> list[int]:
    if not text:
        return []
    return [int(item) for item in text.split(",")]


def join(list: list[str] | list[int] | None = None, separator: str = ",") -> str:
    if not list:
        return ""
    return separator.join(str(item) for item in list)


def try_parse_json_object(text: str | None):
    if not text:
        return None
    try:
        if text.startswith("[") or text.startswith("{"):
            return json.loads(text)
        else:
            return text
    except:
        return text


def contains(collection: str, value: str | None) -> bool:
    if not collection or not value:
        return False
    list = collection.split(",")
    return value in list


def trim_path(path: str) -> str:
    if path.startswith("./"):
        path = path[1:]
    return path.replace("\\", "/")


def extract_markdown_code(text: str, code_type: str) -> str:
    if f"```{code_type}" in text:
        _, code = text.split(f"```{code_type}")
        return code.split("```")[0].strip()
    return text


def extract_provider_model_id(provider_model_id: str) -> tuple[str, str]:
    if not provider_model_id or ":" not in provider_model_id:
        return "", ""
    provider_id, model_id = provider_model_id.split(":")
    if not provider_id or not model_id:
        return "", ""
    return provider_id, model_id


def get_default_provider_model_id(defaultLLMProviderId: str | None, defaultLLMModelId: str | None) -> str:
    if not defaultLLMProviderId or not defaultLLMModelId:
        return ""
    return f"{defaultLLMProviderId}:{defaultLLMModelId}"