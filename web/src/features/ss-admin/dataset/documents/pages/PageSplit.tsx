import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { defaultChunkConfig } from '@/config'
import { Document, ChunkConfig } from '@/models'
import { getFileSize } from '@/utils'
import {
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  SearchOutlined,
  SettingOutlined,
  DeleteOutlined,
  PlusCircleOutlined,
  SelectOutlined,
  DownloadOutlined,
} from '@ant-design/icons'
import {
  Segmented,
  Input,
  Empty,
  Splitter,
  Button,
  Space,
  Table,
  Typography,
  Tag,
  Modal,
  Tooltip,
  Row,
  App,
  Skeleton,
  Cascader,
} from 'antd'
import datasetDocumentsApi from '@/api/admin/dataset/datasetDocumentsApi'
import FileIcon from '@/components/file-icon'
import Segments from '../components/Segments'
import Settings from '../components/Settings'
import { useDocumentsStore } from '../store/documentsStore'

const { Text } = Typography

const PageSplit: React.FC<{
  onOpenDocument: (documentId: number, isPreview: boolean) => void
}> = ({ onOpenDocument }) => {
  const { message, modal } = App.useApp()
  const store = useDocumentsStore()
  const [selectedRowKeys, setSelectedRowKeys] = useState<number[]>([])
  const [showLeftPanel, setShowLeftPanel] = useState(true)
  const [documents, setDocuments] = useState<Document[]>([])
  const [documentsTotal, setDocumentsTotal] = useState(0)
  const [documentChannelIds, setDocumentChannelIds] = useState<number[]>([])
  const [documentSearch, setDocumentSearch] = useState('')
  const [documentsPage, setDocumentsPage] = useState(1)
  const [document, setDocument] = useState<Document | null>(null)
  const [config, setConfig] = useState<ChunkConfig | null>(null)
  const [isConfig, setIsConfig] = useState(false)

  const { isPending, refetch } = useQuery({
    queryKey: [
      'ss-admin',
      'documents',
      'split',
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
    setSelectedRowKeys([document.id])
    setDocument(document)
  }

  const handleDocumentRemove = async () => {
    datasetDocumentsApi.remove({ documentId: document?.id || 0 })
    setDocuments(documents.filter((item) => item.id !== document?.id))
    setDocument(null)
    message.success('文档已删除')
  }

  const handleChunkAndEmbed = async () => {
    setIsConfig(false)
    store.setLoading(true)
    const res = await datasetDocumentsApi.chunkAndEmbed({
      siteId: store.siteId,
      channelId: store.channelId,
      contentId: store.contentId,
      documentId: document?.id || 0,
      config: config || defaultChunkConfig,
    })
    if (res) {
      setDocument({ ...res.document })
      handleDocumentClick(res.document)
      message.success('操作成功')
    }
    store.setLoading(false)
  }

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <>
      <Splitter
        style={{
          height: '100hv',
          minHeight: '550px',
          boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)',
        }}
      >
        {showLeftPanel && (
          <Splitter.Panel
            defaultSize='20%'
            min='0'
            max='70%'
            style={{ backgroundColor: '#fff' }}
          >
            <div className='flex-container'>
              <Segmented
                value='split'
                options={[
                  { label: '左右布局', value: 'split' },
                  { label: '列表布局', value: 'list' },
                  { label: '卡片布局', value: 'card' },
                ]}
                onChange={(value) => {
                  store.setPage(value as 'split' | 'list' | 'card')
                }}
              />
              {document && (
                <Button
                  type='text'
                  icon={<MenuFoldOutlined />}
                  onClick={() => {
                    setShowLeftPanel(false)
                  }}
                />
              )}
            </div>
            <div className='dropdown-container'>
              <Button
                block
                type='primary'
                ghost
                style={{ marginBottom: '6px' }}
                onClick={() => {
                  store.setPage('importing')
                }}
              >
                <Space>
                  <PlusCircleOutlined />
                  添加文档
                </Space>
              </Button>

              {store.channelId === 0 && (
                <Cascader
                  placeholder='选择栏目'
                  variant='outlined'
                  changeOnSelect
                  expandTrigger='hover'
                  style={{ marginBottom: '6px', width: '100%' }}
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
                style={{ marginBottom: '6px' }}
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

              <Table
                size='small'
                rowSelection={{
                  type: 'radio',
                  selectedRowKeys,
                  onChange: (keys) => {
                    setSelectedRowKeys(keys as number[])
                  },
                }}
                onRow={(document) => ({
                  onClick: () => {
                    handleDocumentClick(document)
                  },
                })}
                showHeader={false}
                columns={[
                  {
                    title: 'Name',
                    dataIndex: 'id',
                    render: (_, document) => (
                      <a>
                        <FileIcon type={document.extName} />
                        <span className='document-title'>{document.title}</span>
                      </a>
                    ),
                  },
                ]}
                rowKey='id'
                dataSource={documents}
                pagination={{
                  position: ['bottomCenter'],
                  pageSize: 28,
                  current: documentsPage,
                  showSizeChanger: false,
                  showQuickJumper: false,
                  showTotal: (total) => `共 ${total} 条`,
                  total: documentsTotal,
                  hideOnSinglePage: true,
                  onChange: (page) => {
                    setDocumentsPage(page)
                  },
                }}
              />
            </div>
          </Splitter.Panel>
        )}

        <Splitter.Panel
          defaultSize={showLeftPanel ? '80%' : '100%'}
          min='20%'
          max='100%'
          style={{ backgroundColor: '#fff' }}
        >
          {!document && (
            <Row
              align={'middle'}
              justify={'center'}
              style={{ width: '100%', height: '100%' }}
            >
              <Empty
                image={Empty.PRESENTED_IMAGE_SIMPLE}
                description='请选择一个文档'
              />
            </Row>
          )}
          {document && (
            <>
              <div className='document-header'>
                <Space>
                  {!showLeftPanel && (
                    <Button
                      type='text'
                      icon={<MenuUnfoldOutlined />}
                      onClick={() => setShowLeftPanel(true)}
                    />
                  )}

                  <FileIcon type={document.extName} />
                  <Text strong>{document.title}</Text>
                  <Tag color='green'>{getFileSize(document.fileSize)}</Tag>
                </Space>
                <Space>
                  <Button
                    color='primary'
                    style={{ fontSize: '12px' }}
                    onClick={() => {
                      onOpenDocument(document.id, false)
                    }}
                  >
                    下载文档
                    <DownloadOutlined />
                  </Button>
                  <Button
                    color='primary'
                    style={{ fontSize: '12px' }}
                    onClick={() => {
                      onOpenDocument(document.id, true)
                    }}
                  >
                    预览文档（Markdown格式）
                    <SelectOutlined />
                  </Button>

                  <Tooltip title='查看或调整配置' placement='left'>
                    <Button
                      icon={<SettingOutlined />}
                      type='text'
                      onClick={() => {
                        setIsConfig(true)
                        const separators = document.separators?.split(',')
                        setConfig({
                          separators: separators || [],
                          chunkSize:
                            document.chunkSize || defaultChunkConfig.chunkSize,
                          chunkOverlap:
                            document.chunkOverlap ||
                            defaultChunkConfig.chunkOverlap,
                          isChunkReplaces:
                            document.isChunkReplaces ||
                            defaultChunkConfig.isChunkReplaces,
                          isChunkDeletes:
                            document.isChunkDeletes ||
                            defaultChunkConfig.isChunkDeletes,
                        })
                      }}
                    />
                  </Tooltip>
                  <Tooltip title='删除文档' placement='left'>
                    <Button
                      icon={<DeleteOutlined />}
                      type='text'
                      onClick={() => {
                        modal.confirm({
                          title: '确定删除该文档吗？',
                          content: '删除后无法恢复该文档。',
                          okText: '确定删除',
                          okType: 'danger',
                          cancelText: '取消',
                          onOk: () => {
                            handleDocumentRemove()
                          },
                        })
                      }}
                    />
                  </Tooltip>
                </Space>
              </div>

              <Segments document={document} />
            </>
          )}
        </Splitter.Panel>
      </Splitter>
      <Modal
        title='查看或调整配置'
        centered
        footer={[
          <Button key='cancel' onClick={() => setIsConfig(false)}>
            取消
          </Button>,
          <Button
            key='confirm'
            type='primary'
            onClick={() => {
              modal.confirm({
                title: '确定对该文档重新分段吗？',
                content:
                  '系统将根据原始文档重新分段，重新分段后系统将无法保留已编辑或新增的分段内容。',
                okText: '确定重新分段',
                cancelText: '取消',
                onOk: () => {
                  handleChunkAndEmbed()
                },
              })
            }}
          >
            重新分段
          </Button>,
        ]}
        open={isConfig}
        onCancel={() => setIsConfig(false)}
      >
        <Settings
          config={config}
          onChange={(allValues) => setConfig(allValues)}
        ></Settings>
      </Modal>
    </>
  )
}

export default PageSplit
