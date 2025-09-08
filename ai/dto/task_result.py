from pydantic import BaseModel


class TaskResult(BaseModel):
    state: str
    result: dict
    detail: str
