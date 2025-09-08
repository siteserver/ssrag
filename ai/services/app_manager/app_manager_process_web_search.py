from dto import RunProcess, RunVariable
from models import FlowNodeSettings
from configs.constants import DEFAULT_INPUT_NAME, DEFAULT_OUTPUT_NAME
from utils import bocha_utils


def app_manager_process_web_search(
    settings: FlowNodeSettings, inVariables: list[RunVariable]
) -> RunProcess:
    out_variables = []
    response = None

    query = ""
    for variable in inVariables:
        if variable.name == DEFAULT_INPUT_NAME:
            query = str(variable.value)

    response = bocha_utils.bocha_websearch_tool(
        api_key=settings.webSearchApiKey,
        query=query,
        freshness=settings.webSearchFreshness,
        summary=settings.webSearchSummary,
        include=settings.webSearchInclude,
        exclude=settings.webSearchExclude,
        count=settings.webSearchCount,
    )
    if response.success:
        out_variables.append(
            RunVariable(
                name=DEFAULT_OUTPUT_NAME,
                value=response.webPages,
            )
        )
    else:
        raise Exception(response.errorMessage)

    return RunProcess(outVariables=out_variables, response=None)
