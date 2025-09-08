import { CloseOutlined } from '@ant-design/icons'
import {
  BaseEdge,
  useReactFlow,
  EdgeLabelRenderer,
  getBezierPath,
  Position,
} from '@xyflow/react'

const CustomEdge: React.FC<{
  id: string
  sourceX: number
  sourceY: number
  targetX: number
  targetY: number
  sourcePosition: Position
  targetPosition: Position
}> = ({
  id,
  sourceX,
  sourceY,
  targetX,
  targetY,
  sourcePosition,
  targetPosition,
}) => {
  const { setEdges } = useReactFlow()

  const [edgePath, labelX, labelY] = getBezierPath({
    sourceX,
    sourceY,
    sourcePosition,
    targetX,
    targetY,
    targetPosition,
  })
  return (
    <>
      <BaseEdge id={id} path={edgePath} />
      <EdgeLabelRenderer>
        <CloseOutlined
          style={{
            position: 'absolute',
            borderRadius: '50%',
            padding: '2px',
            border: '1px solid #666',
            backgroundColor: '#fff',
            fontSize: '9px',
            transform: `translate(-50%, -50%) translate(${labelX}px, ${labelY}px)`,
            pointerEvents: 'all',
            color: '#666',
          }}
          className='nodrag nopan'
          onClick={() => setEdges((edges) => edges.filter((e) => e.id !== id))}
        />
      </EdgeLabelRenderer>
    </>
  )
}

export default CustomEdge
