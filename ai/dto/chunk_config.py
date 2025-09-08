from pydantic import BaseModel


class ChunkConfig(BaseModel):
    separators: list[str]
    chunkSize: int
    chunkOverlap: int
    isChunkReplaces: bool
    isChunkDeletes: bool
