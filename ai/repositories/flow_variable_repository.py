from sqlmodel import Session, select, asc, delete
from models import FlowVariable
from utils.db_utils import engine
from enums import VariableType
from utils import string_utils


class FlowVariableRepository:

    def insert(self, variable: FlowVariable):
        with Session(engine) as session:
            session.add(variable)
            session.commit()
            session.refresh(variable)
        return variable.id

    def update(self, variable: FlowVariable):
        with Session(engine) as session:
            db_variable = session.get(FlowVariable, variable.id)
            if db_variable:
                for key, value in variable.dict().items():
                    setattr(db_variable, key, value)
                session.commit()
                session.refresh(db_variable)
                return db_variable
            return None

    def get(self, siteId: int, id: int) -> FlowVariable | None:
        if not siteId or id == 0:
            return None

        with Session(engine) as session:
            statement = select(FlowVariable).where(
                FlowVariable.siteId == siteId, FlowVariable.id == id
            )
            return session.exec(statement).first()

    def get_global_variables(self, siteId: int) -> list[FlowVariable]:
        with Session(engine) as session:
            statement = select(FlowVariable).where(
                FlowVariable.siteId == siteId, FlowVariable.type == VariableType.GLOBAL
            )
            statement = statement.order_by(asc(FlowVariable.id))
            return list(session.exec(statement).all())

    def get_variables(
        self, siteId: int, nodeGuid: str, variableType: VariableType
    ) -> list[FlowVariable]:
        with Session(engine) as session:
            statement = select(FlowVariable).where(
                FlowVariable.siteId == siteId,
                FlowVariable.nodeId == nodeGuid,
                FlowVariable.type == variableType,
            )
            statement = statement.order_by(asc(FlowVariable.id))
            return list(session.exec(statement).all())

    def delete_all(self, siteId: int):
        with Session(engine) as session:
            statement = delete(FlowVariable).where(FlowVariable.siteId == siteId)  # type: ignore
            session.exec(statement)  # type: ignore
            session.commit()

    def delete_all_by_node(self, siteId: int, nodeGuid: str):
        with Session(engine) as session:
            statement = delete(FlowVariable).where(
                FlowVariable.siteId == siteId, FlowVariable.nodeId == nodeGuid  # type: ignore
            )  # type: ignore
            session.exec(statement)  # type: ignore
            session.commit()

    def delete_all_except(self, siteId: int, excludeVariableIds: list[int]):
        with Session(engine) as session:
            statement = delete(FlowVariable).where(
                FlowVariable.siteId == siteId,  # type: ignore
                FlowVariable.id.not_in(excludeVariableIds),  # type: ignore
            )  # type: ignore
            session.exec(statement)  # type: ignore
            session.commit()
