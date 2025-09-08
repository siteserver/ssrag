from pydantic import BaseModel
from models import CeleryTask


class GetResult(BaseModel):
    tasks: list[CeleryTask]
    pendingCount: int
    documentCount: int
