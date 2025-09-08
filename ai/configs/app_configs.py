from pydantic_settings import SettingsConfigDict
from .common import CommonConfig
from .vectors import VectorsConfig
from .storages import StoragesConfig


class AppConfigs(
    # Common configs
    CommonConfig,
    # Storages configs
    StoragesConfig,
    # Vectors configs
    VectorsConfig,
):
    model_config = SettingsConfigDict(
        # read from dotenv format config file
        env_file=".env",
        env_file_encoding="utf-8",
        # ignore extra attributes
        extra="ignore",
    )
