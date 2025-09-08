from typing import Optional

from pydantic import Field, NonNegativeInt, PositiveInt
from pydantic_settings import BaseSettings


class RedisConfig(BaseSettings):
    """
    Configuration settings for Redis connection
    """

    REDIS_HOST: str = Field(
        description="Hostname or IP address of the Redis server",
        default="localhost",
    )

    REDIS_PORT: PositiveInt = Field(
        description="Port number on which the Redis server is listening",
        default=6379,
    )

    REDIS_PASSWORD: Optional[str] = Field(
        description="Password for Redis authentication (if required)",
        default=None,
    )

    REDIS_SSL: bool = Field(
        description="Enable SSL/TLS for the Redis connection",
        default=False,
    )

    REDIS_DB: NonNegativeInt = Field(
        description="Redis database number to use (0-15)",
        default=0,
    )

    REDIS_PREFIX: Optional[str] = Field(
        description="Redis key prefix",
        default="",
    )
