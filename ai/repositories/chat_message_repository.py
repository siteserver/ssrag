from datetime import datetime
from sqlmodel import Session, select, desc, asc
from models import ChatMessage
from utils.db_utils import engine


class ChatMessageRepository:
    def insert(self, message: ChatMessage) -> int:
        """Insert a new chat message and delete old messages if threshold is reached"""
        self._delete_if_threshold()

        with Session(engine) as session:
            session.add(message)
            session.commit()
            session.refresh(message)
            return message.id

    def get_count(self, siteId: int, sessionId: str) -> int:
        """Get count of messages for given app and session"""
        with Session(engine) as session:
            statement = select(ChatMessage).where(
                ChatMessage.siteId == siteId, ChatMessage.sessionId == sessionId
            )
            return len(session.exec(statement).all())

    def delete(self, siteId: int, sessionId: str, message_id: int) -> None:
        """Delete specific message"""
        with Session(engine) as session:
            statement = select(ChatMessage).where(
                ChatMessage.siteId == siteId,
                ChatMessage.sessionId == sessionId,
                ChatMessage.id == message_id,
            )
            message = session.exec(statement).first()
            if message:
                session.delete(message)
                session.commit()

    def delete_all(self, siteId: int, sessionId: str | None = None) -> None:
        """Delete all messages for given app and optional session"""
        with Session(engine) as session:
            statement = select(ChatMessage).where(ChatMessage.siteId == siteId)
            if sessionId:
                statement = statement.where(ChatMessage.sessionId == sessionId)
            messages = session.exec(statement).all()
            for message in messages:
                session.delete(message)
            session.commit()

    def get_all(self, siteId: int, sessionId: str) -> list[ChatMessage]:
        """Get all messages for given app and session"""
        with Session(engine) as session:
            statement = (
                select(ChatMessage)
                .where(ChatMessage.siteId == siteId, ChatMessage.sessionId == sessionId)
                .order_by(asc(ChatMessage.id))
            )
            return list(session.exec(statement).all())

    def _delete_if_threshold(self) -> None:
        """Delete messages older than configured threshold"""
        # TODO: Implement threshold check using config repository
        pass

    def get_list(
        self,
        siteId: int,
        sessionId: str,
        keyword: str | None = None,
        startDate: str | None = None,
        endDate: str | None = None,
        page: int = 1,
        pageSize: int = 10,
    ) -> tuple[int, list[ChatMessage]]:
        """Get paginated list of messages with filters"""
        with Session(engine) as session:
            statement = (
                select(ChatMessage)
                .where(ChatMessage.siteId == siteId)
                .order_by(desc(ChatMessage.id))
            )

            if sessionId:
                statement = statement.where(ChatMessage.sessionId == sessionId)

            if startDate:
                statement = statement.where(
                    ChatMessage.createdDate >= datetime.strptime(startDate, "%Y-%m-%d")
                )
            if endDate:
                statement = statement.where(
                    ChatMessage.createdDate <= datetime.strptime(endDate, "%Y-%m-%d")
                )

            if keyword:
                statement = statement.where(ChatMessage.content.ilike(f"%{keyword}%"))  # type: ignore

            # Get total count
            count = len(list(session.exec(statement).all()))

            # Apply pagination
            statement = statement.offset((page - 1) * pageSize).limit(pageSize)
            messages = list(session.exec(statement).all())

            return count, messages
