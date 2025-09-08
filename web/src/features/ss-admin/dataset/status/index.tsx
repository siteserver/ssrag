import { useEffect, useState } from 'react'
import { TaskStatus } from '@/enums'
import { CeleryTask } from '@/models'
import { getFileSize, getQueryInt } from '@/utils'
import { Progress, Card, Statistic } from 'antd'
import datasetStatusApi from '@/api/admin/dataset/datasetStatusApi'
import FileIcon from '@/components/file-icon'
import './app.css'

const { Meta } = Card

const siteId = getQueryInt('siteId')

const Status: React.FC = () => {
  const [pendingCount, setPendingCount] = useState<number>(0)
  const [documentCount, setDocumentCount] = useState<number>(0)
  const [processingTask, setProcessingTask] = useState<CeleryTask | null>(null)
  const [handledTasks, setHandledTasks] = useState<CeleryTask[]>([])

  const apiGet = async () => {
    const res = await datasetStatusApi.get({
      siteId,
    })
    if (res) {
      setPendingCount(res.pendingCount)
      setDocumentCount(res.documentCount)
      setProcessingTask(null)
      for (const task of res.tasks) {
        const rawResult = task?.taskResult
        if (rawResult && typeof rawResult === 'string') {
          try {
            const jsonStr = rawResult.replace(/'/g, '"')
            task.resultObj = JSON.parse(jsonStr)
          } catch {
            task.resultObj = null
          }
        }

        if (
          task.taskStatus === TaskStatus.SUCCESS ||
          task.taskStatus === TaskStatus.FAILURE
        ) {
          handledTasks.push(task)
          setHandledTasks(handledTasks)
        } else if (task.taskStatus === TaskStatus.PROGRESS) {
          setProcessingTask(task)
        }
      }
    }
  }

  useEffect(() => {
    apiGet()
    const interval = setInterval(() => {
      apiGet()
    }, 3000)
    return () => clearInterval(interval)
  }, [])

  return (
    <div className='mb-3 p-3'>
      <div className='mb-3 flex justify-center'>
        <Card
          variant='borderless'
          style={{ width: '300px', marginRight: '10px' }}
          className='mr-4 text-center'
        >
          <Statistic title='队列任务数' value={pendingCount} />
        </Card>
        <Card
          variant='borderless'
          style={{ width: '300px', marginRight: '10px' }}
          className='text-center'
        >
          <Statistic title='已分段知识库文档' value={documentCount} />
        </Card>
      </div>
      <div className='mb-3'>
        {processingTask && (
          <Card
            key={processingTask.taskId}
            size='small'
            style={{ width: '100%', marginBottom: 10 }}
          >
            <Meta
              avatar={
                <FileIcon
                  type={processingTask.resultObj?.extName || ''}
                  className='h-10 w-10'
                />
              }
              title={
                (processingTask.resultObj?.fileName || '') +
                (processingTask.resultObj?.extName || '')
              }
              description={getFileSize(processingTask.resultObj?.fileSize || 0)}
            />
            <Progress percent={50} />
          </Card>
        )}
      </div>
      <div className='mb-3'>
        {handledTasks.map((task) => {
          if (!task.resultObj) return null
          return (
            <Card
              key={task.taskId}
              size='small'
              style={{ width: '100%', marginBottom: 10 }}
            >
              <Meta
                avatar={
                  <FileIcon
                    type={task.resultObj?.extName || ''}
                    className='h-10 w-10'
                  />
                }
                title={
                  (task.resultObj?.fileName || '') +
                  (task.resultObj?.extName || '')
                }
                description={getFileSize(task.resultObj?.fileSize || 0)}
              />
              <Progress percent={100} />
            </Card>
          )
        })}
      </div>

      {/* {pendingCount === 0 && (
        <Result
          status='success'
          title='数据处理成功'
          subTitle='暂无需要分段的数据'
        />
      )}
      {pendingCount !== undefined && pendingCount > 0 && (
        
      )} */}
    </div>
  )
}

export default Status
