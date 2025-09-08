import { ReactFlowEdge } from '@/dto'
import { FlowEdge } from '@/models'

export function getEdgesByRes(res: { edges: FlowEdge[] }): ReactFlowEdge[] {
  return res.edges.map((edge) => {
    return {
      id: edge.uuid + '-' + edge.id,
      type: 'custom-edge',
      source: edge.source,
      sourceHandle: edge.sourceHandle,
      target: edge.target,
      targetHandle: edge.targetHandle,
    } as ReactFlowEdge
  })
}
