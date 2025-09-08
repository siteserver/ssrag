from utils.db_utils import engine
from sqlalchemy import text
from models import Content


class ContentRepository:
    def get(
        self, table_name: str, siteId: int, channelId: int, id: int
    ) -> Content | None:
        with engine.connect() as conn:
            result = conn.execute(
                text(
                    f'SELECT "Id", "Uuid", "CreatedDate", "LastModifiedDate", "SiteId", "ChannelId", "Title", "ImageUrl", "FileUrl", "Body", "Knowledge" FROM {table_name} WHERE "SiteId" = :siteId AND "ChannelId" = :channelId AND "Id" = :id'
                ),
                {"siteId": siteId, "channelId": channelId, "id": id},
            )
            row = result.fetchone()
            if row:
                return Content(
                    id=row[0],
                    uuid=row[1],
                    createdDate=row[2],
                    lastModifiedDate=row[3],
                    siteId=row[4],
                    channelId=row[5],
                    title=row[6],
                    imageUrl=row[7],
                    fileUrl=row[8],
                    body=row[9],
                    knowledge=row[10],
                )

    def update_knowledge(
        self, table_name: str, siteId: int, channelId: int, id: int, knowledge: bool
    ):
        with engine.connect() as conn:
            conn.execute(
                text(
                    f'UPDATE {table_name} SET "Knowledge" = :knowledge WHERE "SiteId" = :siteId AND "ChannelId" = :channelId AND "Id" = :id'
                ),
                {
                    "siteId": siteId,
                    "channelId": channelId,
                    "id": id,
                    "knowledge": knowledge,
                },
            )
            conn.commit()

    def get_checked_content_ids(
        self, table_name: str, siteId: int, channelId: int
    ) -> list[int]:
        with engine.connect() as conn:
            result = conn.execute(
                text(
                    f'SELECT "Id" FROM {table_name} WHERE "SiteId" = :siteId AND "ChannelId" = :channelId AND "Checked" = :checked'
                ),
                {"siteId": siteId, "channelId": channelId, "checked": True},
            )
            return [row[0] for row in result]
