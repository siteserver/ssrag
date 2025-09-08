from pydantic import BaseModel
from enums import AuthType


class Auth(BaseModel):
    role: list[AuthType] | None = None
    adminName: str | None = None
    userName: str | None = None
    isPersistent: bool | None = None
    token: str | None = None
