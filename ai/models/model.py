from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Boolean, Text
from datetime import datetime
from configs import table_names
from utils import string_utils
from typing import List
import json


class Model(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_MODEL  # type: ignore
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
    modelType: str = Field(
        sa_column=Column("ModelType", String(500)),
        default=None,
    )
    modelId: str = Field(
        sa_column=Column("ModelId", String(500)),
        default=None,
    )
    skills: str = Field(
        sa_column=Column("Skills", String(500)),
        default=None,
    )
    extendValues: str = Field(
        sa_column=Column("ExtendValues", Text),
        default=None,
    )

    @property
    def properties(self) -> dict:
        return string_utils.to_json_object(self.extendValues)

    @property
    def skill_list(self) -> List[str]:
        return json.loads(self.skills) if self.skills else []

    @skill_list.setter
    def skill_list(self, value: List[str]):
        self.skills = json.dumps(value)
