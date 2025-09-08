from pydantic import BaseModel, model_validator


class PGVectorConfig(BaseModel):
    host: str
    port: int
    user: str
    password: str
    database: str
    schema: str

    @model_validator(mode="before")
    @classmethod
    def validate_config(cls, values: dict) -> dict:
        if not values["host"]:
            raise ValueError("config PGVECTOR_HOST is required")
        if not values["port"]:
            raise ValueError("config PGVECTOR_PORT is required")
        if not values["user"]:
            raise ValueError("config PGVECTOR_USER is required")
        if not values["password"]:
            raise ValueError("config PGVECTOR_PASSWORD is required")
        if not values["database"]:
            raise ValueError("config PGVECTOR_DATABASE is required")
        return values
