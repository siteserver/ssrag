import { useState } from 'react'
import { defaultChunkConfig } from '@/config'
import { Document, ChunkConfig } from '@/models'
import {
  SettingOutlined,
  DeleteOutlined,
  SelectOutlined,
  DownloadOutlined,
  LeftOutlined,
} from '@ant-design/icons'
import {
  Button,
  Space,
  Tooltip,
  Row,
  App,
  Layout,
  Divider,
  Typography,
  Col,
  Modal,
  Tag,
} from 'antd'
import datasetDocumentsApi from '@/api/admin/dataset/datasetDocumentsApi'
import { getFileSize } from '@/utils/getFileSize'
import FileIcon from '@/components/file-icon'
import Segments from '../components/Segments'
import Settings from '../components/Settings'
import { useDocumentsStore } from '../store/documentsStore'

const { Header, Content } = Layout
const { Text } = Typography

const PageView: React.FC<{
  onOpenDocument: (documentId: number, isPreview: boolean) => void
  document: Document
}> = ({ onOpenDocument, document }) => {
  const { message, modal } = App.useApp()
  const store = useDocumentsStore()
  const [config, setConfig] = useState<ChunkConfig | null>(null)
  const [isConfig, setIsConfig] = useState(false)

  const handleDocumentRemove = async () => {
    datasetDocumentsApi.remove({ documentId: document?.id || 0 })
    store.setDocument(null)
    store.goBack()
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
      store.setDocument(res.document)
      message.success('操作成功')
    }
    store.setLoading(false)
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
          <Col span={3}>
            <Space>
              <Button
                color='default'
                variant='filled'
                icon={<LeftOutlined />}
                onClick={() => {
                  store.goBack()
                }}
              >
                <Text strong style={{ fontSize: '16px' }}>
                  返回
                </Text>
              </Button>
            </Space>
          </Col>
          <Col span={13} style={{ textAlign: 'center' }}>
            <Space>
              <FileIcon type={document.extName} />
              <Text strong>{document.title}</Text>
              <Tag color='green'>{getFileSize(document.fileSize)}</Tag>
            </Space>
          </Col>
          <Col span={8} style={{ textAlign: 'right', paddingRight: 10 }}>
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
          </Col>
        </Row>

        <Divider style={{ marginTop: 8, borderColor: '#d9d9d9' }}></Divider>
      </Header>
      <Content
        style={{
          minHeight: 120,
          backgroundColor: '#fff',
        }}
      >
        <Segments document={document} />

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
      </Content>
    </Layout>
  )
}

export default PageView
