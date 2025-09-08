from pydantic import BaseModel


class BoolResult(BaseModel):
    value: bool
