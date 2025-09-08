from sqlalchemy import text
from models import Dataset
from utils.db_utils import engine
from configs import table_names
from dto import SearchScope
from utils import string_utils


class DatasetRepository:

    def insert(self, dataset: Dataset):
        with engine.connect() as conn:
            conn.execute(
                text(
                    f"""
                    INSERT INTO {table_names.TABLE_NAME_DATASET} ("SiteId", "NodeId", "DatasetSiteId", "DatasetAllChannels", "DatasetChannelIds")
                    VALUES (:siteId, :nodeId, :datasetSiteId, :datasetAllChannels, :datasetChannelIds)
                    """
                ),
                {
                    "siteId": dataset.siteId,
                    "nodeId": dataset.nodeId,
                    "datasetSiteId": dataset.datasetSiteId,
                    "datasetAllChannels": dataset.datasetAllChannels,
                    "datasetChannelIds": dataset.datasetChannelIds,
                },
            )
            conn.commit()

    def update(self, dataset: Dataset):
        with engine.connect() as conn:
            conn.execute(
                text(
                    f'UPDATE {table_names.TABLE_NAME_DATASET} SET "DatasetAllChannels" = :datasetAllChannels, "DatasetChannelIds" = :datasetChannelIds WHERE "SiteId" = :siteId AND "NodeId" = :nodeId AND "DatasetSiteId" = :datasetSiteId'
                ),
                {
                    "datasetAllChannels": dataset.datasetAllChannels,
                    "datasetChannelIds": dataset.datasetChannelIds,
                    "siteId": dataset.siteId,
                    "nodeId": dataset.nodeId,
                    "datasetSiteId": dataset.datasetSiteId,
                },
            )
            conn.commit()

    def exists(self, siteId: int, nodeId: str, datasetSiteId: int) -> bool:
        with engine.connect() as conn:
            result = conn.execute(
                text(
                    f"""
                    SELECT COUNT(*) FROM {table_names.TABLE_NAME_DATASET} WHERE "SiteId" = :siteId AND "NodeId" = :nodeId AND "DatasetSiteId" = :datasetSiteId
                    """
                ),
                {"siteId": siteId, "nodeId": nodeId, "datasetSiteId": datasetSiteId},
            )
            count = result.scalar()
            return (count or 0) > 0

    def delete(self, siteId: int, nodeId: str, datasetSiteId: int):
        with engine.connect() as conn:
            conn.execute(
                text(
                    f'DELETE FROM {table_names.TABLE_NAME_DATASET} WHERE "SiteId" = :siteId AND "NodeId" = :nodeId AND "DatasetSiteId" = :datasetSiteId'
                ),
                {"siteId": siteId, "nodeId": nodeId, "datasetSiteId": datasetSiteId},
            )
            conn.commit()

    def delete_all(self, siteId: int):
        with engine.connect() as conn:
            conn.execute(
                text(
                    f'DELETE FROM {table_names.TABLE_NAME_DATASET} WHERE "SiteId" = :siteId'
                ),
                {"siteId": siteId},
            )
            conn.commit()

    def get_selected_site_ids(self, siteId: int, nodeId: str) -> list[int]:
        with engine.connect() as conn:
            result = conn.execute(
                text(
                    f'SELECT "DatasetSiteId" FROM {table_names.TABLE_NAME_DATASET} WHERE "SiteId" = :siteId AND "NodeId" = :nodeId'
                ),
                {"siteId": siteId, "nodeId": nodeId},
            )
            return [row[0] for row in result.fetchall()]

    def get(self, siteId: int, nodeId: str, datasetSiteId: int) -> Dataset | None:
        with engine.connect() as conn:
            result = conn.execute(
                text(
                    f'SELECT "DatasetAllChannels", "DatasetChannelIds" FROM {table_names.TABLE_NAME_DATASET} WHERE "SiteId" = :siteId AND "NodeId" = :nodeId AND "DatasetSiteId" = :datasetSiteId'
                ),
                {"siteId": siteId, "nodeId": nodeId, "datasetSiteId": datasetSiteId},
            )
            row = result.fetchone()
            if row is None:
                return None
            return Dataset(
                siteId=siteId,
                nodeId=nodeId,
                datasetSiteId=datasetSiteId,
                datasetAllChannels=row[0],
                datasetChannelIds=row[1],
            )

    def get_datasets(self, siteId: int, nodeId: str) -> list[Dataset]:
        with engine.connect() as conn:
            result = conn.execute(
                text(
                    f'SELECT "DatasetSiteId", "DatasetAllChannels", "DatasetChannelIds" FROM {table_names.TABLE_NAME_DATASET} WHERE "SiteId" = :siteId AND "NodeId" = :nodeId'
                ),
                {"siteId": siteId, "nodeId": nodeId},
            )
            return [
                Dataset(
                    siteId=siteId,
                    nodeId=nodeId,
                    datasetSiteId=row[0],
                    datasetAllChannels=row[1],
                    datasetChannelIds=row[2],
                )
                for row in result.fetchall()
            ]

    def get_search_scope(self, siteId: int, nodeId: str) -> SearchScope:
        search_scope = SearchScope(siteIds=[], channelIds=[], contentIds=[])
        datasets = self.get_datasets(siteId, nodeId)
        for dataset in datasets:
            if dataset.datasetAllChannels:
                if dataset.datasetSiteId not in search_scope.siteIds:
                    search_scope.siteIds.append(dataset.datasetSiteId)
            elif dataset.datasetChannelIds:
                channelIds = string_utils.split_to_int(dataset.datasetChannelIds)
                for channelId in channelIds:
                    if channelId not in search_scope.channelIds:
                        search_scope.channelIds.append(channelId)
        return search_scope
