from sqlmodel import Session, select, delete, asc
from models import Prompt
from utils.db_utils import engine
from managers import cache_manager
from configs import table_names


class PromptRepository:
    def get_all(self, site_id: int) -> list[Prompt]:
        cache_key = cache_manager.get_list_key(
            table_names.TABLE_NAME_PROMPT, str(site_id)
        )
        json_str = cache_manager.get(cache_key)
        if json_str:
            return Prompt.load_prompts(json_str)

        statement = (
            select(Prompt).where(Prompt.siteId == site_id).order_by(asc(Prompt.taxis))
        )

        with Session(engine) as session:
            prompts = session.exec(statement).all()

        items = list(prompts)

        json_str = Prompt.json_prompts(items)
        cache_manager.set(cache_key, json_str)
        return items

    def create(self, prompt: Prompt):
        self.clear_cache(prompt.siteId)
        with Session(engine) as session:
            session.add(prompt)
            session.commit()

    def delete_all(self, site_id: int):
        self.clear_cache(site_id)
        statement = delete(Prompt).where(Prompt.siteId == site_id)  # type: ignore
        with Session(engine) as session:
            session.exec(statement)  # type: ignore
            session.commit()

    def clear_cache(self, siteId: int | None = None) -> None:
        cache_manager.delete(
            cache_manager.get_list_key(table_names.TABLE_NAME_PROMPT, str(siteId))
        )
