import { useEffect, useState } from 'react'
import { TipsJinja2 } from '@/config'
import { FlowNodeSettings } from '@/dto'
import {
  VariableDataType,
  OutputFormat,
  OutputFormatDisplayNames,
} from '@/enums'
import { QuestionCircleOutlined, PlusOutlined } from '@ant-design/icons'
import {
  Segmented,
  Select,
  Collapse,
  Typography,
  Row,
  Col,
  Tooltip,
  Flex,
  Button,
  Switch,
  Checkbox,
  FormInstance,
  Form,
  Modal,
} from 'antd'
import { Settings2 } from 'lucide-react'
import AdaptiveHeightEditor from '@/components/adaptive-height-editor'
import ConfigsModelsApp from '@/features/ss-admin/settings/configsModels/configsModelsApp'
import ChatHistories from '../components/ChatHistories'
import IgnoreExceptions from '../components/IgnoreExceptions'
import ModelSelector from '../components/ModelSelector'
import ModelSettings from '../components/ModelSettings'
import VariableEditor from '../components/VariableEditor'
import VariableReference from '../components/VariableReference'
import { useStore } from '../store'

const { Text } = Typography

const LLMSettings: React.FC<{
  nodeId: string
  form: FormInstance<FlowNodeSettings>
}> = ({ nodeId, form }) => {
  const store = useStore()
  const [segment, setSegment] = useState('first')
  const [chatHistoryEnabled, setChatHistoryEnabled] = useState(false)
  const [modalConfigsModels, setModalConfigsModels] = useState(false)

  const settings = store.getNodeSettings(nodeId)

  useEffect(() => {
    setChatHistoryEnabled(settings.chatHistoryEnabled || false)

    form.setFieldsValue({
      chatHistoryEnabled: settings.chatHistoryEnabled,
      llmSystemPrompt: settings.llmSystemPrompt,
      llmUserPrompt: settings.llmUserPrompt,
      llmIsReply: settings.llmIsReply,
      llmOutputFormat: settings.llmOutputFormat,
    })
  }, [settings, store.models, store.defaultModel, form])

  const items = [
    {
      key: '1',
      label: (
        <Row align='middle' justify='space-between'>
          <Col>
            <Row align='middle'>
              <Typography.Text type='secondary' strong>
                输入
              </Typography.Text>
              <Tooltip title='输入需要添加到提示词的信息，这些信息可以被下方的提示词引用'>
                <QuestionCircleOutlined
                  style={{
                    marginLeft: '8px',
                    color: '#999',
                    cursor: 'pointer',
                  }}
                />
              </Tooltip>
            </Row>
          </Col>
          <Col>
            <Tooltip title='开启对话历史后，会自动把会话上下文信息，发送给模型'>
              <Text type='secondary' style={{ marginRight: '4px' }}>
                对话历史
              </Text>
            </Tooltip>
            <Form.Item noStyle name='chatHistoryEnabled'>
              <Switch
                size='small'
                style={{ marginLeft: '5px', marginRight: '10px' }}
                onChange={(value) => {
                  setChatHistoryEnabled(value)
                }}
              />
            </Form.Item>

            <Button
              size='small'
              type='primary'
              icon={<PlusOutlined style={{ verticalAlign: '-0.125em' }} />}
              onClick={() => {
                store.setNodeInVariables(nodeId, [
                  ...(store.getNodeInVariables(nodeId) || []),
                  {
                    key: (
                      store.getNodeInVariables(nodeId)?.length || 0
                    ).toString(),
                    name: '',
                    dataType: VariableDataType.String,
                    value: '',
                  },
                ])
              }}
            ></Button>
          </Col>
        </Row>
      ),
      children: (
        <>
          <VariableReference nodeId={nodeId} form={form}></VariableReference>
          {chatHistoryEnabled && (
            <ChatHistories nodeId={nodeId} form={form}></ChatHistories>
          )}
        </>
      ),
    },
    {
      key: '2',
      label: (
        <Row align='middle' justify='space-between'>
          <Col>
            <Text type='secondary' strong>
              模型
            </Text>
            <Tooltip title='向模型提供用户指令，如查询或任何基于文本输入的提问'>
              <QuestionCircleOutlined
                style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
              />
            </Tooltip>
          </Col>
          <Col>
            <Button
              type='link'
              style={{ height: 'auto' }}
              icon={<Settings2 className='h-4 w-4' />}
              onClick={() => {
                setModalConfigsModels(true)
              }}
            >
              模型设置
            </Button>
          </Col>
        </Row>
      ),
      children: <ModelSelector nodeId={nodeId} form={form} />,
    },
    {
      key: '3',
      label: (
        <Row align='middle'>
          <Text type='secondary' strong>
            系统提示词
          </Text>
          <Tooltip title='为对话提供系统级指导，如设定人设和回复逻辑，支持 Jinja2 模板语言'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </Row>
      ),
      children: (
        <>
          <Form.Item noStyle name='llmSystemPrompt'>
            <AdaptiveHeightEditor
              language='markdown'
              height='100px'
              value={settings.llmSystemPrompt || ''}
              onChange={(value) => {
                form.setFieldsValue({ llmSystemPrompt: value })
              }}
            />
          </Form.Item>
          <div className='tips'>{`为对话提供系统级指导，如设定人设和回复逻辑，${TipsJinja2}`}</div>
        </>
      ),
    },
    {
      key: '4',
      label: (
        <Row align='middle'>
          <Text type='secondary' strong>
            用户提示词
          </Text>
          <Tooltip title='向模型提供用户指令，如查询或任何基于文本输入的提问，支持 Jinja2 模板语言'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </Row>
      ),
      children: (
        <>
          <Form.Item noStyle name='llmUserPrompt'>
            <AdaptiveHeightEditor
              language='markdown'
              height='100px'
              value={settings.llmUserPrompt || ''}
              onChange={(value) => {
                form.setFieldsValue({ llmUserPrompt: value })
              }}
            />
          </Form.Item>
          <div className='tips'>{`向模型提供用户指令，如查询或任何基于文本输入的提问，${TipsJinja2}`}</div>
        </>
      ),
    },
    {
      key: '5',
      label: (
        <Row align='middle' justify='space-between'>
          <Col>
            <Text type='secondary' strong>
              输出
            </Text>
            <Tooltip title='大模型运行完成后生成的内容'>
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
              输出格式
            </Text>
            <Form.Item noStyle name='llmOutputFormat'>
              <Select
                size='small'
                style={{ marginLeft: '5px', marginRight: '10px', width: 110 }}
                options={[
                  {
                    value: OutputFormat.Markdown,
                    label: OutputFormatDisplayNames[OutputFormat.Markdown],
                  },
                  {
                    value: OutputFormat.JSON,
                    label: OutputFormatDisplayNames[OutputFormat.JSON],
                  },
                ]}
              />
            </Form.Item>
          </Col>
        </Row>
      ),
      children: (
        <>
          <VariableEditor nodeId={nodeId} disabled={true}></VariableEditor>
        </>
      ),
    },
    {
      key: '6',
      label: (
        <Row align='middle'>
          <Text type='secondary' strong>
            回复
          </Text>
          <Tooltip title='勾选直接回复后，智能体中的LLM将直接原文回复对话'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </Row>
      ),
      children: (
        <>
          <Form.Item noStyle name='llmIsReply' valuePropName='checked'>
            <Checkbox>直接回复</Checkbox>
          </Form.Item>
        </>
      ),
    },
    {
      key: '7',
      label: <IgnoreExceptions nodeId={nodeId} form={form} />,
      children: (
        <span className='tips'>忽略异常并在异常发生时使用默认输出替代</span>
      ),
    },
  ]

  return (
    <Flex gap='middle' vertical style={{ margin: '15px 0' }}>
      <Segmented
        style={{ margin: '0 15px' }}
        options={[
          { label: '大模型设置', value: 'first' },
          { label: '高级设置', value: 'second' },
        ]}
        block
        onChange={(value) => {
          setSegment(value)
        }}
      />
      {segment == 'first' && (
        <>
          <Collapse
            items={items}
            bordered={false}
            collapsible='icon'
            defaultActiveKey={items.map((x) => x.key)}
          />
        </>
      )}
      {segment == 'second' && <ModelSettings nodeId={nodeId} form={form} />}

      {modalConfigsModels && (
        <Modal
          open={modalConfigsModels}
          onCancel={() => setModalConfigsModels(false)}
          footer={null}
          width='850px'
          style={{ top: 20, bottom: 20 }}
          destroyOnHidden={true}
        >
          <div className='h-full overflow-y-auto'>
            <ConfigsModelsApp />
          </div>
        </Modal>
      )}
    </Flex>
  )
}

export default LLMSettings
