from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime
from configs import table_names
from datetime import datetime


class Administrator(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_ADMINISTRATOR  # type: ignore
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
        sa_column=Column("UserName", String(50)),
        default=None,
    )
