import { QuestionCircleOutlined } from '@ant-design/icons'
import {
  Flex,
  Input,
  Row,
  Col,
  Typography,
  Tooltip,
  Switch,
  Collapse,
  FormInstance,
} from 'antd'
import VariableReference from '../components/VariableReference'
import { useStore } from '../store'

const { Text } = Typography

const OutputSettings: React.FC<{
  nodeId: string
  form: FormInstance
}> = ({ nodeId, form }) => {
  const store = useStore()
  const settings = store.getNodeSettings(nodeId)

  const items = [
    {
      key: '1',
      label: (
        <Row>
          <Text type='secondary' strong>
            输入
          </Text>
          <Tooltip title='这些变量将在智能体调用工作流过程中被输出'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </Row>
      ),
      children: (
        <VariableReference
          nodeId={nodeId}
          form={form}
          disabled={settings.isFixed}
        ></VariableReference>
      ),
    },
    {
      key: '2',
      label: (
        <Row align='middle' justify='space-between'>
          <Col>
            <Text type='secondary' strong>
              回答内容
            </Text>
            <Tooltip title='编辑智能体的回复内容，即工作流运行完成后，智能体中的LLM将不再组织语言，而是直接用这里编辑的内容原文回复对话，支持 Jinja2 模板语言'>
              <QuestionCircleOutlined
                style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
              />
            </Tooltip>
          </Col>
          <Col>
            <Tooltip
              placement='left'
              title='开启时，回复内容中的大语言模型的生成内容将会逐字流式输出；关闭时，回复内容将全部生成后一次性输出'
            >
              <Text
                type='secondary'
                style={{ marginRight: '4px', cursor: 'pointer' }}
              >
                流式输出
              </Text>
              <Switch
                checked={settings.isStreaming}
                onChange={() => {
                  store.setNodeSettings(nodeId, {
                    ...settings,
                    isStreaming: !settings.isStreaming,
                  })
                }}
                size='small'
                style={{ marginLeft: '5px', marginRight: '10px' }}
              />
            </Tooltip>
          </Col>
        </Row>
      ),
      children: (
        <Input.TextArea
          autoSize={{ minRows: 6 }}
          value={settings.output || ''}
          onChange={(e) =>
            store.setNodeSettings(nodeId, {
              ...settings,
              output: e.target.value,
            })
          }
          placeholder='可以使用{{变量名}}、{{变量名.子变量名}}、{{变量名[数组索引]}}的方式引用输出参数中的变量'
        ></Input.TextArea>
      ),
    },
  ]

  return (
    <Flex gap='middle' vertical style={{ margin: '15px 0' }}>
      <Collapse
        items={items}
        bordered={false}
        collapsible='icon'
        defaultActiveKey={items.map((x) => x.key)}
      />
    </Flex>
  )
}

export default OutputSettings
