from pydantic import BaseModel


class ModelCredentials(BaseModel):
    providerId: str
    modelId: str
    credentials: dict
