import { NodeType } from '@/enums'
import { Handle, Position } from '@xyflow/react'
import NodeBodyVariables from '../components/NodeBodyVariables'
import NodeCard from '../components/NodeCard'
import { useStore } from '../store'

const SqlNode: React.FC<{
  id: string
  isConnectable: boolean
}> = ({ id, isConnectable }) => {
  const store = useStore()
  const settings = store.getNodeSettings(id)
  const inVariables = store.getNodeInVariables(id)
  const outVariables = store.getNodeOutVariables(id)

  const CardBody = () => {
    return settings.sqlQueryString ? (
      <div>
        <NodeBodyVariables title='输入' variables={inVariables} />
        <NodeBodyVariables title='输出' variables={outVariables} />
      </div>
    ) : (
      <div className='placeholder'>请设置 SQL 查询</div>
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
      <NodeCard id={id} type={NodeType.Sql}>
        <CardBody />
      </NodeCard>
    </>
  )
}

export default SqlNode
