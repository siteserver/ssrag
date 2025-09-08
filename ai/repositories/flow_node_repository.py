from sqlmodel import Session, select, asc, delete
from models import FlowNode, FlowNodeSettings
from enums import NodeType
from utils import string_utils
from utils.db_utils import engine


class FlowNodeRepository:

    def insert(self, uuid: str, settings: FlowNodeSettings) -> int:
        extend_values = settings.to_extend_values()
        flow_node = FlowNode(
            uuid=uuid,
            siteId=settings.siteId,
            nodeType=settings.nodeType or "",
            title=settings.title or "",
            description=settings.description or "",
            extendValues=string_utils.to_json_str(extend_values),
        )
        with Session(engine) as session:
            session.add(flow_node)
            session.commit()
            session.refresh(flow_node)
            return flow_node.id

    def update(self, settings: FlowNodeSettings) -> None:
        extend_values = settings.to_extend_values()
        flow_node = self.get(settings.siteId, settings.nodeId)
        if not flow_node:
            return

        flow_node.nodeType = settings.nodeType or ""
        flow_node.title = settings.title or ""
        flow_node.description = settings.description or ""
        flow_node.extendValues = string_utils.to_json_str(extend_values)
        with Session(engine) as session:
            session.add(flow_node)
            session.commit()
            session.refresh(flow_node)

    def get_start_node(self, siteId: int) -> FlowNode | None:
        if not siteId:
            return None

        statement = select(FlowNode).where(
            FlowNode.siteId == siteId,
            FlowNode.nodeType == NodeType.START,
        )
        with Session(engine) as session:
            flow_node = session.exec(statement).first()
        return flow_node

    def get(self, siteId: int, nodeId: int | None) -> FlowNode | None:
        if not siteId or nodeId is None or nodeId == 0:
            return None

        statement = select(FlowNode).where(
            FlowNode.siteId == siteId,
            FlowNode.id == nodeId,
        )
        with Session(engine) as session:
            flow_node = session.exec(statement).first()
        return flow_node

    def get_by_uuid(self, siteId: int, nodeUuid: str | None) -> FlowNode | None:
        if not siteId or not nodeUuid:
            return None

        statement = select(FlowNode).where(
            FlowNode.siteId == siteId,
            FlowNode.uuid == nodeUuid,
        )
        with Session(engine) as session:
            flow_node = session.exec(statement).first()
        return flow_node

    def get_all(self, siteId: int) -> list[FlowNode]:
        statement = (
            select(FlowNode)
            .where(
                FlowNode.siteId == siteId,
            )
            .order_by(asc(FlowNode.id))
        )

        with Session(engine) as session:
            flow_nodes = session.exec(statement).all()
            return list(flow_nodes)

    def delete_all(self, siteId: int) -> None:
        statement = delete(FlowNode).where(FlowNode.siteId == siteId)  # type: ignore
        with Session(engine) as session:
            session.exec(statement)  # type: ignore
            session.commit()

    def delete_all_except(self, siteId: int, exclude_node_ids: list[int]) -> None:
        statement = delete(FlowNode).where(  # type: ignore
            FlowNode.siteId == siteId, FlowNode.id.not_in(exclude_node_ids)  # type: ignore
        )
        with Session(engine) as session:
            session.exec(statement)  # type: ignore
            session.commit()
