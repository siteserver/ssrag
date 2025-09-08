from enum import Enum


class SiteType(str, Enum):
    WEB = "Web"
    MARKDOWN = "Markdown"
    DOCUMENT = "Document"
    CHAT = "Chat"
    CHATFLOW = "Chatflow"
    AGENT = "Agent"

    @classmethod
    def is_app_site(cls, siteType: "SiteType") -> bool:
        return siteType in [cls.CHAT, cls.CHATFLOW, cls.AGENT]
