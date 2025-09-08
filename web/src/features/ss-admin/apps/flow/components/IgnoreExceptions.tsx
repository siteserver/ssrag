import { useEffect } from 'react'
import { FlowNodeSettings } from '@/dto/flowNodeSettings'
import { QuestionCircleOutlined } from '@ant-design/icons'
import { Typography, Row, Col, Tooltip, Form, FormInstance, Switch } from 'antd'
import { useStore } from '../store'

const { Text } = Typography

const IgnoreExceptions: React.FC<{
  nodeId: string
  form: FormInstance<FlowNodeSettings>
}> = ({ nodeId, form }) => {
  const store = useStore()
  const settings = store.getNodeSettings(nodeId)

  useEffect(() => {
    form.setFieldsValue({
      isIgnoreExceptions: settings.isIgnoreExceptions,
    })
  }, [settings, form])

  return (
    <Row align='middle' justify='space-between'>
      <Col>
        <Text type='secondary' strong>
          异常设置
        </Text>
        <Tooltip title='忽略异常并在异常发生时使用默认输出替代'>
          <QuestionCircleOutlined
            style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
          />
        </Tooltip>
      </Col>
      <Col>
        <Text
          type='secondary'
          style={{ marginRight: '4px', cursor: 'pointer' }}
        >
          忽略异常
        </Text>
        <Form.Item noStyle name='isIgnoreExceptions'>
          <Switch
            size='small'
            style={{ marginLeft: '5px', marginRight: '10px' }}
          />
        </Form.Item>
      </Col>
    </Row>
  )
}

export default IgnoreExceptions
