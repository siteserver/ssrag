from dto import RunProcess, RunVariable
from models import FlowNodeSettings
from utils import string_utils
from services import llm_manager


def app_manager_process_intent(
    settings: FlowNodeSettings, inVariables: list[RunVariable]
) -> RunProcess:
    out_variables = []

    intentions = settings.intentions
    if not intentions or len(intentions) == 0:
        intentions = ["其他"]
    if not intentions[-1] == "其他":
        intentions[-1] = "其他"

    dict = {}
    for variable in inVariables:
        dict[variable.name] = variable.value
    text = string_utils.parse_jinja2(settings.intentPrompt or "{{input}}", dict)

    provider_id, model_id = string_utils.extract_provider_model_id(
        settings.providerModelId
    )
    if not provider_id or not model_id:
        raise Exception("Provider model id is required")

    intent = llm_manager.intent(provider_id, model_id, intentions, text)
    if not intent:
        raise Exception(f"Intent not found for {text}")

    out_variables.append(RunVariable(name="intention", value=intent.intention))
    out_variables.append(RunVariable(name="reason", value=intent.reason))

    return RunProcess(outVariables=out_variables, response=None)
