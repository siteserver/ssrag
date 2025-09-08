from repositories import site_repository, chat_group_repository
from managers import AuthManager
from .__base import GetResult
from datetime import datetime
from enums import SiteType
import uuid
from repositories import chat_message_repository
from models import Site, SiteValues, ChatMessage


async def home_get(
    auth_manager: AuthManager,
    id: str | None = None,
    siteId: int | None = None,
    sessionId: str | None = None,
) -> GetResult:
    all_sites = site_repository.get_summaries()
    sites = [
        site
        for site in all_sites
        if site.siteType is not None and SiteType.is_app_site(site.siteType)
    ]
    if siteId:
        sites = [site for site in sites if site.id == siteId]

    site: Site | None = None
    values: SiteValues | None = None
    messages: list[ChatMessage] = []
    if id:
        site = site_repository.get_by_uuid(id)
    elif siteId:
        site = site_repository.get(siteId)

    if site:
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
        sites=sites,
        sessionId=sessionId,
        messages=messages,
        chatGroups=chatGroups,
    )
