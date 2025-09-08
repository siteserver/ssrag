from pydantic import Field, PositiveInt
from pydantic_settings import BaseSettings


class CeleryConfig(BaseSettings):
    """
    Configuration settings for Celery
    """

    CELERY_BROKER_DB: PositiveInt = Field(
        description="Celery broker database number",
        default=1,
    )

    CELERY_BACKEND_DB: PositiveInt = Field(
        description="Celery backend database number",
        default=2,
    )
