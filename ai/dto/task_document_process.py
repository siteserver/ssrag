from typing import Optional
from pydantic import BaseModel


class TaskDocumentProcess(BaseModel):
    uuid: Optional[str] = None
    siteId: Optional[int] = None
    channelId: Optional[int] = None
    contentId: Optional[int] = None
    dirPath: Optional[str] = None
    fileName: Optional[str] = None
    extName: Optional[str] = None
