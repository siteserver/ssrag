import { TipsJinja2 } from '@/config'
import { Row, Col, Form, Select, Typography, FormInstance } from 'antd'
import AdaptiveHeightEditor from '@/components/adaptive-height-editor'
import { useStore } from '../store'

const { Text } = Typography

const SettingsAI: React.FC<{
  form: FormInstance
}> = ({ form }) => {
  const store = useStore()

  return (
    <Row>
      <Col span={24}>
        <Form.Item
          name='providerModelId'
          label='AI 大模型'
          tooltip='向模型提供用户指令，如查询或任何基于文本输入的提问'
          rules={[{ required: true, message: '请选择 AI 大模型' }]}
        >
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
        <Form.Item
          name='llmSystemPrompt'
          label='提示词'
          tooltip='为对话提供系统级指导，如设定人设和回复逻辑'
        >
          <AdaptiveHeightEditor
            language='markdown'
            height='100px'
            value={store.values.llmSystemPrompt || ''}
            onChange={(value) => {
              form.setFieldsValue({ llmSystemPrompt: value })
            }}
          />
          <div className='tips'>{`为对话提供系统级指导，如设定人设和回复逻辑，${TipsJinja2}`}</div>
        </Form.Item>
      </Col>
    </Row>
  )
}

export default SettingsAI
