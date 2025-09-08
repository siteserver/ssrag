import yaml
from typing import Any


def yaml_to_json(yaml_file_path: str) -> dict[str, Any]:
    with open(yaml_file_path, "r", encoding="utf-8") as yaml_file:
        yaml_content = yaml.safe_load(yaml_file)

    return yaml_content


def yaml_string_to_json(yaml_string: str) -> dict[str, Any]:
    yaml_content = yaml.safe_load(yaml_string)
    return yaml_content
