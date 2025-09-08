from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Text
from datetime import datetime
from configs import table_names


class Segment(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_SEGMENT  # type: ignore
    id: str = Field(
        sa_column=Column("Id", String(50), primary_key=True),
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
    contentId: int = Field(
        sa_column=Column("ContentId", Integer),
        default=None,
    )
    documentId: int = Field(
        sa_column=Column("DocumentId", Integer),
        default=None,
    )
    taxis: int = Field(
        sa_column=Column("Taxis", Integer),
        default=None,
    )
    text: str = Field(
        sa_column=Column("Text", Text),
        default=None,
    )
    textHash: str = Field(
        sa_column=Column("TextHash", String(500)),
        default=None,
    )
