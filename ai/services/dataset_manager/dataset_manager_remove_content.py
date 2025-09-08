from repositories import (
    document_repository,
    segment_repository,
)
from vectors import Vector


def dataset_manager_remove_content(site_id: int, channel_id: int, content_id: int):
    document_repository.delete_by_content_id(site_id, channel_id, content_id)
    segment_repository.delete_by_content_id(site_id, channel_id, content_id)
    vector = Vector()
    vector.delete_by_content_id(site_id, channel_id, content_id)
