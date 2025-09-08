from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Boolean, Text
from configs import table_names
from datetime import datetime
import json
from utils import string_utils


class Channel(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_CHANNEL  # type: ignore
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
    channelName: str = Field(
        sa_column=Column("ChannelName", String(500)),
        default=None,
    )
    siteId: int = Field(
        sa_column=Column("SiteId", Integer),
        default=None,
    )
    parentId: int = Field(
        sa_column=Column("ParentId", Integer),
        default=None,
    )
    parentsPath: str = Field(
        sa_column=Column("ParentsPath", String(500)),
        default=None,
    )
    parentsCount: int = Field(
        sa_column=Column("ParentsCount", Integer),
        default=None,
    )
    childrenCount: int = Field(
        sa_column=Column("ChildrenCount", Integer),
        default=None,
    )
    indexName: str = Field(
        sa_column=Column("IndexName", String(500)),
        default=None,
    )
    taxis: int = Field(
        sa_column=Column("Taxis", Integer),
        default=None,
    )
    imageUrl: str = Field(
        sa_column=Column("ImageUrl", String(500)),
        default=None,
    )
    content: str = Field(
        sa_column=Column("Content", Text),
        default=None,
    )
    knowledge: bool = Field(
        sa_column=Column("Knowledge", Boolean),
        default=None,
    )
    keywords: str = Field(
        sa_column=Column("Keywords", String(500)),
        default=None,
    )
    description: str = Field(
        sa_column=Column("Description", String(500)),
        default=None,
    )
    extendValues: str = Field(
        sa_column=Column("ExtendValues", Text),
        default=None,
    )

    @classmethod
    def load(cls, json_str: str | None) -> "Channel":
        if json_str is None:
            return cls()
        try:
            return cls.model_validate(json.loads(json_str))
        except Exception as e:
            return cls()

    def json(self) -> str:
        return string_utils.to_json_str(self)
