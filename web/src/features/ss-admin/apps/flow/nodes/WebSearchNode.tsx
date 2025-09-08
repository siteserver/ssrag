import { NodeType } from '@/enums'
import { Handle, Position } from '@xyflow/react'
import NodeBodyVariables from '../components/NodeBodyVariables'
import NodeCard from '../components/NodeCard'
import { useStore } from '../store'

const WebSearchNode: React.FC<{
  id: string
  isConnectable: boolean
}> = ({ id, isConnectable }) => {
  const store = useStore()
  const settings = store.getNodeSettings(id)
  const inVariables = store.getNodeInVariables(id)
  const outVariables = store.getNodeOutVariables(id)

  const CardBody = () => {
    return settings.webSearchApiKey ? (
      <div>
        <NodeBodyVariables title='输入' variables={inVariables} />
        <NodeBodyVariables title='输出' variables={outVariables} />
      </div>
    ) : (
      <div className='placeholder'>请设置联网搜索</div>
    )
  }

  return (
    <>
      <Handle
        type='target'
        position={Position.Left}
        id='left'
        isConnectable={isConnectable}
      />
      <Handle
        type='source'
        position={Position.Right}
        id='right'
        isConnectable={isConnectable}
      />
      <NodeCard id={id} type={NodeType.WebSearch}>
        <CardBody />
      </NodeCard>
    </>
  )
}

export default WebSearchNode
