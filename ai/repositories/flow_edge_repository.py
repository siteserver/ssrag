from sqlmodel import Session, select, asc, delete
from models import FlowEdge
from utils.db_utils import engine
from utils import string_utils


class FlowEdgeRepository:

    def insert(self, edge: FlowEdge) -> int:
        with Session(engine) as session:
            session.add(edge)
            session.commit()
            session.refresh(edge)
            return edge.id

    def get_all(self, siteId: int) -> list[FlowEdge]:
        if not siteId:
            return []
        statement = (
            select(FlowEdge).where(FlowEdge.siteId == siteId).order_by(asc(FlowEdge.id))
        )
        with Session(engine) as session:
            flow_edges = session.exec(statement).all()
            return list(flow_edges)

    def get_previous_node_guid(self, siteId: int, nodeGuid: str | None) -> str:
        if not siteId or not nodeGuid:
            return ""

        statement = select(FlowEdge).where(
            FlowEdge.siteId == siteId,
            FlowEdge.target == nodeGuid,
        )
        with Session(engine) as session:
            flow_edge = session.exec(statement).first()
            return flow_edge.source if flow_edge else ""

    def get_next_node_uuid(self, siteId: int, nodeUuid: str | None) -> str:
        if siteId == 0 or nodeUuid is None or nodeUuid == "":
            return ""

        statement = select(FlowEdge).where(
            FlowEdge.siteId == siteId,
            FlowEdge.source == nodeUuid,
        )
        with Session(engine) as session:
            flow_edge = session.exec(statement).first()
            return flow_edge.target if flow_edge else ""

    def get_right_edges(self, siteId: int, nodeGuid: str | None) -> list[FlowEdge]:
        if not siteId or not nodeGuid:
            return []

        statement = select(FlowEdge).where(
            FlowEdge.siteId == siteId,
            FlowEdge.source == nodeGuid,
            FlowEdge.sourceHandle.startswith("right"),
        )
        with Session(engine) as session:
            flow_edges = session.exec(statement).all()
            return list(flow_edges)

    def get_bottom_edges(self, siteId: int, nodeGuid: str | None) -> list[FlowEdge]:
        if not siteId or not nodeGuid:
            return []

        statement = select(FlowEdge).where(
            FlowEdge.siteId == siteId,
            FlowEdge.source == nodeGuid,
            FlowEdge.sourceHandle.startswith("bottom"),
        )
        with Session(engine) as session:
            flow_edges = session.exec(statement).all()
            return list(flow_edges)

    def delete_all(self, siteId: int) -> None:
        statement = delete(FlowEdge).where(FlowEdge.siteId == siteId)  # type: ignore
        with Session(engine) as session:
            session.exec(statement)  # type: ignore
            session.commit()

    def delete_all_except(self, siteId: int, exclude_edge_ids: list[int]) -> None:
        statement = delete(FlowEdge).where(  # type: ignore
            FlowEdge.siteId == siteId, FlowEdge.id.not_in(exclude_edge_ids)  # type: ignore
        )
        with Session(engine) as session:
            session.exec(statement)  # type: ignore
            session.commit()
