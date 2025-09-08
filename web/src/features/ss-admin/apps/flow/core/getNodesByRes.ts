import { ReactFlowNode } from '@/dto'
import { FlowNodeSettings } from '@/dto'
import { NodeType } from '@/enums'

// import { getInVariables } from './getInVariables'
// import { getOutVariables } from './getOutVariables'

export const getNodesByRes = (res: { nodes: FlowNodeSettings[] }) => {
  return res.nodes.map((settings) => {
    // const node = {
    //   ...settings,
    //   inVariables: getInVariables(settings.inVariables),
    //   outVariables: getOutVariables(settings.outVariables),
    //   running: {
    //     ...(settings.running || {}),
    //     isRun: false,
    //     isRunning: false,
    //   },
    // } as FlowNodeSettings
    const reactFlowNode = {
      id: settings.id,
      type: settings.nodeType,
      deletable: !(settings.nodeType == NodeType.Input && settings.isFixed),
      position: {
        x: settings.positionX,
        y: settings.positionY,
      },
      // data: node,
    } as ReactFlowNode
    return reactFlowNode
  })
}
