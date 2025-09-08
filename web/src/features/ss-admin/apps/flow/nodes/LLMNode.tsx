import { Model } from '@/dto'
import { NodeType } from '@/enums'
import { Handle, Position } from '@xyflow/react'
import { Typography, Avatar, Space } from 'antd'
import NodeBodyItem from '../components/NodeBodyItem'
import NodeBodyVariables from '../components/NodeBodyVariables'
import NodeCard from '../components/NodeCard'
import { useStore } from '../store'

const { Text } = Typography

const LLMNode: React.FC<{
  id: string
  isConnectable: boolean
}> = ({ id, isConnectable }) => {
  const store = useStore()
  const settings = store.getNodeSettings(id)
  const inVariables = store.getNodeInVariables(id)
  const outVariables = store.getNodeOutVariables(id)

  let model: Model | null | undefined = store.models.find(
    (model) =>
      model.providerId + ':' + model.modelId === settings.providerModelId
  )
  if (!model) {
    model = store.defaultModel
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
      <NodeCard id={id} type={NodeType.LLM}>
        <NodeBodyVariables title='输入' variables={inVariables} />
        <NodeBodyVariables title='输出' variables={outVariables} />
        {model && (
          <NodeBodyItem key={model.modelId} title='模型'>
            <Space align='center'>
              <Avatar
                src={`/assets/images/providers/${model.providerId}/${model.iconUrl}`}
                size={20}
              />
              <span>{model.providerName}</span>
              <Text strong>{model.modelId}</Text>
            </Space>
          </NodeBodyItem>
        )}
      </NodeCard>
    </>
  )
}

export default LLMNode
