from models.flow_node import FlowNode, FlowNodeSettings


def app_manager_get_flow_node_settings(flow_node: FlowNode) -> FlowNodeSettings:
    settings = FlowNodeSettings.create(flow_node)

    if settings.positionX == 0:
        settings.positionX = 500
    if settings.positionY == 0:
        settings.positionY = 200

    return settings
