import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Segment, Document, ChannelSummary } from '@/models'
import { Breadcrumb, message, Pagination, Skeleton } from 'antd'
import '@/assets/github-markdown.css'
import datasetSegmentsApi from '@/api/admin/dataset/datasetSegmentsApi'
import { useDocumentsStore } from '../store/documentsStore'
import SegmentItem from './SegmentItem'

const Segments: React.FC<{
  document: Document
}> = ({ document }) => {
  const store = useDocumentsStore()
  const [segments, setSegments] = useState<Segment[]>([])
  const [total, setTotal] = useState(0)
  const [breadcrumb, setBreadcrumb] = useState<ChannelSummary[]>([])
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)

  const handleSegmentSubmit = async (
    segment: Segment,
    op: string,
    value: string,
    index: number
  ) => {
    store.setLoading(true)
    if (op === 'remove') {
      op = 'edit'
      value = ''
    }
    if (op === 'edit') {
      datasetSegmentsApi.update({
        documentId: segment.documentId,
        segmentId: segment.id,
        content: value,
      })
      let sgs = [...segments]
      if (!value) {
        sgs = sgs.filter((item) => item.id !== segment.id)
        setSegments(sgs)
      } else {
        sgs[index] = { ...segment, text: value }
        setSegments(sgs)
      }
      // const ds = store.documents.map((item) => {
      //   if (item.id === document?.id) {
      //     item.segments = sgs
      //   }
      //   return item
      // })
      // store.setDocuments(ds)
      message.success('操作成功')
    } else if (op === 'add') {
      if (!value) return
      datasetSegmentsApi.insert({
        documentId: segment.documentId,
        taxis: segment.taxis,
        content: value,
      })
      const sgs = [...segments]
      sgs.splice(index + 1, 0, {
        ...segment,
        text: value,
      })
      setSegments(sgs)
      // const ds = dataset.map((item) => {
      //   if (item.id === document.id) {
      //     item.segments = sgs;
      //   }
      //   return item;
      // });
      // setDataset(ds);
      message.success('操作成功')
    }
    store.setLoading(false)
  }

  const { isPending } = useQuery({
    queryKey: [
      'ss-admin',
      'documents',
      'segments',
      document.siteId,
      document.channelId,
      document.id,
      page,
      pageSize,
    ],
    queryFn: async () => {
      const res = await datasetSegmentsApi.get({
        siteId: document.siteId,
        channelId: document.channelId,
        documentId: document.id,
        page,
        pageSize,
      })
      if (res) {
        setSegments(res.segments)
        setTotal(res.count)
        setBreadcrumb(res.breadcrumb)
      }
      return res
    },
    refetchOnMount: 'always',
  })

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <div className='document-container'>
      <Breadcrumb
        items={breadcrumb.map((item) => {
          return {
            title: item.channelName,
          }
        })}
      />
      <div id='document-content' className='document-content'>
        <div>
          {segments.map((segment, index) => {
            return (
              <SegmentItem
                key={index + '-' + segment.id}
                siteUrl={store.siteUrl}
                segment={segment}
                index={index}
                onSubmit={handleSegmentSubmit}
              ></SegmentItem>
            )
          })}
        </div>
        <div className='document-content-pagination'>
          <Pagination
            current={page}
            total={total}
            pageSize={pageSize}
            showSizeChanger
            showQuickJumper
            align='center'
            hideOnSinglePage={true}
            showTotal={(total, range) =>
              `第 ${range[0]}-${range[1]} 条，共 ${total} 条`
            }
            onChange={async (page, pageSize) => {
              store.setLoading(true)
              const res = await datasetSegmentsApi.get({
                siteId: document.siteId,
                channelId: document.channelId,
                documentId: document?.id || 0,
                page,
                pageSize,
              })
              if (res) {
                setPage(page)
                setPageSize(pageSize)
                setSegments(res.segments)
                setTotal(res.count)
                const targetElement =
                  window.document.getElementById('document-content')
                if (targetElement) {
                  targetElement.scrollIntoView({ behavior: 'smooth' })
                }
              }
              store.setLoading(false)
            }}
          />
        </div>
      </div>
    </div>
  )
}

export default Segments
