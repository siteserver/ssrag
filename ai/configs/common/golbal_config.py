from pydantic import Field
from pydantic_settings import BaseSettings


class GlobalConfig(BaseSettings):
    """
    Configuration settings for global
    """

    TENANT_ID: str = Field(
        description="Tenant ID for the application",
        default="",
    )

    SECURITY_KEY: str = Field(
        description="Security key for the application",
        default="",
    )
