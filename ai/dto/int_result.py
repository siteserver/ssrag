from pydantic import BaseModel


class IntResult(BaseModel):
    value: int
