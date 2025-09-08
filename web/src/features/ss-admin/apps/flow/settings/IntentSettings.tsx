import { useEffect } from 'react'
import { useState } from 'react'
import { TipsJinja2 } from '@/config'
import { PlusOutlined, QuestionCircleOutlined } from '@ant-design/icons'
import { DndContext, closestCenter, DragEndEvent } from '@dnd-kit/core'
import { SortableContext, verticalListSortingStrategy } from '@dnd-kit/sortable'
import {
  Segmented,
  Collapse,
  Typography,
  Row,
  Col,
  Tooltip,
  Flex,
  Switch,
  FormInstance,
  Form,
  Button,
  App,
  Modal,
} from 'antd'
import { Settings2 } from 'lucide-react'
import flowApi from '@/api/admin/apps/flowApi'
import MagicButton from '@/components/MagicButton'
import AdaptiveHeightEditor from '@/components/adaptive-height-editor'
import SortableInput from '@/components/sortable-input'
import ConfigsModelsApp from '@/features/ss-admin/settings/configsModels/configsModelsApp'
import ChatHistories from '../components/ChatHistories'
import IgnoreExceptions from '../components/IgnoreExceptions'
import ModelSelector from '../components/ModelSelector'
import ModelSettings from '../components/ModelSettings'
import VariableEditor from '../components/VariableEditor'
import VariableReference from '../components/VariableReference'
import { useStore } from '../store'

const { Text } = Typography

const IntentSettings: React.FC<{
  nodeId: string
  form: FormInstance
}> = ({ nodeId, form }) => {
  const store = useStore()
  const settings = store.getNodeSettings(nodeId)
  const [segment, setSegment] = useState('first')
  const [intentions, setIntentions] = useState([''])
  const [chatHistoryEnabled, setChatHistoryEnabled] = useState(false)
  const [modalConfigsModels, setModalConfigsModels] = useState(false)
  const { message } = App.useApp()

  useEffect(() => {
    setChatHistoryEnabled(settings.chatHistoryEnabled || false)

    const list = [...(settings.intentions || [])]
    if (list.length === 0) {
      list.push('')
    }
    setIntentions(list)

    form.setFieldsValue({
      chatHistoryEnabled: settings.chatHistoryEnabled,
      intentPrompt: settings.intentPrompt,
      intentions: settings.intentions,
    })
  }, [settings, form])

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event
    if (active.id !== over?.id) {
      const oldIndex = parseInt(active.id as string)
      const newIndex = parseInt(over?.id as string)
      const newIntentions = [...intentions]
      const [movedItem] = newIntentions.splice(oldIndex, 1)
      newIntentions.splice(newIndex, 0, movedItem)
      setIntentions(newIntentions)
    }
  }

  const handleOptimize = async () => {
    const intentions = form.getFieldValue('intentions') || []
    if (
      intentions.length === 0 ||
      intentions.every((item: string) => item === '')
    ) {
      message.error('请先添加需要优化的意图')
      return
    }
    const res = await flowApi.optimize({
      siteId: store.siteId,
      items: intentions,
    })
    const newIntentions = res?.items || []
    setIntentions(newIntentions)
    form.setFieldsValue({
      intentions: newIntentions,
    })
  }

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
        <Row align='middle' justify='space-between'>
          <Col>
            <Row align='middle'>
              <Typography.Text type='secondary' strong>
                意图匹配
              </Typography.Text>
              <Tooltip title='用于与用户输入匹配的意图选项'>
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
            <MagicButton onClick={handleOptimize} style={{ height: 25 }}>
              一键优化
            </MagicButton>
          </Col>
        </Row>
      ),
      children: (
        <>
          <DndContext
            collisionDetection={closestCenter}
            onDragEnd={handleDragEnd}
          >
            <SortableContext
              items={intentions.map((_, i) => i)}
              strategy={verticalListSortingStrategy}
            >
              {intentions.map((_, index) => (
                <SortableInput
                  key={index}
                  name='intentions'
                  index={index}
                  placeholder='请输入意图'
                  onDelete={(i) => {
                    const newIntentions = intentions.filter((_, j) => j !== i)
                    setIntentions(newIntentions)
                  }}
                />
              ))}
            </SortableContext>
          </DndContext>
          <Button
            block
            color='primary'
            variant='dashed'
            icon={<PlusOutlined style={{ verticalAlign: '-0.125em' }} />}
            onClick={() => {
              const newIntentions = [...intentions]
              newIntentions.push('')
              setIntentions(newIntentions)
            }}
          >
            添加意图
          </Button>
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
          <Form.Item noStyle name='intentPrompt'>
            <AdaptiveHeightEditor
              language='markdown'
              height='100px'
              value={settings.intentPrompt || ''}
              onChange={(value) => {
                form.setFieldsValue({ intentPrompt: value })
              }}
            />
          </Form.Item>
          <div className='tips'>{`支持额外的系统提示词，如对意图选项提供更详细的例子以增强用户输入与意图匹配的成功率，${TipsJinja2}`}</div>
        </>
      ),
    },
    {
      key: '5',
      label: (
        <Row>
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
          <VariableEditor nodeId={nodeId} disabled={true}></VariableEditor>
        </>
      ),
    },
    {
      key: '6',
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
          { label: '意图识别', value: 'first' },
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
            defaultActiveKey={['1', '2', '3', '4', '5', '6']}
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

export default IntentSettings
