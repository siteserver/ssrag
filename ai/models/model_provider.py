from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Boolean, Text
from datetime import datetime
from configs import table_names
from utils import string_utils


class ModelProvider(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_MODEL_PROVIDER  # type: ignore
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
    providerId: str = Field(
        sa_column=Column("ProviderId", String(500)),
        default=None,
    )
    providerName: str = Field(
        sa_column=Column("ProviderName", String(500)),
        default=None,
    )
    iconUrl: str = Field(
        sa_column=Column("IconUrl", String(500)),
        default=None,
    )
    description: str = Field(
        sa_column=Column("Description", String(500)),
        default=None,
    )
    credentials: str = Field(
        sa_column=Column("Credentials", Text),
        default=None,
    )
    credentialsSalt: str = Field(
        sa_column=Column("CredentialsSalt", String(500)),
        default=None,
    )
