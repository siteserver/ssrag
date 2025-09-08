from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Boolean
from configs import table_names
from datetime import datetime


class FlowVariable(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_FLOW_VARIABLE  # type: ignore
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
    nodeId: str = Field(
        sa_column=Column("NodeId", String(500)),
        default=None,
    )
    type: str = Field(
        sa_column=Column("Type", String(500)),
        default=None,
    )
    name: str = Field(
        sa_column=Column("Name", String(500)),
        default=None,
    )
    dataType: str = Field(
        sa_column=Column("DataType", String(500)),
        default=None,
    )
    description: str = Field(
        sa_column=Column("Description", String(500)),
        default=None,
    )
    isDisabled: bool = Field(
        sa_column=Column("IsDisabled", Boolean, default=False),
        default=False,
    )
    isReference: bool = Field(
        sa_column=Column("IsReference", Boolean, default=False),
        default=False,
    )
    referenceNodeId: str = Field(
        sa_column=Column("ReferenceNodeId", String(500)),
        default=None,
    )
    referenceName: str = Field(
        sa_column=Column("ReferenceName", String(500)),
        default=None,
    )
    value: str = Field(
        sa_column=Column("Value", String(500)),
        default=None,
    )
