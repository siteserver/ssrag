import { NodeType } from '@/enums'
import { Handle, Position } from '@xyflow/react'
import NodeCard from '../components/NodeCard'
import { useStore } from '../store'

const IntentNode: React.FC<{
  id: string
  isConnectable: boolean
}> = ({ id, isConnectable }) => {
  const store = useStore()
  const settings = store.getNodeSettings(id)
  const intentions = settings.intentions || []
  // if (intentions.length > 0 && intentions[intentions.length - 1] === '') {
  //   intentions.pop()
  // }

  const getHandleStyle = (count: number) => {
    const elementCard = document.getElementById(`node-${id}`)
    const elementGroup = document.getElementById(`node-${id}-${count}`)
    if (elementCard && elementGroup) {
      const rectCard = elementCard.getBoundingClientRect()
      const rectGroup = elementGroup.getBoundingClientRect()
      const top = rectGroup.top - rectCard.top + rectGroup.height / 2
      return { top: `${top}px` }
    }

    return {}
  }

  const cardStyle = {
    padding: '10px',
    borderRadius: '8px',
    background: '#fcfcff',
    marginBottom: '10px',
    border: '1px solid #ddd',
  }

  return (
    <>
      <Handle
        type='target'
        position={Position.Left}
        id='left'
        isConnectable={isConnectable}
      />

      {intentions.map((_, index) => {
        return (
          <Handle
            key={index}
            type='source'
            position={Position.Right}
            id={`right-${index + 1}`}
            style={getHandleStyle(index + 1)}
            isConnectable={isConnectable}
          />
        )
      })}
      <Handle
        type='source'
        position={Position.Right}
        id='right-0'
        style={getHandleStyle(0)}
        isConnectable={isConnectable}
      />

      <NodeCard id={id} type={NodeType.Intent}>
        {intentions.map((intention, intentionIndex) => (
          <div
            id={`node-${id}-${intentionIndex + 1}`}
            key={intentionIndex}
            style={cardStyle}
          >
            <div style={{ marginBottom: '8px', color: '#999' }}>
              {intentionIndex == 0 ? '如果' : '否则如果'}
            </div>
            <div style={{ padding: '2px' }}>{intention}</div>
          </div>
        ))}

        <div id={`node-${id}-0`} style={cardStyle}>
          <div style={{ color: '#999' }}>否则</div>
        </div>
      </NodeCard>
    </>
  )
}

export default IntentNode
