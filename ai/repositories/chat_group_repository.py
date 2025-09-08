from datetime import datetime
from sqlmodel import Session, select, desc
from models import ChatGroup
from utils.db_utils import engine


class ChatGroupRepository:
    def insert(self, group: ChatGroup) -> int:
        with Session(engine) as session:
            session.add(group)
            session.commit()
            session.refresh(group)
            return group.id

    def update_title(self, siteId: int, sessionId: str, title: str):
        with Session(engine) as session:
            statement = select(ChatGroup).where(
                ChatGroup.siteId == siteId, ChatGroup.sessionId == sessionId
            )
            group = session.exec(statement).first()
            if group:
                group.title = title
                session.commit()

    def update_deleted(self, siteId: int, sessionId: str):
        with Session(engine) as session:
            statement = select(ChatGroup).where(
                ChatGroup.siteId == siteId, ChatGroup.sessionId == sessionId
            )
            group = session.exec(statement).first()
            if group:
                group.isDeleted = True
                session.commit()

    def get_all_by_site_id(
        self,
        siteId: int,
        page: int,
        pageSize: int,
        dateStart: str | None,
        dateEnd: str | None,
        title: str | None,
    ) -> list[ChatGroup]:
        with Session(engine) as session:
            statement = select(ChatGroup).where(ChatGroup.siteId == siteId)
            if dateStart:
                statement = statement.where(
                    ChatGroup.createdDate >= datetime.strptime(dateStart, "%Y-%m-%d")
                )
            if dateEnd:
                statement = statement.where(
                    ChatGroup.createdDate <= datetime.strptime(dateEnd, "%Y-%m-%d")
                )
            if title:
                statement = statement.where(
                    ChatGroup.title.ilike(f"%{title}%")  # type: ignore
                )

            statement = (
                statement.order_by(desc(ChatGroup.id))
                .offset((page - 1) * pageSize)
                .limit(pageSize)
            )
            return list(session.exec(statement).all())

    def get_by_user_name(self, user_name: str) -> list[ChatGroup]:
        with Session(engine) as session:
            statement = select(ChatGroup).where(ChatGroup.userName == user_name)
            statement = statement.where(
                (ChatGroup.isDeleted == None) | (ChatGroup.isDeleted == False)
            )
            statement = statement.order_by(desc(ChatGroup.id))
            return list(session.exec(statement).all())

    def get_by_admin_name(self, admin_name: str) -> list[ChatGroup]:
        with Session(engine) as session:
            statement = select(ChatGroup).where(ChatGroup.adminName == admin_name)
            statement = statement.where(
                (ChatGroup.isDeleted == None) | (ChatGroup.isDeleted == False)
            )
            statement = statement.order_by(desc(ChatGroup.id))
            return list(session.exec(statement).all())

    def get_by_session_id(self, siteId: int, sessionId: str) -> ChatGroup | None:
        with Session(engine) as session:
            statement = select(ChatGroup).where(
                ChatGroup.siteId == siteId, ChatGroup.sessionId == sessionId
            )
            return session.exec(statement).first()  # type: ignore

    def delete(self, site_id: int, session_id: str):
        with Session(engine) as session:
            statement = select(ChatGroup).where(
                ChatGroup.siteId == site_id, ChatGroup.sessionId == session_id
            )
            groups = session.exec(statement).all()
            for group in groups:
                session.delete(group)
            session.commit()

    def delete_all(self, site_id: int):
        with Session(engine) as session:
            statement = select(ChatGroup).where(ChatGroup.siteId == site_id)
            groups = session.exec(statement).all()
            for group in groups:
                session.delete(group)
            session.commit()

    def get_all(self, site_id: int, user_name: str, limit: int) -> list[ChatGroup]:
        with Session(engine) as session:
            statement = (
                select(ChatGroup)
                .where(ChatGroup.siteId == site_id, ChatGroup.userName == user_name)
                .order_by(desc(ChatGroup.id))
                .limit(limit)
            )
            return list(session.exec(statement).all())

    def get_count(
        self, site_id: int, user_name: str, keyword: str, date_from: str, date_to: str
    ) -> int:
        with Session(engine) as session:
            statement = self._build_query(
                site_id, user_name, keyword, date_from, date_to
            )
            return len(session.exec(statement).all())

    def get_all_with_filters(
        self,
        site_id: int,
        user_name: str,
        keyword: str,
        date_from: str,
        date_to: str,
        offset: int,
        limit: int,
    ) -> list[ChatGroup]:
        with Session(engine) as session:
            statement = self._build_query(
                site_id, user_name, keyword, date_from, date_to
            )
            statement = statement.offset(offset).limit(limit)
            return list(session.exec(statement).all())

    def _build_query(
        self, site_id: int, user_name: str, keyword: str, date_from: str, date_to: str
    ):
        statement = (
            select(ChatGroup)
            .where(ChatGroup.siteId == site_id)
            .order_by(desc(ChatGroup.id))
        )

        if user_name:
            statement = statement.where(ChatGroup.userName.ilike(f"%{user_name}%"))  # type: ignore

        if keyword:
            statement = statement.where(ChatGroup.title.ilike(f"%{keyword}%"))  # type: ignore

        if date_from:
            statement = statement.where(
                ChatGroup.createdDate >= datetime.strptime(date_from, "%Y-%m-%d")
            )

        if date_to:
            statement = statement.where(
                ChatGroup.createdDate <= datetime.strptime(date_to, "%Y-%m-%d")
            )

        return statement
