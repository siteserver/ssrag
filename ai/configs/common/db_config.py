from typing import Optional
from pydantic import Field, PositiveInt
from pydantic_settings import BaseSettings


class DbConfig(BaseSettings):
    DB_TYPE: Optional[str] = Field(
        description="Type of database server.",
        default=None,
    )

    DB_HOST: Optional[str] = Field(
        description="Hostname or IP address of the database server.",
        default=None,
    )

    DB_PORT: PositiveInt = Field(
        description="Port number for database connection.",
        default=5432,
    )

    DB_USER: Optional[str] = Field(
        description="Username for database authentication.",
        default=None,
    )

    DB_PASSWORD: Optional[str] = Field(
        description="Password for database authentication.",
        default=None,
    )

    DB_DATABASE: Optional[str] = Field(
        description="Name of the database to connect to.",
        default=None,
    )

    DB_SCHEMA: Optional[str] = Field(
        description="Name of the database schema to connect to.",
        default=None,
    )
