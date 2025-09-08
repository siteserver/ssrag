from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime
from configs import table_names
from datetime import datetime


class Department(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_DEPARTMENT  # type: ignore
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
    code: str = Field(
        sa_column=Column("Code", String(500)),
        default=None,
    )
    name: str = Field(
        sa_column=Column("Name", String(500)),
        default=None,
    )
    parentId: int = Field(
        sa_column=Column("ParentId", Integer),
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
    count: int = Field(
        sa_column=Column("Count", Integer),
        default=None,
    )
    wxCount: int = Field(
        sa_column=Column("WxCount", Integer),
        default=None,
    )
    managerUserNames: str = Field(
        sa_column=Column("ManagerUserNames", String(500)),
        default=None,
    )
    taxis: int = Field(
        sa_column=Column("Taxis", Integer),
        default=None,
    )
    description: str = Field(
        sa_column=Column("Description", String(500)),
        default=None,
    )
