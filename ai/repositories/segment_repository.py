from sqlmodel import Session, select, asc, delete, func
from utils.db_utils import engine
from models import Segment
from utils import string_utils


class SegmentRepository:

    def get_segment(self, segmentId: str) -> Segment | None:
        with Session(engine) as session:
            query = select(Segment).where(Segment.id == segmentId)
            segment = session.exec(query).first()
        return segment

    def get_segment_ids_by_document_ids(self, documentIds: list[int]) -> list[str]:
        with Session(engine) as session:
            query = select(Segment.id).where(Segment.documentId.in_(documentIds))  # type: ignore
            segment_ids = session.exec(query).all()
        return list(segment_ids)

    def get_all_by_document_id(self, documentId: int) -> list[Segment]:
        with Session(engine) as session:
            query = (
                select(Segment)
                .where(Segment.documentId == documentId)
                .order_by(asc(Segment.taxis))
            )
            segments = session.exec(query).all()
        return list(segments)

    def get_all_by_document_id_and_page(
        self, documentId: int, page: int = 1, page_size: int = 20
    ) -> list[Segment]:
        with Session(engine) as session:
            query = (
                select(Segment)
                .where(
                    Segment.documentId == documentId,
                )
                .order_by(asc(Segment.taxis))
                .offset((page - 1) * page_size)
                .limit(page_size)
            )
            segments = session.exec(query).all()

        return list(segments)

    def insert(
        self,
        segment: Segment,
    ):
        segment.textHash = string_utils.generate_hash(segment.text)

        with Session(engine) as session:
            session.add(segment)
            session.commit()
            session.refresh(segment)

        return segment

    def update_content(
        self,
        segmentId: str,
        text: str,
    ):
        db_segment = self.get_segment(segmentId)
        if db_segment:
            db_segment.text = text
            db_segment.textHash = string_utils.generate_hash(text)

            with Session(engine) as session:
                session.add(db_segment)
                session.commit()
                session.refresh(db_segment)

        return db_segment

    def update_all_taxis(self, documentId: int, taxis: int):
        with Session(engine) as session:
            statement = select(Segment).where(
                Segment.documentId == documentId,
                Segment.taxis > taxis,
            )
            segments = session.exec(statement).all()
            for segment in segments:
                segment.taxis += 1
                session.add(segment)
            session.commit()

    def delete_by_ids(self, segmentIds: list[str]):
        with Session(engine) as session:
            stmt = delete(Segment).where(Segment.id.in_(segmentIds))  # type: ignore
            session.exec(stmt)  # type: ignore
            session.commit()

    def delete_by_site_id(self, siteId: int):
        with Session(engine) as session:
            stmt = delete(Segment).where(Segment.siteId == siteId)  # type: ignore
            session.exec(stmt)  # type: ignore
            session.commit()

    def delete_by_channel_id(self, siteId: int, channelId: int):
        with Session(engine) as session:
            stmt = delete(Segment).where(Segment.siteId == siteId, Segment.channelId == channelId)  # type: ignore
            session.exec(stmt)  # type: ignore
            session.commit()

    def delete_by_content_id(self, siteId: int, channelId: int, contentId: int):
        with Session(engine) as session:
            stmt = delete(Segment).where(Segment.siteId == siteId, Segment.channelId == channelId, Segment.contentId == contentId)  # type: ignore
            session.exec(stmt)  # type: ignore
            session.commit()

    def delete_by_document_id(self, documentId: int):
        with Session(engine) as session:
            stmt = delete(Segment).where(Segment.documentId == documentId)  # type: ignore
            session.exec(stmt)  # type: ignore
            session.commit()

    def count(self, documentId: int):
        with Session(engine) as session:
            query = select(func.count(Segment.id)).where(  # type: ignore
                Segment.documentId == documentId,
            )
            results = session.exec(query).first()
        return results or 0
