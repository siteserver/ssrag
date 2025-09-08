from sqlmodel import Session, text, insert, delete, update
from utils.db_utils import engine
from models import CeleryTask
from configs import table_names
import uuid
from datetime import datetime, timedelta
from enums import TaskStatus


class CeleryTaskRepository:
    def get_pending_count(self, siteId: int) -> int:
        with engine.connect() as conn:
            result = conn.execute(
                text(
                    f"""
                    SELECT COUNT(*)
                    FROM "{table_names.TABLE_NAME_CELERY_TASK}"
                    WHERE "SiteId" = :siteId
                    AND "TaskStatus" = :taskStatus
                    """
                ),
                {"siteId": siteId, "taskStatus": TaskStatus.PENDING},
            )
            return result.fetchone()[0]  # type: ignore

    def get_pending_tasks(self, siteId: int, limit: int) -> list[CeleryTask]:
        with engine.connect() as conn:
            result = conn.execute(
                text(
                    f"""
SELECT "Id", "Uuid", "CreatedDate", "LastModifiedDate", "SiteId", "ChannelId", "ContentId", "TaskId", "TaskStatus", "TaskResult"
FROM "{table_names.TABLE_NAME_CELERY_TASK}"
WHERE "SiteId" = :siteId
AND "TaskStatus" = :taskStatus
ORDER BY "Id"
LIMIT :limit
                    """
                ),
                {"siteId": siteId, "taskStatus": TaskStatus.PENDING, "limit": limit},
            )
            tasks = [
                CeleryTask(
                    id=row[0],
                    uuid=row[1],
                    createdDate=row[2],
                    lastModifiedDate=row[3],
                    siteId=row[4],
                    channelId=row[5],
                    contentId=row[6],
                    taskId=row[7],
                    taskStatus=row[8],
                    taskResult=row[9],
                )
                for row in result.fetchall()
            ]

            return tasks

    def update_status(
        self,
        siteId: int,
        taskId: str,
        taskStatus: TaskStatus,
        taskResult: str | None = None,
    ) -> None:
        with Session(engine) as session:
            stmt = (
                update(CeleryTask)
                .where(CeleryTask.siteId == siteId)  # type: ignore
                .where(CeleryTask.taskId == taskId)  # type: ignore
                .values(
                    taskStatus=taskStatus,
                    taskResult=taskResult,
                    lastModifiedDate=datetime.now(),
                )
            )
            session.execute(stmt)
            session.commit()

    def insert(
        self,
        siteId: int,
        channelId: int,
        contentId: int,
        taskId: str,
    ) -> None:
        with Session(engine) as session:
            stmt = insert(CeleryTask).values(
                uuid=str(uuid.uuid4()),
                createdDate=datetime.now(),
                siteId=siteId,
                channelId=channelId,
                contentId=contentId,
                taskId=taskId,
                taskStatus=TaskStatus.PENDING,
            )
            session.execute(stmt)
            session.commit()

    def delete_all(self, siteId: int) -> None:
        with Session(engine) as session:
            stmt = delete(CeleryTask).where(CeleryTask.siteId == siteId)  # type: ignore
            session.execute(stmt)
            session.commit()

    def delete_10_minutes_ago(self, siteId: int) -> None:
        with Session(engine) as session:
            stmt = (
                delete(CeleryTask)
                .where(CeleryTask.siteId == siteId)  # type: ignore
                .where(
                    CeleryTask.taskStatus == TaskStatus.FAILURE  # type: ignore
                    or CeleryTask.taskStatus == TaskStatus.SUCCESS  # type: ignore
                )
                .where(
                    CeleryTask.createdDate
                    < datetime.now() - timedelta(minutes=10)  # type: ignore
                )
            )
            session.execute(stmt)
            session.commit()
