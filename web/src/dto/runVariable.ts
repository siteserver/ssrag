import { VariableDataType, VariableType } from '../enums'

export interface RunVariable {
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

  // key?: string
  // type?: VariableType
  // name?: string
  // dataType?: VariableDataType
  description?: string
  // isDisabled?: boolean
  // isReference?: boolean
  // referenceNodeId?: string
  // referenceName?: string
  // value?: string | number | boolean | object
}
