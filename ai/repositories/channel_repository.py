from sqlmodel import Session, select, text
from utils.db_utils import engine
from models import Channel, ChannelSummary
from dto import Cascade
from managers import cache_manager
from configs import table_names
from utils import string_utils


class ChannelRepository:
    def _get_list_key(self, siteId: int) -> str:
        return cache_manager.get_list_key(table_names.TABLE_NAME_CHANNEL, str(siteId))

    def _get_entity_key(self, channelId: int) -> str:
        return cache_manager.get_entity_key(
            table_names.TABLE_NAME_CHANNEL, str(channelId)
        )

    def get_summary(self, siteId: int, channelId: int) -> ChannelSummary | None:
        summaries = self.get_summaries(siteId)
        for summary in summaries:
            if summary.id == channelId:
                return summary
        return None

    def get_summaries(self, siteId: int) -> list[ChannelSummary]:
        cache_key = self._get_list_key(siteId)
        json_str = cache_manager.get(cache_key)
        if json_str:
            return ChannelSummary.load_summaries(json_str)

        with engine.connect() as conn:
            result = conn.execute(
                text(
                    f"""
SELECT "Id", "ChannelName", "ParentId", "ParentsPath", "IndexName", "Knowledge", "TableName", "Taxis", "AddDate"
FROM "{table_names.TABLE_NAME_CHANNEL}"
WHERE "SiteId" = :siteId
ORDER BY "Taxis", "Id"
                    """
                ),
                {"siteId": siteId},
            )
            summaries = [
                ChannelSummary(
                    id=row[0],
                    channelName=row[1],
                    parentId=row[2],
                    parentsPath=string_utils.split_to_int(row[3]),
                    indexName=row[4],
                    knowledge=row[5],
                    tableName=row[6],
                    taxis=row[7],
                    addDate=row[8],
                )
                for row in result.fetchall()
            ]

            json_str = ChannelSummary.json_summaries(summaries)
            cache_manager.set(cache_key, json_str)

            return summaries

    def get(self, siteId: int, channelId: int) -> Channel | None:
        cache_key = self._get_entity_key(channelId)
        json_str = cache_manager.get(cache_key)
        if json_str:
            channel = Channel.load(json_str)
            if channel and channel.id == channelId:
                return channel

        with Session(engine) as session:
            statement = select(Channel).where(
                Channel.siteId == siteId, Channel.id == channelId
            )
            channel = session.exec(statement).first()
            if channel is not None:
                json_str = channel.json()
                cache_manager.set(cache_key, json_str)
            return channel

    def update(self, channel: Channel) -> None:
        with Session(engine) as session:
            session.add(channel)
            session.commit()
            session.refresh(channel)

    def get_breadcrumb(self, siteId: int, channelId: int) -> list[ChannelSummary]:
        summary = self.get_summary(siteId, channelId)
        if summary is None:
            return []

        breadcrumb = []
        if summary.parentsPath:
            for parentId in summary.parentsPath:
                parent_summary = self.get_summary(siteId, parentId)
                if parent_summary:
                    breadcrumb.append(parent_summary)
        breadcrumb.append(summary)

        return breadcrumb

    def get_channel_options(self, siteId: int, parentId: int) -> list[Cascade]:
        with Session(engine) as session:
            statement = select(Channel).where(
                Channel.siteId == siteId, Channel.parentId == parentId
            )
            channels = session.exec(statement).all()

            option_list = []
            for channel in channels:
                option_list.append(
                    Cascade(
                        value=channel.id,
                        label=channel.channelName,
                        children=self.get_channel_options(siteId, channel.id),
                    )
                )

            return option_list

    def get_channel_ids_with_self(self, parentId: int) -> list[int]:
        channel_ids = self.get_channel_ids(parentId)
        channel_ids.append(parentId)
        return channel_ids

    def get_channel_ids(self, parentId: int) -> list[int]:
        with Session(engine) as session:
            statement = select(Channel.id).where(Channel.parentId == parentId)
            channel_ids = session.exec(statement).all()

            all_channel_ids = [channel_id for channel_id in channel_ids]

            # 循环查询下级栏目
            for channel_id in channel_ids:
                child_ids = self.get_channel_ids(channel_id)
                all_channel_ids.extend(child_ids)

            return all_channel_ids
