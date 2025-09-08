import { FlowNodeSettings } from './flowNodeSettings'
import { NodeRunning } from './nodeRunning'
import { RunVariable } from './runVariable'

export interface ReactFlowNode {
  id: string
  type: string
  parentId?: string
  parent?: ReactFlowNode
  extent?: 'parent' | undefined
  deletable?: boolean
  data: FlowNodeSettings
  position: { x: number; y: number }
  width?: number
  height?: number
  style?: {
    width?: number
    height?: number
    zIndex?: number
  }
}

export interface ReactFlowNodeData extends Record<string, unknown> {
  inVariables?: RunVariable[]
  outVariables?: RunVariable[]
  running?: NodeRunning
}

export interface ReactFlowEdge {
  id: string
  source: string
  sourceHandle: string
  target: string
  targetHandle: string
}
