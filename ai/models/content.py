from dataclasses import dataclass
from sqlalchemy import Column, Integer, String, DateTime, Boolean, Text
from sqlmodel import Field, SQLModel
from utils import string_utils
from datetime import datetime
import json
from dataclasses import asdict
from typing import Optional


@dataclass
class ContentValues:
    separators: list[str] | None = None
    chunkSize: int | None = None
    chunkOverlap: int | None = None
    isChunkReplaces: bool | None = None
    isChunkDeletes: bool | None = None


class Content(SQLModel):
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
    channelId: int = Field(
        sa_column=Column("ChannelId", Integer),
        default=None,
    )
    checked: bool = Field(
        sa_column=Column("Checked", Boolean),
        default=None,
    )
    title: Optional[str] = Field(
        sa_column=Column("Title", String(500)),
        default=None,
    )
    imageUrl: Optional[str] = Field(
        sa_column=Column("ImageUrl", String(500)),
        default=None,
    )
    fileUrl: Optional[str] = Field(
        sa_column=Column("FileUrl", String(500)),
        default=None,
    )
    body: Optional[str] = Field(
        sa_column=Column("Body", Text),
        default=None,
    )
    knowledge: bool = Field(
        sa_column=Column("Knowledge", Boolean),
        default=None,
    )
    extendValues: Optional[str] = Field(
        sa_column=Column("ExtendValues", Text),
        default=None,
    )

    @property
    def content_values(self) -> ContentValues:
        data = string_utils.to_json_object(self.extendValues)
        return ContentValues(**data)

    @content_values.setter
    def content_values(self, value: ContentValues):
        self.extendValues = json.dumps(asdict(value))
