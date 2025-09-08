from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Boolean
from configs import table_names
from datetime import datetime


class User(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_USER  # type: ignore
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
        sa_column=Column("UserName", String),
        default=None,
    )
    departmentId: int = Field(
        sa_column=Column("DepartmentId", Integer),
        default=None,
    )
    manager: bool = Field(
        sa_column=Column("Manager", Boolean),
        default=None,
    )
    level: int = Field(
        sa_column=Column("Level", Integer),
        default=None,
    )
    checked: bool = Field(
        sa_column=Column("Checked", Boolean),
        default=None,
    )
    locked: bool = Field(
        sa_column=Column("Locked", Boolean),
        default=None,
    )
    displayName: str = Field(
        sa_column=Column("DisplayName", String),
        default=None,
    )
    mobile: str = Field(
        sa_column=Column("Mobile", String),
        default=None,
    )
    email: str = Field(
        sa_column=Column("Email", String),
        default=None,
    )
    avatarUrl: str = Field(
        sa_column=Column("AvatarUrl", String),
        default=None,
    )
