import { NodeType } from '../enums'

export interface FlowNode {
  id?: string
  uuid: string
  createdDate?: Date
  lastModifiedDate?: Date
  siteId?: number
  nodeId?: number
  parentId?: number
  nodeType?: NodeType
  title?: string
  description?: string
}
