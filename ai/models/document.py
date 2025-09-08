from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Boolean
from datetime import datetime
from configs import table_names


class Document(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_DOCUMENT  # type: ignore
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
    title: str = Field(
        sa_column=Column("Title", String(500)),
        default=None,
    )
    dirPath: str = Field(
        sa_column=Column("DirPath", String(500)),
        default=None,
    )
    fileName: str = Field(
        sa_column=Column("FileName", String(500)),
        default=None,
    )
    extName: str = Field(
        sa_column=Column("ExtName", String(500)),
        default=None,
    )
    fileSize: int = Field(
        sa_column=Column("FileSize", Integer),
        default=None,
    )
    separators: str = Field(
        sa_column=Column("Separators", String(500)),
        default=None,
    )
    chunkSize: int = Field(
        sa_column=Column("ChunkSize", Integer),
        default=None,
    )
    chunkOverlap: int = Field(
        sa_column=Column("ChunkOverlap", Integer),
        default=None,
    )
    isChunkReplaces: bool = Field(
        sa_column=Column("IsChunkReplaces", Boolean),
        default=None,
    )
    isChunkDeletes: bool = Field(
        sa_column=Column("IsChunkDeletes", Boolean),
        default=None,
    )
