from dataclasses import dataclass
from sqlmodel import Field, SQLModel, Column, Integer, String, DateTime, Boolean, Text
from utils import string_utils
from configs import table_names
from datetime import datetime
import json
from dataclasses import asdict
from enums import SearchType


@dataclass
class SiteValues:
    providerModelId: str = ""
    llmSystemPrompt: str = ""

    datasetSearchType: SearchType = SearchType.SEMANTIC
    datasetMaxCount: int = 5
    datasetMinScore: float = 0.5

    displayType: str = "Home"
    headerText: str = "✨ AI 助手"
    footerText: str = "内容由AI生成，仅供参考"

    welcomeTitle: str = "你好，很高兴见到你！"
    welcomeVariant: str = "borderless"
    welcomePosition: str = "top"

    isHotPrompts: bool = False
    hotPromptsTitle: str = ""
    isFunctionPrompts: bool = False
    functionPromptsTitle: str = ""
    isInputPrompts: bool = True

    senderPlaceholder: str = "有问题，尽管问，SHIFT+ENTER 可换行"
    senderAllowSpeech: bool = True

    isChatDefaultOpen: bool = False
    isChatIconDraggable: bool = False
    chatOpenIconUrl: str = ""
    chatCloseIconUrl: str = ""

    separators: list[str] | None = None
    chunkSize: int | None = None
    chunkOverlap: int | None = None
    isChunkReplaces: bool | None = None
    isChunkDeletes: bool | None = None


class Site(SQLModel, table=True):
    __tablename__ = table_names.TABLE_NAME_SITE  # type: ignore
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
    siteDir: str = Field(
        sa_column=Column("SiteDir", String(500)),
        default=None,
    )
    tableName: str = Field(
        sa_column=Column("TableName", String(500)),
        default=None,
    )
    siteName: str = Field(
        sa_column=Column("SiteName", String(500)),
        default=None,
    )
    siteType: str = Field(
        sa_column=Column("SiteType", String(500)),
        default=None,
    )
    iconUrl: str = Field(
        sa_column=Column("IconUrl", String(500)),
        default=None,
    )
    imageUrl: str = Field(
        sa_column=Column("ImageUrl", String(500)),
        default=None,
    )
    keywords: str = Field(
        sa_column=Column("Keywords", String(500)),
        default=None,
    )
    description: str = Field(
        sa_column=Column("Description", String(500)),
        default=None,
    )
    root: bool = Field(
        sa_column=Column("Root", Boolean),
        default=None,
    )
    taxis: int = Field(
        sa_column=Column("Taxis", Integer),
        default=None,
    )
    disabled: bool = Field(
        sa_column=Column("Disabled", Boolean),
        default=None,
    )
    settings: str = Field(
        sa_column=Column("Settings", Text),
        default=None,
    )
    extendValues: str = Field(
        sa_column=Column("ExtendValues", Text),
        default=None,
    )

    @classmethod
    def load(cls, json_str: str | None) -> "Site":
        if json_str is None:
            return cls()
        try:
            return cls.model_validate(json.loads(json_str))
        except Exception as e:
            return cls()

    def json(self) -> str:
        return string_utils.to_json_str(self)

    @property
    def site_values(self) -> SiteValues:
        data = string_utils.to_json_object(self.settings)
        siteValues = SiteValues()
        siteValues.providerModelId = data.get("providerModelId", "")
        siteValues.llmSystemPrompt = data.get("llmSystemPrompt", "")
        siteValues.datasetSearchType = data.get(
            "datasetSearchType", SearchType.SEMANTIC
        )
        siteValues.datasetMaxCount = data.get("datasetMaxCount", 5)
        siteValues.datasetMinScore = data.get("datasetMinScore", 0.5)
        siteValues.displayType = data.get("displayType", "Home")
        siteValues.headerText = data.get("headerText", "✨ AI 助手")
        siteValues.footerText = data.get("footerText", "内容由AI生成，仅供参考")
        siteValues.welcomeTitle = data.get("welcomeTitle", "你好，很高兴见到你！")
        siteValues.welcomeVariant = data.get("welcomeVariant", "borderless")
        siteValues.welcomePosition = data.get("welcomePosition", "top")

        siteValues.isHotPrompts = data.get("isHotPrompts", False)
        siteValues.hotPromptsTitle = data.get("hotPromptsTitle", "")
        siteValues.isFunctionPrompts = data.get("isFunctionPrompts", False)
        siteValues.functionPromptsTitle = data.get("functionPromptsTitle", "")
        siteValues.isInputPrompts = data.get("isInputPrompts", True)

        siteValues.senderPlaceholder = data.get(
            "senderPlaceholder", "有问题，尽管问，SHIFT+ENTER 可换行"
        )
        siteValues.senderAllowSpeech = data.get("senderAllowSpeech", True)
        siteValues.isChatDefaultOpen = data.get("isChatDefaultOpen", False)
        siteValues.isChatIconDraggable = data.get("isChatIconDraggable", False)
        siteValues.chatOpenIconUrl = data.get("chatOpenIconUrl", "")
        siteValues.chatCloseIconUrl = data.get("chatCloseIconUrl", "")
        siteValues.separators = data.get("separators", [])
        siteValues.chunkSize = data.get("chunkSize", None)
        siteValues.chunkOverlap = data.get("chunkOverlap", None)
        siteValues.isChunkReplaces = data.get("isChunkReplaces", None)
        siteValues.isChunkDeletes = data.get("isChunkDeletes", None)
        return siteValues

    @site_values.setter
    def site_values(self, value: SiteValues):
        self.settings = json.dumps(asdict(value))
