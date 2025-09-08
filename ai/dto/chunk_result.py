from pydantic import BaseModel


class ChunkResult(BaseModel):
    id: int
    fileName: str
    extName: str
    fileSize: int
    chunks: list[str]
