from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Boolean
from configs import table_names
from datetime import datetime


class Dataset(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_DATASET  # type: ignore
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
    datasetSiteId: int = Field(
        sa_column=Column("DatasetSiteId", Integer),
        default=None,
    )
    datasetAllChannels: bool = Field(
        sa_column=Column("DatasetAllChannels", Boolean),
        default=None,
    )
    datasetChannelIds: str = Field(
        sa_column=Column("ChannelIds", String(500)),
        default=None,
    )
