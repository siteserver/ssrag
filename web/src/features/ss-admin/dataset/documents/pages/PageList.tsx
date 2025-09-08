import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Document } from '@/models'
import { SearchOutlined, PlusCircleOutlined } from '@ant-design/icons'
import {
  Segmented,
  Button,
  Space,
  Table,
  Row,
  App,
  Skeleton,
  Layout,
  Divider,
  Typography,
  Col,
  Input,
  Cascader,
  Pagination,
} from 'antd'
import dayjs from 'dayjs'
import datasetDocumentsApi from '@/api/admin/dataset/datasetDocumentsApi'
import { getFileSize } from '@/utils/getFileSize'
import FileIcon from '@/components/file-icon'
import { useDocumentsStore } from '../store/documentsStore'

const { Header, Content } = Layout
const { Text } = Typography

const PageList: React.FC<{
  onOpenDocument: (documentId: number, isPreview: boolean) => void
}> = ({ onOpenDocument }) => {
  const { message, modal } = App.useApp()
  const store = useDocumentsStore()
  const [documents, setDocuments] = useState<Document[]>([])
  const [documentsTotal, setDocumentsTotal] = useState(0)
  const [documentChannelIds, setDocumentChannelIds] = useState<number[]>([])
  const [documentSearch, setDocumentSearch] = useState('')
  const [documentsPage, setDocumentsPage] = useState(1)

  const { isPending, refetch } = useQuery({
    queryKey: [
      'ss-admin',
      'documents',
      'list',
      store.siteId,
      store.channelId,
      store.contentId,
      documentSearch,
      documentsPage,
    ],
    queryFn: async () => {
      const res = await datasetDocumentsApi.list({
        siteId: store.siteId,
        channelId: store.channelId,
        contentId: store.contentId,
        search: documentSearch,
        page: documentsPage,
      })
      if (res) {
        setDocuments(res.documents)
        setDocumentsTotal(res.total)
        setDocumentChannelIds(res.channelIds)
      }
      return res
    },
    refetchOnMount: 'always',
  })

  const handleDocumentClick = async (document: Document) => {
    store.setDocument(document)
  }

  const handleDocumentRemove = async (documentId: number) => {
    datasetDocumentsApi.remove({ documentId })
    setDocuments(documents.filter((item) => item.id !== documentId))
    store.setDocument(null)
    message.success('文档已删除')
  }

  const columns = [
    {
      title: '文档名称',
      render: (_: unknown, document: Document) => (
        <Button style={{ paddingLeft: 0 }} type='link'>
          <FileIcon type={document.extName} />
          <span style={{ color: '#000' }}>{document.title}</span>
        </Button>
      ),
    },
    {
      title: '文档大小',
      width: 100,
      render: (_: unknown, document: Document) => (
        <Text>{getFileSize(document.fileSize)}</Text>
      ),
    },
    {
      title: '更新时间',
      width: 150,
      render: (_: unknown, document: Document) => (
        <Text>
          {dayjs(document.lastModifiedDate).format('YYYY-MM-DD HH:mm:ss')}
        </Text>
      ),
    },
    {
      title: '操作',
      width: 100,
      render: (_: unknown, document: Document) => (
        <Space size={0}>
          <Button
            type='link'
            color='primary'
            onClick={(e) => {
              e.stopPropagation()
              onOpenDocument(document.id, false)
            }}
          >
            下载
          </Button>
          <Button
            type='link'
            color='primary'
            onClick={(e) => {
              e.stopPropagation()
              onOpenDocument(document.id, true)
            }}
          >
            预览
          </Button>

          <Button
            type='link'
            danger
            onClick={(e) => {
              e.stopPropagation()
              modal.confirm({
                title: '确定删除该文档吗？',
                content: '删除后无法恢复该文档。',
                okText: '确定删除',
                okType: 'danger',
                cancelText: '取消',
                onOk: () => {
                  handleDocumentRemove(document.id)
                },
              })
            }}
          >
            删除
          </Button>
        </Space>
      ),
    },
  ]

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <Layout>
      <Header
        style={{
          backgroundColor: '#fff',
          padding: 0,
          height: '46px',
        }}
      >
        <Row
          style={{
            marginTop: 7,
            marginLeft: 18,
            height: '30px',
            lineHeight: '30px',
          }}
        >
          <Col span={8}>
            <Segmented
              value='list'
              options={[
                { label: '左右布局', value: 'split' },
                { label: '列表布局', value: 'list' },
                { label: '卡片布局', value: 'card' },
              ]}
              onChange={(value) => {
                store.setPage(value as 'split' | 'list' | 'card')
              }}
            />
          </Col>
          <Col span={16} style={{ textAlign: 'right', paddingRight: 10 }}>
            <Space>
              {store.channelId === 0 && (
                <Cascader
                  placeholder='选择栏目'
                  variant='outlined'
                  changeOnSelect
                  expandTrigger='hover'
                  style={{ width: '300px', marginRight: 5, textAlign: 'left' }}
                  options={store.channels}
                  value={documentChannelIds}
                  onChange={(value) => {
                    store.setChannelId(value[value.length - 1] as number)
                    setDocumentsPage(1)
                    refetch()
                  }}
                />
              )}

              <Input
                placeholder='搜索'
                prefix={<SearchOutlined />}
                style={{ width: '200px', marginRight: 5 }}
                defaultValue={documentSearch}
                onPressEnter={(e) => {
                  setDocumentSearch(e.currentTarget.value)
                  setDocumentsPage(1)
                }}
                onBlur={(e) => {
                  setDocumentSearch(e.currentTarget.value)
                  setDocumentsPage(1)
                }}
              />
              <Button
                type='primary'
                ghost
                onClick={() => {
                  store.setPage('importing')
                }}
              >
                <PlusCircleOutlined />
                添加文档
              </Button>
            </Space>
          </Col>
        </Row>

        <Divider style={{ marginTop: 8, borderColor: '#d9d9d9' }}></Divider>
      </Header>
      <Content
        style={{
          padding: '10px 20px',
          minHeight: 120,
          backgroundColor: '#f0f2f5',
        }}
      >
        <Table
          size='small'
          columns={columns}
          onRow={(document) => ({
            onClick: () => {
              handleDocumentClick(document)
            },
          })}
          rowKey='id'
          dataSource={documents}
          bordered
          style={{
            width: '100%',
          }}
          scroll={{ x: 'max-content' }}
          pagination={false}
        />

        <div style={{ marginTop: 16, textAlign: 'center' }}>
          <Pagination
            current={documentsPage}
            align='center'
            pageSize={28}
            showSizeChanger={false}
            total={documentsTotal}
            showTotal={(total) => `共 ${total} 条`}
            hideOnSinglePage
            onChange={(page) => setDocumentsPage(page)}
          />
        </div>
      </Content>
    </Layout>
  )
}

export default PageList
