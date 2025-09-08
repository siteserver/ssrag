import { useEffect } from 'react'
import { FlowNodeSettings } from '@/dto'
import { QuestionCircleOutlined, DownOutlined } from '@ant-design/icons'
import {
  Collapse,
  Typography,
  Row,
  Col,
  Tooltip,
  Flex,
  Switch,
  Tree,
  FormInstance,
  Form,
  Input,
  Select,
  InputNumber,
} from 'antd'
import VariableEditor from '../components/VariableEditor'
import VariableReference from '../components/VariableReference'
import { useStore } from '../store'

const { Text } = Typography

// https://bocha-ai.feishu.cn/wiki/RXEOw02rFiwzGSkd9mUcqoeAnNK

const WebSearchSettings: React.FC<{
  nodeId: string
  form: FormInstance
}> = ({ nodeId, form }) => {
  const store = useStore()
  const settings = store.getNodeSettings(nodeId)

  useEffect(() => {
    form.setFieldsValue({
      webSearchApiKey: settings.webSearchApiKey,
      webSearchFreshness: settings.webSearchFreshness || 'noLimit',
      webSearchSummary: settings.webSearchSummary,
      webSearchInclude: settings.webSearchInclude,
      webSearchExclude: settings.webSearchExclude,
      webSearchCount: settings.webSearchCount || 10,
      isIgnoreExceptions: settings.isIgnoreExceptions,
    })
  }, [form, settings])

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
          <VariableReference nodeId={nodeId} form={form}></VariableReference>
        </>
      ),
    },
    {
      key: '2',
      label: (
        <Row align='middle'>
          <Text type='secondary' strong>
            联网搜索设置
          </Text>
          <Tooltip title='设置联网搜索参数'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </Row>
      ),
      children: (
        <>
          <Form.Item<FlowNodeSettings>
            name='webSearchApiKey'
            label='API 密钥'
            rules={[
              {
                required: true,
                message: '请输入 API 密钥',
              },
            ]}
          >
            <Input placeholder='请输入 API 密钥' />
          </Form.Item>
          <Form.Item<FlowNodeSettings>
            name='webSearchFreshness'
            label='时间范围'
            tooltip='推荐使用“不限”。搜索算法会自动进行时间范围的改写，效果更佳。如果指定时间范围，很有可能出现时间范围内没有相关网页的情况，导致找不到搜索结果。'
            rules={[
              {
                required: true,
                message: '搜索指定时间范围内的网页',
              },
            ]}
          >
            <Select>
              <Select.Option value='oneDay'>一天内</Select.Option>
              <Select.Option value='oneWeek'>一周内</Select.Option>
              <Select.Option value='oneMonth'>一个月内</Select.Option>
              <Select.Option value='oneYear'>一年内</Select.Option>
              <Select.Option value='noLimit'>不限</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item<FlowNodeSettings>
            name='webSearchSummary'
            label='是否显示文本摘要'
            valuePropName='checked'
          >
            <Switch />
          </Form.Item>

          <Form.Item<FlowNodeSettings>
            name='webSearchInclude'
            label='指定搜索网址'
            tooltip='多个域名使用 | 或 , 分隔，可填入域名或子域名，最多不能超过 20 个'
          >
            <Input placeholder='请输入指定搜索网址' />
          </Form.Item>
          <Form.Item<FlowNodeSettings>
            name='webSearchExclude'
            label='排除搜索网址'
            tooltip='多个域名使用 | 或 , 分隔，可填入域名或子域名，最多不能超过 20 个'
          >
            <Input placeholder='请输入排除搜索网址' />
          </Form.Item>
          <Form.Item<FlowNodeSettings>
            name='webSearchCount'
            label='返回的搜索结果数量'
            tooltip='响应中返回的返回的搜索结果数量，最大值为50，实际返回结果数量可能会小于指定的数量'
            rules={[
              {
                required: true,
                message: '请输入返回的搜索结果数量',
              },
              {
                type: 'number',
                min: 1,
                max: 50,
                message: '返回的搜索结果数量必须在 1 到 50 之间！',
              },
            ]}
          >
            <InputNumber min={1} max={50} changeOnWheel />
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
          <Tree
            showLine
            switcherIcon={<DownOutlined />}
            defaultExpandedKeys={['0-0']}
            treeData={[
              {
                title: (
                  <>
                    <Text>output</Text>
                    <Text code>Array&lt;Object&gt;</Text>
                  </>
                ),
                key: '0-0',
                children: [
                  {
                    title: (
                      <>
                        <Text>name</Text>
                        <Text code>String 网页的标题</Text>
                      </>
                    ),
                    key: '0-0-1',
                  },
                  {
                    title: (
                      <>
                        <Text>url</Text>
                        <Text code>String 网页的URL</Text>
                      </>
                    ),
                    key: '0-0-2',
                  },
                  {
                    title: (
                      <>
                        <Text>snippet</Text>
                        <Text code>String 网页内容的简短描述</Text>
                      </>
                    ),
                    key: '0-0-3',
                  },
                  {
                    title: (
                      <>
                        <Text>summary</Text>
                        <Text code>
                          String 网页内容的文本摘要，当显示文本摘要时显示
                        </Text>
                      </>
                    ),
                    key: '0-0-4',
                  },
                  {
                    title: (
                      <>
                        <Text>siteName</Text>
                        <Text code>String 网页的网站名称</Text>
                      </>
                    ),
                    key: '0-0-5',
                  },
                  {
                    title: (
                      <>
                        <Text>siteIcon</Text>
                        <Text code>String 网页的网站图标</Text>
                      </>
                    ),
                    key: '0-0-6',
                  },
                  {
                    title: (
                      <>
                        <Text>dateLastCrawled</Text>
                        <Text code>String 网页的发布时间</Text>
                      </>
                    ),
                    key: '0-0-7',
                  },
                ],
              },
            ]}
          />
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
            <Form.Item noStyle name='isIgnoreExceptions'>
              <Switch
                size='small'
                style={{ marginLeft: '5px', marginRight: '10px' }}
              />
            </Form.Item>
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

export default WebSearchSettings
