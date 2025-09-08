from pydantic import BaseModel


class RunVariable(BaseModel):
    name: str
    value: str | dict | list | None
