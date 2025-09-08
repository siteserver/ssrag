import { useEffect } from 'react'
import { TipsJinja2 } from '@/config'
import { TextProcessType, VariableDataType } from '@/enums'
import { QuestionCircleOutlined, PlusOutlined } from '@ant-design/icons'
import {
  Collapse,
  Typography,
  Row,
  Col,
  Tooltip,
  Flex,
  Switch,
  Form,
  Input,
  Select,
  Button,
  Checkbox,
  FormInstance,
} from 'antd'
import VariableEditor from '../components/VariableEditor'
import VariableReference from '../components/VariableReference'
import { useStore } from '../store'

const { Text } = Typography

const TextSettings: React.FC<{
  nodeId: string
  form: FormInstance
}> = ({ nodeId, form }) => {
  const store = useStore()
  const settings = store.getNodeSettings(nodeId)

  useEffect(() => {
    form.setFieldsValue({
      textSplit: settings.textSplit,
      textReplace: settings.textReplace,
      textTo: settings.textTo,
      textJoint: settings.textJoint,
    })
  }, [settings])

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
              <Tooltip title='请输入API的参数，当此节点运行时，会将这些参数传入并调用这个API'>
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
            <Button
              size='small'
              type='primary'
              disabled={settings.textProcessType === TextProcessType.Split}
              icon={<PlusOutlined style={{ verticalAlign: '-0.125em' }} />}
              onClick={() => {
                store.setNodeInVariables(nodeId, [
                  ...store.getNodeInVariables(nodeId),
                  {
                    key: store.getNodeInVariables(nodeId).length.toString(),
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
          <VariableReference
            nodeId={nodeId}
            form={form}
            disabled={settings.isFixed}
          ></VariableReference>
        </>
      ),
    },
    {
      key: '2',
      label: (
        <Row align='middle' justify='space-between'>
          <Col>
            <Text type='secondary' strong>
              规则设置
            </Text>
          </Col>
          <Col>
            <Text
              type='secondary'
              style={{ marginRight: '4px', cursor: 'pointer' }}
            >
              处理类型
            </Text>
            <Select
              defaultValue={settings.textProcessType}
              style={{ marginLeft: '5px', marginRight: '10px', width: 110 }}
              size='small'
              onChange={(value) => {
                if (value == TextProcessType.Joint) {
                  store.setNodeOutVariables(nodeId, [
                    {
                      key: '0',
                      name: 'output',
                      dataType: VariableDataType.String,
                      value: '',
                    },
                  ])
                } else if (value == TextProcessType.Replace) {
                  const variables =
                    store.getNodeInVariables(nodeId).length > 0
                      ? [store.getNodeInVariables(nodeId)[0]]
                      : [
                          {
                            key: '0',
                            name: 'input',
                            dataType: VariableDataType.String,
                            value: '',
                          },
                        ]
                  store.setNodeInVariables(nodeId, variables)
                  store.setNodeOutVariables(nodeId, [...variables])
                } else if (value == TextProcessType.Split) {
                  const variables =
                    store.getNodeInVariables(nodeId).length > 0
                      ? [store.getNodeInVariables(nodeId)[0]]
                      : [
                          {
                            key: '0',
                            name: 'input',
                            dataType: VariableDataType.String,
                            value: '',
                          },
                        ]
                  store.setNodeInVariables(nodeId, variables)
                  store.setNodeOutVariables(nodeId, [
                    {
                      key: '0',
                      name: 'output',
                      dataType: VariableDataType.ArrayString,
                      value: '',
                    },
                  ])
                }
                store.setNodeSettings(nodeId, {
                  ...settings,
                  textProcessType: value,
                })
              }}
              options={[
                { value: TextProcessType.Joint, label: '文本拼接' },
                { value: TextProcessType.Split, label: '文本分隔' },
                { value: TextProcessType.Replace, label: '文本替换' },
              ]}
            />
          </Col>
        </Row>
      ),
      children: (
        <>
          {settings.textProcessType === TextProcessType.Joint && (
            <Form.Item
              name='textJoint'
              label='文本拼接'
              rules={[
                {
                  required: true,
                  message: '请输入文本拼接',
                },
              ]}
            >
              <Input.TextArea
                autoSize={{ minRows: 4 }}
                onChange={(e) =>
                  store.setNodeSettings(nodeId, {
                    ...settings,
                    textJoint: e.target.value,
                  })
                }
                placeholder={TipsJinja2}
              ></Input.TextArea>
            </Form.Item>
          )}
          {settings.textProcessType === TextProcessType.Split && (
            <Form.Item
              name='textSplit'
              label='文本分隔符'
              rules={[
                {
                  required: true,
                  message: '请输入文本分隔符',
                },
              ]}
            >
              <Input placeholder='请输入文本分隔符' />
            </Form.Item>
          )}
          {settings.textProcessType === TextProcessType.Replace && (
            <>
              <Form.Item
                name='textReplace'
                label='查找文字'
                rules={[
                  {
                    required: true,
                    message: '请输入查找文字',
                  },
                ]}
              >
                <Input.TextArea
                  autoSize={{ minRows: 4 }}
                  onChange={(e) =>
                    store.setNodeSettings(nodeId, {
                      ...settings,
                      textReplace: e.target.value,
                    })
                  }
                  placeholder='可以使用{{变量名}}、{{变量名.子变量名}}、{{变量名[数组索引]}的方式引用输入参数中的变量...'
                ></Input.TextArea>
              </Form.Item>
              <Form.Item label='查找设置'>
                <Checkbox
                  checked={settings.isTextCaseIgnore}
                  disabled={settings.isTextRegex}
                  onChange={(e) => {
                    store.setNodeSettings(nodeId, {
                      ...settings,
                      isTextCaseIgnore: e.target.checked,
                    })
                  }}
                >
                  不区分大小写
                </Checkbox>
                <Checkbox
                  checked={settings.isTextRegex}
                  disabled={settings.isTextCaseIgnore}
                  onChange={(e) => {
                    store.setNodeSettings(nodeId, {
                      ...settings,
                      isTextRegex: e.target.checked,
                    })
                  }}
                >
                  使用正则表达式
                </Checkbox>
              </Form.Item>
              <Form.Item
                name='textTo'
                label='替换文字'
                rules={[
                  {
                    required: true,
                    message: '请输入替换文字',
                  },
                ]}
              >
                <Input.TextArea
                  autoSize={{ minRows: 4 }}
                  onChange={(e) =>
                    store.setNodeSettings(nodeId, {
                      ...settings,
                      textTo: e.target.value,
                    })
                  }
                  placeholder='可以使用{{变量名}}、{{变量名.子变量名}}、{{变量名[数组索引]}的方式引用输入参数中的变量...'
                ></Input.TextArea>
              </Form.Item>
            </>
          )}
        </>
      ),
    },
    {
      key: '3',
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
          <VariableEditor nodeId={nodeId} disabled={true}></VariableEditor>
        </>
      ),
    },
    {
      key: '4',
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

export default TextSettings
