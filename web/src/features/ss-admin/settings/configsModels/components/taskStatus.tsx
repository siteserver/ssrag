import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Progress, Typography, Row, Button, Result } from 'antd'
import configsModelsApi from '@/api/admin/settings/configsModelsApi'

const { Text } = Typography

const TaskStatus: React.FC<{
  taskId: string
  setTaskId: (taskId: string | null) => void
}> = ({ taskId, setTaskId }) => {
  const [state, setState] = useState('PROGRESS')
  const [result, setResult] = useState<{
    count: number
    current: number
  }>({
    count: 1,
    current: 0,
  })

  useQuery({
    queryKey: ['ss-admin', 'settings', 'configsModels', 'taskStatus', taskId],
    queryFn: async () => {
      apiStatus()
    },
  })

  const apiStatus = async () => {
    const res = await configsModelsApi.status({
      taskId,
    })
    if (res) {
      setResult(res.result)
      if (res.state !== 'SUCCESS') {
        setTimeout(() => {
          apiStatus()
        }, 3000)
      } else {
        setState('SUCCESS')
      }
    }
  }

  const handleSubmit = async () => {
    setTaskId(null)
  }

  return (
    <div className='w-full max-w-[800px] rounded-2xl bg-white p-8'>
      {state === 'SUCCESS' && (
        <>
          <Result status='success' title='知识库文本嵌入重建成功！' />
          <Row justify='center' align='middle' style={{ marginTop: 10 }}>
            <Button onClick={handleSubmit} type='primary'>
              确认
            </Button>
          </Row>
        </>
      )}
      {state === 'PROGRESS' && (
        <>
          <Row justify='center' align='middle' style={{ marginTop: 10 }}>
            <Progress
              percent={Number(
                ((result.current / result.count) * 100).toFixed(2)
              )}
            />
            <Text type='secondary'>知识库文本嵌入重建中...</Text>
          </Row>
          <Row justify='center' align='middle' style={{ marginTop: 10 }}>
            <Button onClick={handleSubmit} type='primary'>
              确认
            </Button>
          </Row>
        </>
      )}
    </div>
  )
}

export default TaskStatus
