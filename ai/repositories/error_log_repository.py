from sqlmodel import Session
from models import ErrorLog
from utils.db_utils import engine
import uuid


class ErrorLogRepository:

    def insert(self, log: ErrorLog) -> int:
        with Session(engine) as session:
            session.add(log)
            session.commit()
            session.refresh(log)
            return log.id

    def add_error_log(self, log: ErrorLog) -> int:
        try:
            log.category = "Python"
            return self.insert(log)
        except:
            return 0

    def add(self, message: str, summary: str, stackTrace: str) -> int:
        return self.add_error_log(
            ErrorLog(
                uuid=str(uuid.uuid4()),
                category="Python",
                pluginId="",
                message=message,
                summary=summary,
                stackTrace=stackTrace,
            )
        )
