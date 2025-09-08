import { NodeType } from '@/enums'
import { Handle, Position } from '@xyflow/react'
import NodeCard from '../components/NodeCard'
import { useStore } from '../store'

const HttpNode: React.FC<{
  id: string
  isConnectable: boolean
}> = ({ id, isConnectable }) => {
  const store = useStore()
  const settings = store.getNodeSettings(id)

  const CardBody = () => {
    return settings.httpUrl ? (
      <div>{`${settings.httpMethod} ${settings.httpUrl}`}</div>
    ) : (
      <div className='placeholder'>请设置 HTTP 调用</div>
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
      <NodeCard id={id} type={NodeType.Http}>
        <CardBody />
      </NodeCard>
    </>
  )
}

export default HttpNode
