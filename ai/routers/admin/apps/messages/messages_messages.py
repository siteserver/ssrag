from .__base import MessageRequest, MessageResponse
from repositories import chat_message_repository


async def messages_messages(request: MessageRequest) -> MessageResponse:
    messages = chat_message_repository.get_all(request.siteId, request.sessionId)

    return MessageResponse(messages=messages)
