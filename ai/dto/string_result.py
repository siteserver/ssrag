from pydantic import BaseModel


class StringResult(BaseModel):
    value: str
