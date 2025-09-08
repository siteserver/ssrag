import { useEffect, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { SearchType } from '@/enums'
import { SiteSummary } from '@/models'
import { QuestionCircleOutlined, DownOutlined } from '@ant-design/icons'
import {
  Collapse,
  Typography,
  Row,
  Col,
  Tooltip,
  Flex,
  Tree,
  Switch,
  Form,
  FormInstance,
  Skeleton,
} from 'antd'
import flowApi from '@/api/admin/apps/flowApi'
import SettingsDataset from '../../components/SettingsDataset'
import SettingsSearching from '../../components/SettingsSearching'
import VariableEditor from '../components/VariableEditor'
import VariableReference from '../components/VariableReference'
import { useStore } from '../store'

const { Text } = Typography

const DatasetSettings: React.FC<{
  nodeId: string
  form: FormInstance
}> = ({ nodeId, form }) => {
  const store = useStore()

  const settings = store.getNodeSettings(nodeId)
  const [datasetSites, setDatasetSites] = useState<SiteSummary[]>([])

  const { isPending } = useQuery({
    queryKey: ['flow', 'settings', 'DatasetSettings', store.siteId, nodeId],
    queryFn: async () => {
      const res = await flowApi.dataset({
        siteId: store.siteId,
        nodeId,
      })
      if (res) {
        setDatasetSites(res.datasetSites)
      }
      return res
    },
  })

  useEffect(() => {
    form.setFieldsValue({
      datasetSearchType: settings.datasetSearchType || SearchType.Semantic,
      datasetMaxCount: settings.datasetMaxCount || 5,
      datasetMinScore: settings.datasetMinScore || 0.5,
      isIgnoreExceptions: settings.isIgnoreExceptions || false,
    })
  }, [settings, form])

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
              style={{
                marginLeft: '8px',
                color: '#999',
                cursor: 'pointer',
              }}
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
          <Typography.Text type='secondary' strong>
            知识库
          </Typography.Text>
          <Tooltip title='添加需要被检索的知识库'>
            <QuestionCircleOutlined
              style={{
                marginLeft: '8px',
                color: '#999',
                cursor: 'pointer',
              }}
            />
          </Tooltip>
        </Row>
      ),
      children: (
        <SettingsDataset
          siteId={store.siteId}
          nodeId={nodeId}
          datasetSites={datasetSites}
          onChange={(datasetSites) => {
            setDatasetSites(datasetSites)
          }}
        />
      ),
    },
    {
      key: '3',
      label: (
        <Row align='middle'>
          <Text type='secondary' strong>
            检索设置
          </Text>
          <Tooltip title='知识库检索设置'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </Row>
      ),
      children: <SettingsSearching />,
    },
    {
      key: '4',
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
                        <Text>value</Text>
                        <Text code>String</Text>
                      </>
                    ),
                    key: '0-0-1',
                  },
                  {
                    title: (
                      <>
                        <Text>documentId</Text>
                        <Text code>Number</Text>
                      </>
                    ),
                    key: '0-0-2',
                  },
                ],
              },
            ]}
          />
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

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

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

export default DatasetSettings
