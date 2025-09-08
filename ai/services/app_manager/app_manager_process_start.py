from dto import RunProcess, RunVariable
from configs import constants


def app_manager_process_start(inVariables: list[RunVariable]) -> RunProcess:
    out_variables = []
    input_var = next(
        (v for v in inVariables if v.name == constants.DEFAULT_INPUT_NAME), None
    )
    out_variables.append(
        RunVariable(
            name=constants.DEFAULT_OUTPUT_NAME,
            value=input_var.value if input_var else "",
        )
    )

    return RunProcess(outVariables=out_variables)
