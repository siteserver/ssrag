import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { TaskDocumentProcess } from '@/dto'
import { TaskStatus } from '@/enums'
import { getFileSize } from '@/utils'
import { Progress, Card, Typography, Row, Button, Divider, Result } from 'antd'
import datasetDocumentsApi from '@/api/admin/dataset/datasetDocumentsApi'
import FileIcon from '@/components/file-icon'
import { useDocumentsStore } from '../store/documentsStore'

const { Text } = Typography
const { Meta } = Card

const Importing2: React.FC<{
  tasks: TaskDocumentProcess[]
}> = ({ tasks }) => {
  const store = useDocumentsStore()
  const [state, setState] = useState(TaskStatus.PROGRESS)
  const [results, setResults] = useState<
    {
      taskId: string
      state: string
      detail: string
      result: {
        id: number
        fileName: string
        extName: string
        fileSize: number
        title: string
      } | null
    }[]
  >([])

  useQuery({
    queryKey: [
      'documents',
      store.siteId,
      store.channelId,
      store.contentId,
      tasks.map((task) => task.taskId),
    ],
    queryFn: async () => {
      apiStatus()
    },
  })

  const apiStatus = async () => {
    const res = await datasetDocumentsApi.status({
      taskIds: tasks.map((task) => task.taskId),
    })
    if (res) {
      setResults(res.results)
      if (
        res.results.some(
          (task) =>
            task.state === TaskStatus.PROGRESS ||
            task.state === TaskStatus.PENDING
        )
      ) {
        setTimeout(() => {
          apiStatus()
        }, 3000)
      } else {
        setState(
          res.results.some((task) => task.state === TaskStatus.FAILURE)
            ? TaskStatus.FAILURE
            : TaskStatus.SUCCESS
        )
      }
    }
  }

  const handleSubmit = async () => {
    const res = await datasetDocumentsApi.get({
      siteId: store.siteId,
      channelId: store.channelId,
      contentId: store.contentId,
    })
    if (res) {
      store.goBack()
      // store.setDocuments(res.documents)
      // store.setPage('documents')
    }
  }

  return (
    <>
      {/* {task.state === 'PENDING' && (
        <Result status='info' title='数据处理中...' />
      )}
      {task.state === 'PROGRESS' && (
        
      )}
      
       */}

      {state === TaskStatus.FAILURE && (
        <Result
          status='error'
          title={`数据处理失败：${
            results.find((result) => result.taskId === tasks[0].taskId)
              ?.result || ''
          }`}
          subTitle={
            results.find((result) => result.taskId === tasks[0].taskId)
              ?.detail || ''
          }
        />
      )}

      {state === TaskStatus.SUCCESS && (
        <Result status='success' title='数据处理成功！' />
      )}
      {state === TaskStatus.PROGRESS && (
        <>
          {tasks.map((task) => {
            const result = results.find(
              (result) => result.taskId === task.taskId
            )
            let percent = 0
            if (result?.state === TaskStatus.PROGRESS) {
              percent = 50
            } else if (result?.state === TaskStatus.SUCCESS) {
              percent = 100
            }
            return (
              <Card
                key={task.taskId}
                size='small'
                style={{ width: '100%', marginBottom: 10 }}
              >
                <Meta
                  avatar={
                    <FileIcon
                      type={task?.extName || ''}
                      style={{ width: 50, height: 50 }}
                    />
                  }
                  title={(task?.fileName || '') + (task?.extName || '')}
                  description={getFileSize(result?.result?.fileSize || 0)}
                />
                <Progress percent={percent} />
              </Card>
            )
          })}
          <Divider />

          <Row justify='center' align='middle' style={{ marginTop: 20 }}>
            <Text type='secondary'>点击确认不影响数据处理</Text>
          </Row>
        </>
      )}

      <Row justify='center' align='middle' style={{ marginTop: 10 }}>
        <Button onClick={handleSubmit} type='primary'>
          确认
        </Button>
      </Row>
    </>
  )
}

export default Importing2
