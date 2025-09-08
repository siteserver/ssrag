import { ReactFlowProvider } from '@xyflow/react'
import '@xyflow/react/dist/style.css'
import '@/assets/github-markdown.css'
import FlowNodes from './FlowNodes'
import './app.css'

const Flow: React.FC = () => {
  return (
    <ReactFlowProvider>
      <div className='flow-nodes'>
        <FlowNodes />
      </div>
    </ReactFlowProvider>
  )
}

export default Flow
