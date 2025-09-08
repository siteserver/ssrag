import uuid
from models import ChatGroup, ChatMessage
from enums import AiRoleType
from managers import AuthManager
from .__base import HistoryRequest, HistoryResult
from repositories import chat_group_repository, chat_message_repository


async def chat_history(
    auth_manager: AuthManager, request: HistoryRequest
) -> HistoryResult:
    sessionId = request.sessionId
    if sessionId is None:
        sessionId = str(uuid.uuid4())

    chat_group = chat_group_repository.get_by_session_id(request.siteId, sessionId)
    if chat_group is None:
        chat_group = ChatGroup(
            uuid=str(uuid.uuid4()),
            siteId=request.siteId,
            sessionId=sessionId,
            title=request.message,
            adminName=auth_manager.get_admin_name() or "",
            userName=auth_manager.get_user_name() or "",
        )
        chat_group_repository.insert(chat_group)

    chat_message_user = ChatMessage(
        uuid=str(uuid.uuid4()),
        siteId=request.siteId,
        sessionId=sessionId,
        role=AiRoleType.USER.value,
        content=request.message,
    )
    chat_message_repository.insert(chat_message_user)

    chat_message_assistant = ChatMessage(
        uuid=str(uuid.uuid4()),
        siteId=request.siteId,
        sessionId=sessionId,
        role=AiRoleType.ASSISTANT.value,
        reasoning=request.reasoning,
        content=request.content,
    )
    chat_message_repository.insert(chat_message_assistant)

    return HistoryResult(chatGroup=chat_group)
