from repositories import (
    document_repository,
    segment_repository,
)
from vectors import Vector


def dataset_manager_remove_channel(site_id: int, channel_id: int):
    document_repository.delete_by_channel_id(site_id, channel_id)
    segment_repository.delete_by_channel_id(site_id, channel_id)
    vector = Vector()
    vector.delete_by_channel_id(site_id, channel_id)
