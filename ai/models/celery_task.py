from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime
from configs import table_names
from datetime import datetime


class CeleryTask(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_CELERY_TASK  # type: ignore
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
    contentId: int = Field(
        sa_column=Column("ContentId", Integer),
        default=None,
    )
    taskId: str = Field(
        sa_column=Column("TaskId", String(500)),
        default=None,
    )
    taskStatus: str = Field(
        sa_column=Column("TaskStatus", String(500)),
        default=None,
    )
    taskResult: str = Field(
        sa_column=Column("TaskResult", String(500)),
        default=None,
    )
