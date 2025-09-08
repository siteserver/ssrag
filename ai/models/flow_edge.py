from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime
from configs import table_names
from datetime import datetime


class FlowEdge(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_FLOW_EDGE  # type: ignore
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
    source: str = Field(
        sa_column=Column("Source", String(500)),
        default=None,
    )
    sourceHandle: str = Field(
        sa_column=Column("SourceHandle", String(500)),
        default=None,
    )
    target: str = Field(
        sa_column=Column("Target", String(500)),
        default=None,
    )
    targetHandle: str = Field(
        sa_column=Column("TargetHandle", String(500)),
        default=None,
    )
