from dto import RunProcess, RunVariable
from models import FlowNodeSettings
from configs import constants
from ..llm_manager import LLMManager


def app_manager_process_llm(
    settings: FlowNodeSettings, inVariables: list[RunVariable], thinking: bool
) -> RunProcess:
    out_variables = []
    response = None

    llm_manager = LLMManager()

    if settings.llmIsReply:
        response = llm_manager.run_stream(settings, inVariables, thinking)
    else:
        result = llm_manager.run_invoke(settings, inVariables)
        content = str(result)
        out_variables.append(
            RunVariable(name=constants.DEFAULT_OUTPUT_NAME, value=content)
        )

    return RunProcess(outVariables=out_variables, response=response)
