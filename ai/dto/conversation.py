from pydantic import BaseModel


class Message(BaseModel):
    role: str
    content: str


class ChatRequest(BaseModel):
    messages: list[Message]
    stream: bool
    modelId: str | None = None


class Delta(BaseModel):
    content: str


class Choice(BaseModel):
    index: int
    delta: Delta
    role: str


class ChatResponse(BaseModel):
    id: str
    choices: list[Choice]
    created: int
    model: str
