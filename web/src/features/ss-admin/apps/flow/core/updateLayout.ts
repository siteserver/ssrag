import { ReactFlowNode, ReactFlowEdge } from '@/dto'
import { NodeType } from '@/enums/nodeType'

export function updateLayout(
  nodes: ReactFlowNode[],
  edges: ReactFlowEdge[],
  setNodes: (nodes: ReactFlowNode[]) => void
) {
  let newNodes: ReactFlowNode[] = []
  // Create a map to track visited nodes
  const visitedNodes = new Map()
  // Start with input nodes
  const inputNodes = nodes.filter(
    (node) => node.type === NodeType.Start && !node.parentId
  )

  // Add input nodes first
  inputNodes.forEach((node) => {
    if (!visitedNodes.has(node.id)) {
      visitedNodes.set(node.id, true)
      newNodes.push(node)
    }
  })

  // Process remaining nodes based on edge connections
  let lastSize = 0

  // Continue until no more nodes are added in an iteration
  while (lastSize !== visitedNodes.size) {
    lastSize = visitedNodes.size

    edges.forEach((edge) => {
      const sourceId = edge.source
      const targetId = edge.target

      // If source is visited but target is not, add target
      if (visitedNodes.has(sourceId) && !visitedNodes.has(targetId)) {
        const targetNode = nodes.find((node) => node.id === String(targetId))
        if (targetNode) {
          visitedNodes.set(targetId, true)
          newNodes.push(targetNode)
        }
      }
    })
  }

  // Add any remaining nodes that weren't connected
  nodes.forEach((node) => {
    if (!visitedNodes.has(node.id)) {
      newNodes.push(node)
    }
  })

  let index = 0
  let line = 0
  let x = 100
  let y = 100
  newNodes = [...newNodes].map((node) => {
    if (index % 4 === 0) {
      line += 1
      x = 100
      if (line > 1) {
        y += 200
      }
    } else {
      x += 400
    }
    index += 1
    return {
      ...node,
      position: { x: x, y: y },
    }
  })
  setNodes(newNodes)
}
