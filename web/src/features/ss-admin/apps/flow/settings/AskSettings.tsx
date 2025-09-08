import { useState, useEffect } from 'react'
import { TipsJinja2 } from '@/config'
import { QuestionCircleOutlined } from '@ant-design/icons'
import { DndContext, DragEndEvent, closestCenter } from '@dnd-kit/core'
import { SortableContext, verticalListSortingStrategy } from '@dnd-kit/sortable'
import {
  Collapse,
  Typography,
  Row,
  Col,
  Tooltip,
  Flex,
  Input,
  Switch,
  Segmented,
  Form,
  FormInstance,
} from 'antd'
import SortableInput from '@/components/sortable-input'
import VariableEditor from '../components/VariableEditor'
import VariableReference from '../components/VariableReference'
import { useStore } from '../store'

const { Text } = Typography

const AskSettings: React.FC<{
  nodeId: string
  form: FormInstance
}> = ({ nodeId, form }) => {
  const store = useStore()
  const settings = store.getNodeSettings(nodeId)

  const [options, setOptions] = useState([''])

  useEffect(() => {
    form.setFieldsValue({
      question: settings.question,
    })

    const list = settings.options || []
    if (list.length === 0 || list[list.length - 1] !== '') {
      list.push('')
    }
    setOptions(list)
  }, [settings, form])

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event
    if (active.id !== over?.id) {
      const oldIndex = Number(active.id)
      const newIndex = Number(over?.id)
      const newOptions = [...options]
      const [movedItem] = newOptions.splice(oldIndex, 1)
      newOptions.splice(newIndex, 0, movedItem)
      setOptions(newOptions)
      store.setNodeSettings(nodeId, {
        ...settings,
        options: newOptions,
      })
    }
  }

  const items = [
    {
      key: '1',
      label: (
        <Row align='middle'>
          <Typography.Text type='secondary' strong>
            输入
          </Typography.Text>
          <Tooltip title='输入需要添加到提示词的信息，这些信息可以被下方的提示词引用'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </Row>
      ),
      children: (
        <>
          <VariableReference nodeId={nodeId} form={form}></VariableReference>
        </>
      ),
    },
    {
      key: '2',
      label: (
        <Row align='middle'>
          <Text type='secondary' strong>
            提问内容
          </Text>
          <Tooltip title='设置向用户发出的问题，支持 Jinja2 模板语言'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </Row>
      ),
      children: (
        <>
          <Form.Item
            name='question'
            label='提问内容'
            rules={[
              {
                required: true,
                message: '请输入提问内容',
              },
            ]}
          >
            <Input.TextArea
              autoSize={{ minRows: 4 }}
              onChange={(e) =>
                store.setNodeSettings(nodeId, {
                  ...settings,
                  question: e.target.value,
                })
              }
              placeholder={TipsJinja2}
            ></Input.TextArea>
          </Form.Item>
          <div className='tips'>{`设置向用户发出的问题，${TipsJinja2}`}</div>
        </>
      ),
    },
    {
      key: '3',
      label: (
        <Row align='middle'>
          <Typography.Text type='secondary' strong>
            提问回复
          </Typography.Text>
          <Tooltip title='请选择回复类型'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </Row>
      ),
      children: (
        <>
          <Segmented
            style={{ marginBottom: '10px' }}
            value={settings.isDirectReply || false}
            options={[
              { label: '选项回复', value: false },
              { label: '直接回复', value: true },
            ]}
            block
            onChange={(value) => {
              store.setNodeSettings(nodeId, {
                ...settings,
                isDirectReply: value,
              })
            }}
          />
          {!settings.isDirectReply && (
            <>
              <Text
                type='secondary'
                strong
                style={{ display: 'block', marginBottom: '10px' }}
              >
                回复选项
              </Text>
              <DndContext
                collisionDetection={closestCenter}
                onDragEnd={handleDragEnd}
              >
                <SortableContext
                  items={options.map((_, i) => i)}
                  strategy={verticalListSortingStrategy}
                >
                  {options.map((_, index) => (
                    <SortableInput
                      name='options'
                      key={index}
                      index={index}
                      placeholder='留空对用户不可见，当用户回复无关内容时，走此分支'
                      onDelete={(i) => {
                        const newOptions = options.filter((_, j) => j !== i)
                        if (newOptions.length === 0) {
                          newOptions.push('')
                        } else if (newOptions[newOptions.length - 1] !== '') {
                          newOptions.push('')
                        }
                        setOptions(newOptions)
                      }}
                    />
                  ))}
                </SortableContext>
              </DndContext>
            </>
          )}
        </>
      ),
    },
    {
      key: '4',
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
      key: '5',
      label: (
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
            <Switch
              checked={settings.isIgnoreExceptions}
              onChange={() => {
                store.setNodeSettings(nodeId, {
                  ...settings,
                  isIgnoreExceptions: !settings.isIgnoreExceptions,
                })
              }}
              size='small'
              style={{ marginLeft: '5px', marginRight: '10px' }}
            />
          </Col>
        </Row>
      ),
      children: (
        <span className='tips'>忽略异常并在异常发生时使用默认输出替代</span>
      ),
    },
  ]

  return (
    <Flex gap='middle' vertical style={{ margin: '0 0 15px' }}>
      <Collapse
        items={items}
        bordered={false}
        collapsible='icon'
        defaultActiveKey={['2', '3', '4', '5']}
      />
    </Flex>
  )
}

export default AskSettings
