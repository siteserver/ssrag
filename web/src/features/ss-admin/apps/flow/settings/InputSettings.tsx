import { QuestionCircleOutlined } from '@ant-design/icons'
import { Flex } from 'antd'
import { Row, Typography, Tooltip, Collapse } from 'antd'
import VariableEditor from '../components/VariableEditor'
import { useStore } from '../store'

const { Text } = Typography

const InputSettings: React.FC<{
  nodeId: string
}> = ({ nodeId }) => {
  const store = useStore()
  const settings = store.getNodeSettings(nodeId)

  const items = [
    {
      key: '1',
      label: (
        <Row align='middle'>
          <Text type='secondary' strong>
            输入
          </Text>
          <Tooltip title='定义启动工作流需要的输入参数，这些内容将在智能体对话过程中被LLM阅读，使LLM可以在合适的时候启动工作流并填入正确的信息'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </Row>
      ),
      children: (
        <VariableEditor
          nodeId={nodeId}
          disabled={settings.isFixed || false}
        ></VariableEditor>
      ),
    },
    {
      key: '2',
      label: (
        <Row align='middle'>
          <Text type='secondary' strong>
            输出
          </Text>
          <Tooltip title='大模型运行完成后生成的内容'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </Row>
      ),
      children: (
        <>
          <VariableEditor
            nodeId={nodeId}
            disabled={settings.isFixed || false}
          ></VariableEditor>
        </>
      ),
    },
  ]

  return (
    <Flex gap='middle' vertical>
      <Collapse
        items={items}
        bordered={false}
        collapsible='icon'
        defaultActiveKey={items.map((x) => x.key)}
      />
    </Flex>
  )
}

export default InputSettings
