import { NodeType } from '@/enums'
import { Handle, Position } from '@xyflow/react'
import NodeBodyVariables from '../components/NodeBodyVariables'
import NodeCard from '../components/NodeCard'
import { useStore } from '../store'

const StartNode: React.FC<{
  id: string
  isConnectable: boolean
}> = ({ id, isConnectable }) => {
  const store = useStore()
  const inVariables = store.getNodeInVariables(id)
  const outVariables = store.getNodeOutVariables(id)

  return (
    <>
      <Handle
        type='source'
        position={Position.Right}
        id='right'
        isConnectable={isConnectable}
      />
      <NodeCard id={id} type={NodeType.Start}>
        <NodeBodyVariables title='输入' variables={inVariables} />
        <NodeBodyVariables title='输出' variables={outVariables} />
      </NodeCard>
    </>
  )
}

export default StartNode
