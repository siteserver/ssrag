import { VariableDataType } from '../enums/variableDataType'
import { VariableType } from '../enums/variableType'

export interface FlowVariable {
  id?: number
  nodeId?: string
  type?: VariableType
  key?: string
  name?: string
  dataType?: VariableDataType
  value?: Record<string, unknown> | string | number
  isDisabled?: boolean
  isReference?: boolean
  referenceNodeId?: string
  referenceName?: string
  description?: string
}
