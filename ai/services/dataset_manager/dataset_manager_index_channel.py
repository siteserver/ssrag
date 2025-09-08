from repositories import (
    site_repository,
    channel_repository,
    content_repository,
)
from .dataset_manager_index_content import dataset_manager_index_content


def dataset_manager_index_channel(siteId: int, channelId: int):
    site = site_repository.get(siteId)
    if site is None:
        return

    channel_summary = channel_repository.get_summary(siteId, channelId)
    if channel_summary is None:
        return

    if channel_summary.knowledge:
        content_ids = content_repository.get_checked_content_ids(
            site.tableName, siteId, channel_summary.id
        )
        for content_id in content_ids:
            dataset_manager_index_content(siteId, channel_summary.id, content_id)
