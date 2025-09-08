from dto import RunProcess, RunVariable
from models import FlowNodeSettings
from configs.constants import DEFAULT_INPUT_NAME, DEFAULT_OUTPUT_NAME
from utils import string_utils
from repositories import dataset_repository
from vectors import Vector


def app_manager_process_dataset(
    vector: Vector,
    settings: FlowNodeSettings,
    inVariables: list[RunVariable],
) -> RunProcess:
    out_variables = []

    query = ""
    for variable in inVariables:
        if variable.name == DEFAULT_INPUT_NAME:
            query = str(variable.value)

    searchScope = dataset_repository.get_search_scope(settings.siteId, settings.id)
    results = vector.search(
        searchScope,
        query,
        settings.datasetSearchType,
        settings.datasetMaxCount,
        settings.datasetMinScore,
    )

    markdownList = []
    for result in results:
        markdownList.append(
            f"[{result.docName}](ssrag://doc-{result.id})\n{result.text}"
        )

    # [产品说明书.pdf](ssrag://doc-123)
    # 我们的产品支持多语言翻译，包含20+语种...
    # ---
    # [FAQ.md](ssrag://doc-456)
    # 常见问题解答：退货流程需7个工作日...

    out_variables.append(
        RunVariable(
            name=DEFAULT_OUTPUT_NAME,
            value=string_utils.join(markdownList, "\n---\n"),
        )
    )

    return RunProcess(outVariables=out_variables, response=None)
