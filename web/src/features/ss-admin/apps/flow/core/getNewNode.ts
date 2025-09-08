import { FlowNodeSettings, ReactFlowNode, RunVariable } from '@/dto'
import {
  NodeType,
  VariableDataType,
  TextProcessType,
  getNodeTypeName,
  OutputFormat,
} from '@/enums'
import { uuid } from '@/utils'

export function getNewNode(
  nodeType: NodeType,
  position?: { x: number; y: number }
): {
  node: ReactFlowNode
  settings: FlowNodeSettings
  inVariables: RunVariable[]
  outVariables: RunVariable[]
} {
  const node = {
    id: uuid(),
    type: nodeType,
    position: position || { x: 500, y: 200 },
  } as ReactFlowNode
  const settings = {
    title: getNodeTypeName(nodeType),
  } as FlowNodeSettings
  const inVariables: RunVariable[] = []
  const outVariables: RunVariable[] = []

  if (nodeType == NodeType.LLM) {
    settings.nodeType = NodeType.LLM
    settings.llmUserPrompt = '{{input}}'
    settings.llmIsReply = true
    settings.llmOutputFormat = OutputFormat.Markdown
    inVariables.push({
      name: 'input',
      dataType: VariableDataType.String,
      isDisabled: true,
      value: '',
    })
    outVariables.push({
      name: 'output',
      dataType: VariableDataType.String,
      isDisabled: true,
      value: '',
    })
  } else if (nodeType == NodeType.WebSearch) {
    settings.nodeType = NodeType.WebSearch
    settings.webSearchCount = 10
    inVariables.push({
      name: 'input',
      dataType: VariableDataType.String,
      isDisabled: true,
      value: '',
    })
    outVariables.push({
      name: 'output',
      dataType: VariableDataType.ArrayObject,
      value: '',
    })
  } else if (nodeType == NodeType.Dataset) {
    settings.nodeType = NodeType.Dataset
    inVariables.push({
      name: 'input',
      dataType: VariableDataType.String,
      isDisabled: true,
      value: '',
    })
    outVariables.push({
      name: 'output',
      dataType: VariableDataType.ArrayObject,
      value: '',
    })
  } else if (nodeType == NodeType.Text) {
    settings.nodeType = NodeType.Text
    settings.textProcessType = TextProcessType.Joint
  } else if (nodeType == NodeType.Sql) {
    settings.nodeType = NodeType.Sql
    outVariables.push({
      key: 'rows',
      name: 'rows',
      dataType: VariableDataType.ArrayObject,
      value: '',
    })
    outVariables.push({
      key: 'count',
      name: 'count',
      dataType: VariableDataType.Integer,
      value: 0,
    })
  } else if (nodeType == NodeType.Intent) {
    settings.nodeType = NodeType.Intent
    settings.intentPrompt = '{{input}}'
    inVariables.push({
      name: 'input',
      dataType: VariableDataType.String,
      value: '',
    })
    outVariables.push({
      key: 'intention',
      name: 'intention',
      dataType: VariableDataType.String,
      value: '',
    })
    outVariables.push({
      key: 'reason',
      name: 'reason',
      dataType: VariableDataType.String,
      value: '',
    })
  } else if (nodeType == NodeType.Ask) {
    settings.nodeType = NodeType.Ask
    outVariables.push({
      key: 'answer',
      name: 'answer',
      dataType: VariableDataType.String,
      value: '',
    })
  }
  return { node, settings, inVariables, outVariables }
}
