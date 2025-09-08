from pydantic import BaseModel


class Cascade(BaseModel):
    value: int
    label: str
    children: list["Cascade"] | None = None
    disableCheckbox: bool | None = None
