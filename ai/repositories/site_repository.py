import uuid
from datetime import datetime
from sqlalchemy import text, delete, update, insert, select
from sqlalchemy.orm import Session
from utils.db_utils import engine
from models import Site, SiteSummary
from managers import cache_manager
from configs import table_names


ATTR_ID = "Id"
ATTR_SITE_NAME = "SiteName"
ATTR_SITE_TYPE = "SiteType"
ATTR_ICON_URL = "IconUrl"
ATTR_SITE_DIR = "SiteDir"
ATTR_DESCRIPTION = "Description"
ATTR_TABLE_NAME = "TableName"
ATTR_ROOT = "Root"
ATTR_DISABLED = "Disabled"
ATTR_TAXIS = "Taxis"


class SiteRepository:
    def get_summaries(self) -> list[SiteSummary]:
        cache_key = cache_manager.get_list_key(table_names.TABLE_NAME_SITE)
        json_str = cache_manager.get(cache_key)
        if json_str:
            return SiteSummary.load_summaries(json_str)

        with engine.connect() as conn:
            result = conn.execute(
                text(
                    f"""
SELECT "{ATTR_ID}", "{ATTR_SITE_NAME}", "{ATTR_SITE_TYPE}", "{ATTR_ICON_URL}", "{ATTR_SITE_DIR}", "{ATTR_DESCRIPTION}", "{ATTR_TABLE_NAME}", "{ATTR_ROOT}", "{ATTR_DISABLED}", "{ATTR_TAXIS}"
FROM "{table_names.TABLE_NAME_SITE}"
WHERE "{ATTR_ID}" != 0
ORDER BY "{ATTR_TAXIS}", "{ATTR_ID}"
                    """
                ),
            )
            summaries: list[SiteSummary] = [
                SiteSummary(
                    id=row[0],
                    siteName=row[1],
                    siteType=row[2],
                    iconUrl=row[3],
                    siteDir=row[4],
                    description=row[5],
                    tableName=row[6],
                    root=row[7],
                    disabled=row[8],
                    taxis=row[9],
                )
                for row in result.all()
            ]

            json_str = SiteSummary.json_summaries(summaries)
            cache_manager.set(cache_key, json_str)

            return summaries

    def get_summary(self, siteId: int) -> SiteSummary | None:
        summaries = self.get_summaries()
        for summary in summaries:
            if summary.id == siteId:
                return summary
        return None

    def get_by_uuid(self, uuid: str) -> Site | None:
        with Session(engine) as session:
            site = session.query(Site).filter(Site.uuid == uuid).first()  # type: ignore
            return site

    def get(self, siteId: int) -> Site | None:
        cache_key = cache_manager.get_entity_key(
            table_names.TABLE_NAME_SITE, str(siteId)
        )
        json_str = cache_manager.get(cache_key)
        if json_str:
            site = Site.load(json_str)
            if site and site.id == siteId:
                return site

        with Session(engine) as session:
            site = session.query(Site).filter(Site.id == siteId).first()  # type: ignore
            if site is not None:
                json_str = site.json()
                cache_manager.set(cache_key, json_str)
            return site

    def update_site(self, site: Site) -> None:
        with Session(engine) as session:
            stmt = (
                update(Site)
                .where(Site.id == site.id)  # type: ignore
                .values(
                    siteName=site.siteName,
                    iconUrl=site.iconUrl,
                    description=site.description,
                    siteDir=site.siteDir,
                    siteType=site.siteType,
                    root=site.root,
                    disabled=site.disabled,
                    settings=site.settings,
                )
            )
            session.execute(stmt)
            session.commit()
        self.clear_cache(site.id)

    def delete(self, siteId: int) -> None:
        statement = delete(Site).where(Site.id == siteId)  # type: ignore
        with Session(engine) as session:
            session.execute(statement)
            session.commit()
        self.clear_cache(siteId)

    def get_site_ids(self) -> list[int]:
        summaries = self.get_summaries()
        return [summary.id for summary in summaries]

    def get_max_taxis(self) -> int:
        summaries = self.get_summaries()
        return max(
            (summary.taxis for summary in summaries if summary.taxis is not None),
            default=0,
        )

    def set_taxis(self, site_id: int, taxis: int):
        with Session(engine) as session:
            stmt = (
                update(Site)
                .where(Site.id == site_id)  # type: ignore
                .values(taxis=taxis)
            )
            session.execute(stmt)
            session.commit()
        self.clear_cache(site_id)

    def update_taxis_down(self, site_id: int, taxis: int):
        with Session(engine) as session:
            statement = (
                select(Site)
                .where(Site.taxis > taxis, Site.id != site_id)  # type: ignore
                .order_by(Site.taxis)  # type: ignore
            )
            higher_site = session.execute(statement).first()

            if higher_site:
                self.set_taxis(site_id, higher_site[0].taxis)
                self.set_taxis(higher_site[0].id, taxis)

    def update_taxis_up(self, site_id: int, taxis: int):
        with Session(engine) as session:
            statement = (
                select(Site)
                .where(Site.taxis < taxis, Site.id != site_id)  # type: ignore
                .order_by(Site.taxis.desc())  # type: ignore
            )
            lower_site = session.execute(statement).first()

            if lower_site:
                self.set_taxis(site_id, lower_site[0].taxis)
                self.set_taxis(lower_site[0].id, taxis)

    def clear_cache(self, siteId: int | None = None) -> None:
        if siteId:
            cache_manager.delete(
                cache_manager.get_entity_key(table_names.TABLE_NAME_SITE, str(siteId))
            )
        cache_manager.delete(cache_manager.get_list_key(table_names.TABLE_NAME_SITE))
