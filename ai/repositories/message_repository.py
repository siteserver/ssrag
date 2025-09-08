from sqlmodel import Session, select, func, desc, text
from models import Message
from utils.db_utils import engine


class MessageRepository:

    def insert(self, message: Message):
        with Session(engine) as session:
            session.add(message)
            session.commit()
            session.refresh(message)
        return message.id

    def get_unread_count(self, user_name: str) -> int:
        statement = select(func.count()).where(
            Message.isRead == False, Message.userName == user_name, Message.dataId > 0
        )
        with Session(engine) as session:
            count = session.exec(statement).one()
        return count

    def get_count_by_user_name(
        self,
        user_name: str,
        keyword: str | None = None,
        is_read: bool | None = None,
        created_at: list[str] | None = None,
    ) -> int:
        with Session(engine) as session:
            statement = select(func.count()).where(
                Message.userName == user_name, Message.dataId > 0
            )
            if keyword:
                statement = statement.where(
                    Message.Title.like(f"%{keyword}%") | Message.Body.like(f"%{keyword}%")  # type: ignore
                )
            if is_read is not None:
                statement = statement.where(Message.isRead == is_read)
            if created_at and len(created_at) == 2:
                statement = statement.where(
                    text(f"createdDate between '{created_at[0]}' and '{created_at[1]}'")
                )
            return session.exec(statement).one()

    def get_all_by_user_name(
        self,
        user_name: str,
        current: int,
        page_size: int,
        keyword: str | None = None,
        is_read: bool | None = None,
        created_at: list[str] | None = None,
    ) -> list[Message]:
        statement = (
            select(Message)
            .where(Message.userName == user_name, Message.dataId > 0)
            .order_by(desc(Message.id))
        )
        if keyword:
            statement = statement.where(
                Message.Title.like(f"%{keyword}%") | Message.Body.like(f"%{keyword}%")  # type: ignore
            )
        if is_read is not None:
            statement = statement.where(Message.isRead == is_read)
        if created_at and len(created_at) == 2:
            statement = statement.where(
                text(f"createdDate between '{created_at[0]}' and '{created_at[1]}'")
            )
        statement = statement.offset((current - 1) * page_size).limit(page_size)
        with Session(engine) as session:
            return list(session.exec(statement).all())

    def delete(self, user_name: str, ids: list[int]):
        with Session(engine) as session:
            statement = select(Message).where(
                Message.userName == user_name, Message.id.in_(ids)  # type: ignore
            )
            messages = session.exec(statement).all()
            for message in messages:
                session.delete(message)
            session.commit()

    def read(self, user_name: str, id: int):
        with Session(engine) as session:
            statement = select(Message).where(
                Message.userName == user_name, Message.id == id
            )
            messages = session.exec(statement).all()
            for message in messages:
                message.isRead = True
                session.commit()

    def read_all(self, user_name: str):
        with Session(engine) as session:
            statement = select(Message).where(
                Message.userName == user_name, Message.isRead == False
            )
            messages = session.exec(statement).all()
            for message in messages:
                message.isRead = True
                session.commit()
