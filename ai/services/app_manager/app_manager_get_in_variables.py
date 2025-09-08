from models import FlowNodeSettings
from dto import RunVariable
from enums import VariableType
from repositories import flow_variable_repository


def app_manager_get_in_variables(
    settings: FlowNodeSettings,
    inVariablesDict: dict[str, list[RunVariable]],
    outVariablesDict: dict[str, list[RunVariable]],
) -> list[RunVariable]:
    inVariables = []

    nodeInVariables = flow_variable_repository.get_variables(
        settings.siteId, settings.id, VariableType.INPUT
    )

    for variable in nodeInVariables:
        value = ""
        if variable.isReference:
            if variable.referenceNodeId in outVariablesDict:
                referenceVariables = outVariablesDict[variable.referenceNodeId]
                referenceVariable = next(
                    (v for v in referenceVariables if v.name == variable.referenceName),
                    None,
                )
                if referenceVariable is not None:
                    value = str(referenceVariable.value)
        else:
            if settings.id is not None:
                requestVariables = inVariablesDict[settings.id]
                requestVariable = next(
                    (v for v in requestVariables if v.name == variable.name), None
                )
                if requestVariable is not None:
                    value = str(requestVariable.value)

        inVariables.append(RunVariable(name=variable.name, value=value))

    return inVariables
