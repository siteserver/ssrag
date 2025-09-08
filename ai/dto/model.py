from pydantic import BaseModel


class Model(BaseModel):
    providerId: str
    providerName: str
    iconUrl: str
    modelId: str
