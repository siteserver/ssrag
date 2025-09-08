from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime
from datetime import datetime
from configs import table_names
import json
from utils import string_utils


class Prompt(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_PROMPT  # type: ignore
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
    siteId: int = Field(
        sa_column=Column("SiteId", Integer),
        default=None,
    )
    position: str = Field(
        sa_column=Column("Position", String(500)),
        default=None,
    )
    title: str = Field(
        sa_column=Column("Title", String(500)),
        default=None,
    )
    iconUrl: str = Field(
        sa_column=Column("IconUrl", String(500)),
        default=None,
    )
    text: str = Field(
        sa_column=Column("Text", String(500)),
        default=None,
    )
    taxis: int = Field(
        sa_column=Column("Taxis", Integer),
        default=None,
    )

    @classmethod
    def load_prompts(cls, json_str: str | None) -> list["Prompt"]:
        if json_str is None:
            return []
        return json.loads(json_str, object_hook=cls.model_validate)

    @classmethod
    def json_prompts(cls, prompts: list["Prompt"] | None) -> str:
        if prompts is None:
            return "[]"
        return string_utils.to_json_str(prompts)
