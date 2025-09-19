from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Text
from utils import string_utils
from configs import table_names
from datetime import datetime
from dataclasses import dataclass, asdict
import json


@dataclass
class ConfigValues:
    init: bool = False
    defaultLLMProviderId: str | None = None
    defaultLLMModelId: str | None = None
    defaultTextEmbeddingProviderId: str | None = None
    defaultTextEmbeddingModelId: str | None = None
    defaultRerankProviderId: str | None = None
    defaultRerankModelId: str | None = None
    defaultToImageProviderId: str | None = None
    defaultToImageModelId: str | None = None
    defaultSpeech2TextProviderId: str | None = None
    defaultSpeech2TextModelId: str | None = None
    defaultTTSProviderId: str | None = None
    defaultTTSModelId: str | None = None


class Config(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_CONFIG  # type: ignore
    id: int = Field(
        sa_column=Column("Id", Integer, primary_key=True),
        default=None,
    )
    uuid: str = Field(
        sa_column=Column("Uuid", String(50)),
        default=None,
    )
    createdDate: datetime = Field(
        sa_column=Column("CreatedDate", DateTime, default=datetime.now),
        default=None,
    )
    lastModifiedDate: datetime = Field(
        sa_column=Column("LastModifiedDate", DateTime, default=datetime.now),
        default=None,
    )
    databaseVersion: str = Field(
        sa_column=Column("DatabaseVersion", String(500)),
        default=None,
    )
    updateDate: datetime = Field(
        sa_column=Column("UpdateDate", DateTime),
        default=None,
    )
    configs: str = Field(
        sa_column=Column("Configs", Text),
        default=None,
    )

    @property
    def config_values(self) -> ConfigValues:
        data = string_utils.to_json_object(self.configs)
        return ConfigValues(**data)

    @config_values.setter
    def config_values(self, value: ConfigValues):
        self.configs = json.dumps(asdict(value))
