import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { LoadingOutlined } from '@ant-design/icons'
import { Spin, Modal, Button, Skeleton } from 'antd'
import datasetDocumentsApi from '@/api/admin/dataset/datasetDocumentsApi'
import AdaptiveHeightEditor from '@/components/adaptive-height-editor'
import ConfigsModelsApp from '../../settings/configsModels/configsModelsApp'
import './app.css'
import PageCard from './pages/PageCard'
import PageImporting from './pages/PageImporting'
import PageList from './pages/PageList'
import PageSplit from './pages/PageSplit'
import PageView from './pages/PageView'
import { useDocumentsStore } from './store/documentsStore'

const Documents: React.FC = () => {
  const store = useDocumentsStore()
  const [modalConfigsModels, setModalConfigsModels] = useState(false)
  const [markdown, setMarkdown] = useState('')
  const [isPreview, setIsPreview] = useState(false)

  const { isPending } = useQuery({
    queryKey: [
      'ss-admin',
      'documents',
      store.siteId,
      store.channelId,
      store.contentId,
    ],
    queryFn: async () => {
      const res = await datasetDocumentsApi.get({
        siteId: store.siteId,
        channelId: store.channelId,
        contentId: store.contentId,
      })
      if (res) {
        setModalConfigsModels(res.isModelReady === false)
        store.init(res.siteUrl, res.channels)
      }
      return res
    },
  })

  const handleOpenDocument = async (documentId: number, isPreview: boolean) => {
    store.setLoading(true)
    if (isPreview) {
      const res = await datasetDocumentsApi.getMarkdown({
        documentId: documentId,
      })
      if (res) {
        setMarkdown(res.value)
        setIsPreview(true)
      }
    } else {
      const res = await datasetDocumentsApi.download({
        documentId: documentId,
      })
      if (res) {
        window.open(res.value, '_blank')
      }
    }
    store.setLoading(false)
  }

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <div className='dataset-container'>
      <Spin
        spinning={store.loading}
        indicator={
          <LoadingOutlined
            style={{
              fontSize: 48,
            }}
            spin
          />
        }
        fullscreen
      />
      {store.page === 'split' && (
        <PageSplit onOpenDocument={handleOpenDocument}></PageSplit>
      )}
      {store.page === 'list' && (
        <PageList onOpenDocument={handleOpenDocument}></PageList>
      )}
      {store.page === 'card' && <PageCard></PageCard>}
      {store.page === 'view' && store.document && (
        <PageView
          document={store.document}
          onOpenDocument={handleOpenDocument}
        ></PageView>
      )}
      {store.page === 'importing' && <PageImporting></PageImporting>}
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
      <Modal
        centered
        closable={false}
        open={isPreview}
        onCancel={() => setIsPreview(false)}
        width={'100%'}
        styles={{
          body: {
            height: window.innerHeight - 140 + 'px',
            overflow: 'auto',
          },
        }}
        footer={
          <Button type='primary' onClick={() => setIsPreview(false)}>
            关 闭
          </Button>
        }
      >
        <AdaptiveHeightEditor
          height='100%'
          language='markdown'
          value={markdown}
          onChange={() => {}}
        />
      </Modal>
    </div>
  )
}

export default Documents
