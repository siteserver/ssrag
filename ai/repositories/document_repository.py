from sqlmodel import Session, select, delete, desc, func
from .channel_repository import ChannelRepository
from utils import string_utils
from dto import ChunkConfig
from models import Document
from utils.db_utils import engine


class DocumentRepository:
    def get_document_ids(self, siteId: int, channelId: int, contentId: int):
        with Session(engine) as session:
            query = select(Document.id).where(
                Document.siteId == siteId,
                Document.channelId == channelId,
                Document.contentId == contentId,
            )
            document_ids = session.exec(query).all()
        return list(document_ids)

    def get_all(self):
        with Session(engine) as session:
            query = select(Document)
            documents = session.exec(query).all()
        return list(documents)

    def get_total_count_by_site_id(self, siteId: int) -> int:
        return self.get_total_count(siteId, 0, 0, "")

    def get_total_count(
        self, siteId: int, channelId: int, contentId: int, search: str = ""
    ) -> int:
        count = 0
        with Session(engine) as session:
            query = select(func.count(Document.id))  # type: ignore
            if siteId != 0:
                query = query.where(Document.siteId == siteId)
            if channelId != 0:
                query = query.where(Document.channelId == channelId)
            if contentId != 0:
                query = query.where(Document.contentId == contentId)
            if search:
                query = query.where(Document.title.like(f"%{search}%"))  # type: ignore
            count = session.exec(query).first()
        if count is None:
            return 0
        return count

    def get_documents(
        self,
        siteId: int,
        channelId: int,
        contentId: int,
        search: str = "",
        offset: int = 0,
        limit: int = 30,
    ):
        with Session(engine) as session:
            query = (
                select(Document).order_by(desc(Document.id)).offset(offset).limit(limit)
            )
            if siteId != 0:
                query = query.where(Document.siteId == siteId)
            if channelId != 0:
                channel_repository = ChannelRepository()
                channelIds = channel_repository.get_channel_ids(channelId)
                channelIds.append(channelId)
                query = query.where(Document.channelId.in_(channelIds))  # type: ignore
            if contentId != 0:
                query = query.where(Document.contentId == contentId)
            if search:
                query = query.where(Document.title.like(f"%{search}%"))  # type: ignore
            documents = session.exec(query).all()
        return list(documents)

    def get_documents_by_document_ids(self, documentIds: list):
        with Session(engine) as session:
            query = select(Document).where(
                Document.id.in_(documentIds),  # type: ignore
            )
            documents = session.exec(query).all()
        return list(documents)

    def get_document(self, documentId: int):
        with Session(engine) as session:
            query = select(Document).where(Document.id == documentId)
            document = session.exec(query).first()
        return document

    def get_title(self, documentId: int):
        with Session(engine) as session:
            query = select(Document.title).where(Document.id == documentId)
            title = session.exec(query).first()
        return title

    def insert(self, document: Document):
        with Session(engine) as session:
            session.add(document)
            session.commit()
            session.refresh(document)
        return document

    def update(self, document: Document):
        db_document = self.get_document(document.id)
        if db_document:
            db_document.lastModifiedDate = document.lastModifiedDate
            db_document.title = document.title
            db_document.fileName = document.fileName
            db_document.extName = document.extName
            db_document.fileSize = document.fileSize
            db_document.separators = document.separators
            db_document.chunkSize = document.chunkSize
            db_document.chunkOverlap = document.chunkOverlap
            db_document.isChunkReplaces = document.isChunkReplaces
            db_document.isChunkDeletes = document.isChunkDeletes

            with Session(engine) as session:
                session.add(db_document)
                session.commit()
                session.refresh(db_document)
        return db_document

    def update_config(self, document_id: int, config: ChunkConfig):
        db_document = self.get_document(document_id)
        if db_document:
            db_document.separators = string_utils.join(config.separators, ",")
            db_document.chunkSize = config.chunkSize
            db_document.chunkOverlap = config.chunkOverlap
            db_document.isChunkReplaces = config.isChunkReplaces
            db_document.isChunkDeletes = config.isChunkDeletes
            with Session(engine) as session:
                session.add(db_document)
                session.commit()
                session.refresh(db_document)
        return db_document

    def delete_by_id(self, document_id: int):
        with Session(engine) as session:
            stmt = delete(Document).where(Document.id == document_id)  # type: ignore
            session.exec(stmt)  # type: ignore
            session.commit()

    def delete_by_ids(self, document_ids: list[int]):
        with Session(engine) as session:
            stmt = delete(Document).where(Document.id.in_(document_ids))  # type: ignore
            session.exec(stmt)  # type: ignore
            session.commit()

    def delete_by_site_id(self, siteId: int):
        with Session(engine) as session:
            stmt = delete(Document).where(Document.siteId == siteId)  # type: ignore
            session.exec(stmt)  # type: ignore
            session.commit()

    def delete_by_channel_id(self, siteId: int, channelId: int):
        with Session(engine) as session:
            stmt = delete(Document).where(Document.siteId == siteId, Document.channelId == channelId)  # type: ignore
            session.exec(stmt)  # type: ignore
            session.commit()

    def delete_by_content_id(self, siteId: int, channelId: int, contentId: int):
        with Session(engine) as session:
            stmt = delete(Document).where(Document.siteId == siteId, Document.channelId == channelId, Document.contentId == contentId)  # type: ignore
            session.exec(stmt)  # type: ignore
            session.commit()
