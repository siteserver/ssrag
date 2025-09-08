import { NodeType } from '@/enums'
import { Handle, Position } from '@xyflow/react'
import NodeCard from '../components/NodeCard'
import { useStore } from '../store'

const AskNode: React.FC<{
  id: string
  isConnectable: boolean
}> = ({ id, isConnectable }) => {
  const store = useStore()
  const settings = store.getNodeSettings(id)
  const options = settings.options || []
  if (options.length > 0 && options[options.length - 1] === '') {
    options.pop()
  }

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

      {options.map((_, index) => {
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

      <NodeCard id={id} type={NodeType.Ask}>
        {options.map((option, optionIndex) => (
          <div
            id={`node-${id}-${optionIndex + 1}`}
            key={optionIndex}
            style={cardStyle}
          >
            <div style={{ marginBottom: '8px', color: '#999' }}>
              {optionIndex == 0 ? '如果' : '否则如果'}
            </div>
            <div style={{ padding: '2px' }}>{option}</div>
          </div>
        ))}

        <div id={`node-${id}-0`} style={cardStyle}>
          <div style={{ color: '#999' }}>否则</div>
        </div>
      </NodeCard>
    </>
  )
}

export default AskNode
