from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime
from configs import table_names
from datetime import datetime


class ErrorLog(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_ERROR_LOG  # type: ignore
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
    category: str = Field(
        sa_column=Column("Category", String(500)),
        default=None,
    )
    pluginId: str = Field(
        sa_column=Column("PluginId", String(500)),
        default=None,
    )
    message: str = Field(
        sa_column=Column("Message", String(500)),
        default=None,
    )
    stackTrace: str = Field(
        sa_column=Column("StackTrace", String(500)),
        default=None,
    )
    summary: str = Field(
        sa_column=Column("Summary", String(500)),
        default=None,
    )
