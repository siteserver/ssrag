from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Boolean
from configs import table_names
from datetime import datetime


class UserGroup(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_USER_GROUP  # type: ignore
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
    isDefault: bool = Field(
        sa_column=Column("IsDefault", Boolean),
        default=None,
    )
    isManager: bool = Field(
        sa_column=Column("IsManager", Boolean),
        default=None,
    )
    homePermissions: str = Field(
        sa_column=Column("HomePermissions", String),
        default=None,
    )
    groupName: str = Field(
        sa_column=Column("GroupName", String),
        default=None,
    )
    taxis: int = Field(
        sa_column=Column("Taxis", Integer),
        default=None,
    )
    description: str = Field(
        sa_column=Column("Description", String),
        default=None,
    )
