from .run_variable import RunVariable
from fastapi.responses import StreamingResponse


class RunProcess:
    def __init__(
        self,
        outVariables: list[RunVariable] = [],
        response: StreamingResponse | None = None,
    ):
        self.outVariables: list[RunVariable] = outVariables
        self.response: StreamingResponse | None = response
