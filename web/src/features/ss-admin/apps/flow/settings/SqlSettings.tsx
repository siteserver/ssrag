import { useEffect } from 'react'
import { TipsJinja2 } from '@/config'
import { DatabaseTypeOptions } from '@/enums'
import { QuestionCircleOutlined } from '@ant-design/icons'
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
  Segmented,
  Radio,
  FormInstance,
} from 'antd'
import VariableEditor from '../components/VariableEditor'
import VariableReference from '../components/VariableReference'
import { useStore } from '../store'

const { Text } = Typography

const SqlSettings: React.FC<{
  nodeId: string
  form: FormInstance
}> = ({ nodeId, form }) => {
  const store = useStore()
  const settings = store.getNodeSettings(nodeId)

  useEffect(() => {
    form.setFieldsValue({
      sqlDatabaseType: settings.sqlDatabaseType,
      sqlDatabaseHost: settings.sqlDatabaseHost,
      isSqlDatabasePort: settings.isSqlDatabasePort || false,
      sqlDatabasePort: settings.sqlDatabasePort,
      sqlDatabaseUserName: settings.sqlDatabaseUserName,
      sqlDatabasePassword: settings.sqlDatabasePassword,
      sqlDatabaseName: settings.sqlDatabaseName,
      sqlQueryString: settings.sqlQueryString,
    })
  }, [settings])

  const items = [
    {
      key: '1',
      label: (
        <Row align='middle'>
          <Typography.Text type='secondary' strong>
            输入
          </Typography.Text>
          <Tooltip title='请输入API的参数，当此节点运行时，会将这些参数传入并调用这个API'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
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
        <Row align='middle'>
          <Text type='secondary' strong>
            SQL 查询设置
          </Text>
          <Tooltip title='要执行的SQL语句，可以直接使用输入参数中的变量，注意rowNum输出返回的行数或者受影响的行数，outputList中的变量名需与SQL中定义的字段名一致。'>
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
            value={settings.isSqlDatabase || false}
            options={[
              { label: '查询当前数据库', value: false },
              { label: '查询指定数据库', value: true },
            ]}
            block
            onChange={(value) => {
              store.setNodeSettings(nodeId, {
                ...settings,
                isSqlDatabase: value,
              })
            }}
          />
          {settings.isSqlDatabase && (
            <>
              <Form.Item
                name='sqlDatabaseType'
                label='数据库类型'
                rules={[
                  {
                    required: true,
                    message: '请选择数据库类型',
                  },
                ]}
              >
                <Select
                  onChange={(value) => {
                    store.setNodeSettings(nodeId, {
                      ...settings,
                      sqlDatabaseType: value,
                    })
                  }}
                  options={DatabaseTypeOptions}
                />
              </Form.Item>
              {settings.sqlDatabaseType && (
                <>
                  <Form.Item
                    label='数据库主机'
                    name='sqlDatabaseHost'
                    rules={[{ required: true, message: '请输入数据库主机' }]}
                  >
                    <Input placeholder='请填写数据库的IP地址或域名地址' />
                  </Form.Item>
                  <Form.Item label='数据库端口'>
                    <Radio.Group
                      value={settings.isSqlDatabasePort || false}
                      onChange={(e) =>
                        store.setNodeSettings(nodeId, {
                          ...settings,
                          isSqlDatabasePort: e.target.value,
                        })
                      }
                    >
                      <Radio value={false}>默认端口</Radio>
                      <Radio value={true}>自定义端口</Radio>
                    </Radio.Group>
                  </Form.Item>
                  {settings.isSqlDatabasePort && (
                    <Form.Item
                      label='自定义端口'
                      name='sqlDatabasePort'
                      rules={[{ required: true, message: '请输入自定义端口' }]}
                    >
                      <Input type='number' placeholder='连接数据库的端口' />
                    </Form.Item>
                  )}
                  <Form.Item
                    label='数据库用户名'
                    name='sqlDatabaseUserName'
                    rules={[{ required: true, message: '请输入数据库用户名' }]}
                  >
                    <Input placeholder='连接数据库的用户名' />
                  </Form.Item>
                  <Form.Item
                    label='数据库密码'
                    name='sqlDatabasePassword'
                    rules={[{ required: true, message: '请输入数据库密码' }]}
                  >
                    <Input.Password placeholder='连接数据库的密码' />
                  </Form.Item>
                  <Form.Item
                    label='数据库实例名称'
                    name='sqlDatabaseName'
                    rules={[
                      { required: true, message: '请输入数据库实例名称' },
                    ]}
                  >
                    <Input placeholder='连接数据库的实例名称' />
                  </Form.Item>
                </>
              )}
            </>
          )}
          <Form.Item
            name='sqlQueryString'
            label='SQL查询命令'
            rules={[
              {
                required: true,
                message: '请输入SQL查询命令',
              },
            ]}
          >
            <Input.TextArea
              autoSize={{ minRows: 4 }}
              value={settings.sqlQueryString || ''}
              onChange={(e) =>
                store.setNodeSettings(nodeId, {
                  ...settings,
                  sqlQueryString: e.target.value,
                })
              }
              placeholder={TipsJinja2}
            ></Input.TextArea>
          </Form.Item>
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
          <Tooltip title='SQL查询运行完成后生成的内容'>
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

export default SqlSettings
