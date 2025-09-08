from pydantic import BaseModel


class Result(BaseModel):
    id: str
    siteId: int
    channelId: int
    contentId: int
    documentId: int
    text: str
    docName: str
    extName: str
    score: float
