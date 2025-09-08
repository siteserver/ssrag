from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Text, Boolean
from datetime import datetime
from configs import table_names


class Message(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_MESSAGE  # type: ignore
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
    userName: str = Field(
        sa_column=Column("UserName", String(500)),
        default=None,
    )
    messageType: str = Field(
        sa_column=Column("MessageType", String(500)),
        default=None,
    )
    senderUserName: str = Field(
        sa_column=Column("SenderUserName", String(500)),
        default=None,
    )
    isRead: bool = Field(
        sa_column=Column("IsRead", Boolean),
        default=None,
    )
    siteId: int = Field(
        sa_column=Column("SiteId", Integer),
        default=None,
    )
    flowId: int = Field(
        sa_column=Column("FlowId", Integer),
        default=None,
    )
    nodeId: int = Field(
        sa_column=Column("NodeId", Integer),
        default=None,
    )
    dataId: int = Field(
        sa_column=Column("DataId", Integer),
        default=None,
    )
    title: str = Field(
        sa_column=Column("Title", String(500)),
        default=None,
    )
    url: str = Field(
        sa_column=Column("Url", String(500)),
        default=None,
    )
    body: str = Field(
        sa_column=Column("Body", Text),
        default=None,
    )
