from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Boolean, Text
from datetime import datetime
from configs import table_names


class ChatGroup(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_CHAT_GROUP  # type: ignore
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
    sessionId: str = Field(
        sa_column=Column("SessionId", String(500)),
        default=None,
    )
    title: str = Field(
        sa_column=Column("Title", String(500)),
        default=None,
    )
    adminName: str = Field(
        sa_column=Column("AdminName", String(500)),
        default=None,
    )
    userName: str = Field(
        sa_column=Column("UserName", String(500)),
        default=None,
    )
    isDeleted: bool = Field(
        sa_column=Column("IsDeleted", Boolean),
        default=False,
    )
