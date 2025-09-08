import { useEffect } from 'react'
import { FlowNodeSettings } from '@/dto/flowNodeSettings'
import { DownOutlined, QuestionCircleOutlined } from '@ant-design/icons'
import {
  Typography,
  Row,
  Col,
  Tree,
  Tooltip,
  Form,
  InputNumber,
  FormInstance,
} from 'antd'
import { useStore } from '../store'

const { Text } = Typography

const ChatHistories: React.FC<{
  nodeId: string
  form: FormInstance<FlowNodeSettings>
}> = ({ nodeId, form }) => {
  const store = useStore()
  const settings = store.getNodeSettings(nodeId)

  useEffect(() => {
    form.setFieldsValue({
      chatHistoryCount: settings.chatHistoryCount || 10,
    })
  }, [settings, form])

  return (
    <Row style={{ marginTop: 10 }}>
      <Col span={12}>
        <Tree
          showLine
          switcherIcon={<DownOutlined />}
          defaultExpandedKeys={['0-0']}
          treeData={[
            {
              title: (
                <>
                  <Text>chatHistory</Text>
                  <Text code>Array&lt;Object&gt;</Text>
                </>
              ),
              key: '0-0',
              children: [
                {
                  title: (
                    <>
                      <Text>role</Text>
                      <Text code>String</Text>
                    </>
                  ),
                  key: '0-0-0',
                },
                {
                  title: (
                    <>
                      <Text>content</Text>
                      <Text code>String</Text>
                    </>
                  ),
                  key: '0-0-1',
                },
              ],
            },
          ]}
        />
      </Col>
      <Col span={12} style={{ textAlign: 'right' }}>
        <Text type='secondary'>对话历史长度</Text>
        <Tooltip title='设置带入模型上下文的对话历史轮数。轮数越多，多轮对话的相关性越高，但消耗的 Token 也越多'>
          <QuestionCircleOutlined
            style={{
              marginLeft: 8,
              marginRight: 8,
              color: '#999',
              cursor: 'pointer',
            }}
          />
        </Tooltip>
        <Form.Item noStyle name='chatHistoryCount'>
          <InputNumber
            style={{ marginRight: 10, display: 'inline-block' }}
            min={1}
            max={30}
            changeOnWheel
          />
        </Form.Item>
      </Col>
    </Row>
  )
}

export default ChatHistories
