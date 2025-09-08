from pydantic import BaseModel


class TaskContentProcess(BaseModel):
    siteId: int
    channelId: int
    contentId: int
