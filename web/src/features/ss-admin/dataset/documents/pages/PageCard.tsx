import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Document } from '@/models'
import { SearchOutlined, PlusCircleOutlined } from '@ant-design/icons'
import {
  Segmented,
  Button,
  Space,
  Row,
  Skeleton,
  Layout,
  Divider,
  Col,
  Input,
  Pagination,
  Card,
  Cascader,
} from 'antd'
import dayjs from 'dayjs'
import datasetDocumentsApi from '@/api/admin/dataset/datasetDocumentsApi'
import { getFileSize } from '@/utils/getFileSize'
import FileIcon from '@/components/file-icon'
import { useDocumentsStore } from '../store/documentsStore'

const { Header, Content } = Layout

const PageCard: React.FC = () => {
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
      'card',
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
              value='card'
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
        <Row gutter={[16, 16]}>
          {documents.map((document: Document) => (
            <Col
              key={document.id}
              xs={24}
              sm={12}
              md={8}
              lg={6}
              onClick={() => handleDocumentClick(document)}
              style={{ cursor: 'pointer' }}
            >
              <Card
                hoverable
                title={
                  <Button style={{ paddingLeft: 0 }} type='link'>
                    <FileIcon type={document.extName} />
                    <span
                      style={{ color: '#000', fontSize: 16, fontWeight: 500 }}
                    >
                      {document.title}
                    </span>
                  </Button>
                }
                variant='outlined'
                style={{ height: '100%' }}
              >
                <p>
                  更新时间：
                  {dayjs(document.lastModifiedDate).format(
                    'YYYY-MM-DD HH:mm:ss'
                  )}
                </p>
                <p>文档大小：{getFileSize(document.fileSize)}</p>
              </Card>
            </Col>
          ))}
        </Row>
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

export default PageCard
