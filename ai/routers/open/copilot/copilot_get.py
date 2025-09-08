from repositories import site_repository, chat_group_repository
from managers import AuthManager
from .__base import GetResult
from datetime import datetime
from fastapi import HTTPException
import uuid
from repositories import chat_message_repository


async def copilot_get(
    auth_manager: AuthManager,
    id: str | None = None,
    siteId: int | None = None,
    sessionId: str | None = None,
) -> GetResult:
    if id:
        site = site_repository.get_by_uuid(id)
    elif siteId:
        site = site_repository.get(siteId)

    if site is None:
        raise HTTPException(status_code=404, detail="Site not found")

    summary = site_repository.get_summary(site.id)
    if summary is None:
        raise HTTPException(status_code=404, detail="Site summary not found")

    values = site.site_values

    if sessionId:
        messages = chat_message_repository.get_all(site.id, sessionId)
    else:
        sessionId = str(uuid.uuid4())
        messages = []

    chatGroups = []
    adminName = auth_manager.get_admin_name()
    userName = auth_manager.get_user_name()
    if adminName:
        chatGroups = chat_group_repository.get_by_admin_name(adminName)
    elif userName:
        chatGroups = chat_group_repository.get_by_user_name(userName)

    if len(chatGroups) > 0:
        chatGroup = chatGroups[0]
        if chatGroup.createdDate.date() == datetime.now().date():
            sessionId = chatGroup.sessionId

    return GetResult(
        site=site,
        values=values,
        sites=[summary],
        chatGroups=chatGroups,
        sessionId=sessionId,
        messages=messages,
    )
