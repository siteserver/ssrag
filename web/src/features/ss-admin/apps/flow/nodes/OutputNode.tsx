import { NodeType } from '@/enums'
import { Handle, Position } from '@xyflow/react'
import NodeBodyVariables from '../components/NodeBodyVariables'
import NodeCard from '../components/NodeCard'
import { useStore } from '../store'

const OutputNode: React.FC<{
  id: string
  isConnectable: boolean
}> = ({ id, isConnectable }) => {
  const store = useStore()
  const settings = store.getNodeSettings(id)
  const inVariables = store.getNodeInVariables(id)
  const outVariables = store.getNodeOutVariables(id)

  return (
    <>
      <Handle
        type='target'
        position={Position.Left}
        id='left'
        isConnectable={isConnectable}
      />
      {!settings.isFixed && (
        <Handle
          type='source'
          position={Position.Right}
          id='right'
          isConnectable={isConnectable}
        />
      )}
      <NodeCard id={id} type={NodeType.Output}>
        <NodeBodyVariables title='输入' variables={inVariables} />
        <NodeBodyVariables title='输出' variables={outVariables} />
      </NodeCard>
    </>
  )
}

export default OutputNode
