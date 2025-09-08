import { useEffect, useState } from 'react'
import { FlowNodeSettings } from '@/dto/flowNodeSettings'
import { QuestionCircleOutlined } from '@ant-design/icons'
import {
  Typography,
  Form,
  FormInstance,
  Radio,
  Slider,
  InputNumber,
  Tooltip,
  Row,
  Col,
  Divider,
} from 'antd'
import { useStore } from '../store'

const { Text } = Typography

const ModelSettings: React.FC<{
  nodeId: string
  form: FormInstance<FlowNodeSettings>
}> = ({ nodeId, form }) => {
  const store = useStore()
  const settings = store.getNodeSettings(nodeId)
  const [diversityMode, setDiversityMode] = useState('precise')

  useEffect(() => {
    if (settings.modelTemperature == 0.1 && settings.modelTopP == 0.7) {
      setDiversityMode('precise')
    } else if (settings.modelTemperature == 1 && settings.modelTopP == 0.7) {
      setDiversityMode('balanced')
    } else if (settings.modelTemperature == 1 && settings.modelTopP == 0.8) {
      setDiversityMode('creative')
    } else {
      setDiversityMode('custom')
    }

    form.setFieldsValue({
      modelTemperature: settings.modelTemperature || 0.1,
      modelTopP: settings.modelTopP || 0.7,
      modelMaxResponseLength: settings.modelMaxResponseLength || 1024,
    })
  }, [settings, form])

  return (
    <div
      style={{
        padding: '12px 24px',
        display: 'flex',
        flexDirection: 'column',
        gap: '16px',
      }}
    >
      <Row align='middle' gutter={4}>
        <Col span={6}>
          <Text strong>生成多样性</Text>
        </Col>
        <Col span={18}>
          <Radio.Group
            value={diversityMode}
            onChange={(e) => {
              setDiversityMode(e.target.value)
              if (e.target.value === 'precise') {
                form.setFieldsValue({ modelTemperature: 0.1, modelTopP: 0.7 })
              } else if (e.target.value === 'balanced') {
                form.setFieldsValue({ modelTemperature: 1, modelTopP: 0.7 })
              } else if (e.target.value === 'creative') {
                form.setFieldsValue({ modelTemperature: 1, modelTopP: 0.8 })
              }
            }}
          >
            <Tooltip title='严格遵循指令生成内容，适用于需准确无误的场合，如正式文档、代码等'>
              <Radio.Button value='precise'>精确模式</Radio.Button>
            </Tooltip>
            <Tooltip title='在创新和精确之间寻求平衡，适用于大多数日常应用场景，生成有趣但不失严谨的内容'>
              <Radio.Button value='balanced'>平衡模式</Radio.Button>
            </Tooltip>
            <Tooltip title='激发创意，提供新颖独特的想法，适合需要灵感和独特观点的场景，如头脑风暴、创意写作等'>
              <Radio.Button value='creative'>创意模式</Radio.Button>
            </Tooltip>
            <Tooltip title='根据需求，进行精细调整，实现个性化优化'>
              <Radio.Button value='custom'>自定义</Radio.Button>
            </Tooltip>
          </Radio.Group>
        </Col>
      </Row>
      <Row align='middle' gutter={4}>
        <Col span={6}>
          <Text>
            生成随机性
            <Tooltip title='temperature: 调高温度会使得模型的输出更多样性和创新性，反之，降低温度会使输出内容更加遵循指令要求但减少多样性。建议不要与 "Top p" 同时调整。'>
              <QuestionCircleOutlined
                style={{ marginLeft: 8, cursor: 'pointer' }}
              />
            </Tooltip>
          </Text>
        </Col>
        <Col span={14}>
          <Form.Item noStyle name='modelTemperature'>
            <Slider min={0} max={1.99} step={0.01} />
          </Form.Item>
        </Col>
        <Col span={4} style={{ textAlign: 'center' }}>
          <Form.Item noStyle name='modelTemperature'>
            <InputNumber min={0} max={1.99} step={0.01} />
          </Form.Item>
        </Col>
      </Row>
      <Row align='middle' gutter={4}>
        <Col span={6}>
          <Text>
            Top P
            <Tooltip title='Top p 为累计概率: 模型在生成输出时会从概率最高的词汇开始选择，直到这些词汇的总概率累积达到 Top p 值。这样可以限制模型只选择这些高概率的词汇，从而控制输出内容的多样性。建议不要与 "生成随机性" 同时调整。'>
              <QuestionCircleOutlined
                style={{ marginLeft: 8, cursor: 'pointer' }}
              />
            </Tooltip>
          </Text>
        </Col>
        <Col span={14}>
          <Form.Item noStyle name='modelTopP'>
            <Slider min={0} max={0.99} step={0.01} />
          </Form.Item>
        </Col>
        <Col span={4} style={{ textAlign: 'center' }}>
          <Form.Item noStyle name='modelTopP'>
            <InputNumber min={0} max={0.99} step={0.01} />
          </Form.Item>
        </Col>
      </Row>
      <Divider style={{ margin: 0 }} />
      <Row align='middle'>
        <Col flex={1}>
          <Text strong>输入及输出设置</Text>
        </Col>
      </Row>
      <Row align='middle' gutter={4}>
        <Col span={6}>
          <Text>
            最大回复长度
            <Tooltip title='控制模型输出的 Tokens 长度上限。通常 100 Tokens 约等于 150 个中文汉字。'>
              <QuestionCircleOutlined
                style={{ marginLeft: 8, cursor: 'pointer' }}
              />
            </Tooltip>
          </Text>
        </Col>
        <Col span={14}>
          <Form.Item noStyle name='modelMaxResponseLength'>
            <Slider min={5} max={2000} step={1} />
          </Form.Item>
        </Col>
        <Col span={4} style={{ textAlign: 'center' }}>
          <Form.Item noStyle name='modelMaxResponseLength'>
            <InputNumber min={5} max={2000} step={1} />
          </Form.Item>
        </Col>
      </Row>
    </div>
  )
}

export default ModelSettings
