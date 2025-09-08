import { NodeType } from '@/enums'

export function isNodeRunnable(nodeType: NodeType) {
  return (
    nodeType !== NodeType.Start &&
    nodeType !== NodeType.Http &&
    nodeType !== NodeType.Input &&
    nodeType !== NodeType.Output &&
    nodeType !== NodeType.Ask
  )
}
