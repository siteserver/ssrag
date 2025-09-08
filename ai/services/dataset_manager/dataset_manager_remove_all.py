from repositories import (
    document_repository,
    segment_repository,
)
from vectors import Vector


def dataset_manager_remove_all(site_id: int):
    document_repository.delete_by_site_id(site_id)
    segment_repository.delete_by_site_id(site_id)
    vector = Vector()
    vector.delete_by_site_id(site_id)
