import { useEffect } from 'react'
import { FlowNodeSettings } from '@/dto/flowNodeSettings'
import { Typography, Form, FormInstance, Select } from 'antd'
import { useStore } from '../store'

const { Text } = Typography

const ModelSelector: React.FC<{
  nodeId: string
  form: FormInstance<FlowNodeSettings>
}> = ({ nodeId, form }) => {
  const store = useStore()
  const settings = store.getNodeSettings(nodeId)

  useEffect(() => {
    let providerModelId = settings.providerModelId
    if (!providerModelId) {
      if (
        store.defaultModel &&
        store.defaultModel.providerId &&
        store.defaultModel.modelId
      ) {
        providerModelId =
          store.defaultModel.providerId + ':' + store.defaultModel.modelId
      }
    }
    if (providerModelId) {
      form.setFieldsValue({
        providerModelId: providerModelId,
      })
    }
  }, [settings, form, store.models, store.defaultModel])

  return (
    <Form.Item noStyle name='providerModelId'>
      <Select
        placeholder='请选择大模型'
        popupMatchSelectWidth={true}
        variant='filled'
        style={{ width: '100%' }}
        showSearch
        optionFilterProp='value'
        options={store.models.map((model) => ({
          value: model.providerId + ':' + model.modelId,
          label: (
            <div className='flex items-center space-x-2'>
              <img
                src={`/assets/images/providers/${model.providerId}/${model.iconUrl}`}
                alt={model.providerName}
                style={{
                  maxWidth: 'fit-content',
                }}
                className='h-4 w-4 rounded object-contain'
              />
              <span>{model.providerName}</span>
              <Text strong>{model.modelId}</Text>
            </div>
          ),
        }))}
      />
    </Form.Item>
  )
}

export default ModelSelector
