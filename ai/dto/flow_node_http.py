from models import FlowNode


class FlowNodeHttp:
    def __init__(self, node: FlowNode):
        self.httpMethod: str
        self.httpUrl: str
        self.httpSecurityKey: str

        if node:
            self.load_dict(node.to_dict())

    def load_dict(self, node: dict):
        self.httpMethod = str(node.get("httpMethod", ""))
        self.httpUrl = str(node.get("httpUrl", ""))
        self.httpSecurityKey = str(node.get("httpSecurityKey", ""))
