import { useState } from 'react'
import { TaskDocumentProcess } from '@/dto'
import { LeftOutlined } from '@ant-design/icons'
import { Button, Typography, Row, Steps, Divider, Layout } from 'antd'
import datasetDocumentsApi from '@/api/admin/dataset/datasetDocumentsApi'
import Importing1 from '../components/Importing1'
import Importing2 from '../components/Importing2'
import { useDocumentsStore } from '../store/documentsStore'

const { Text } = Typography
const { Step } = Steps
const { Header, Content } = Layout

function PageImporting() {
  const store = useDocumentsStore()
  const [step, setStep] = useState(0)
  const [tasks, setTasks] = useState<TaskDocumentProcess[]>([])

  const apiProcess = async (tasks: TaskDocumentProcess[]) => {
    store.setLoading(true)
    const res = await datasetDocumentsApi.process({
      tasks,
    })
    if (res) {
      const tasks2 = [...tasks]
      for (let i = 0; i < tasks2.length; i++) {
        tasks2[i].taskId = res.taskIds[i]
      }
      setTasks(tasks2)
      setStep(step + 1)
    }
    store.setLoading(false)
  }

  return (
    <Layout style={{ height: '100vh' }}>
      <Header
        style={{
          padding: 0,
          backgroundColor: '#fff',
        }}
      >
        <Row
          style={{
            marginTop: 7,
            marginLeft: 18,
            height: '34px',
            lineHeight: '34px',
          }}
        >
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
        </Row>
        <Divider style={{ marginTop: 5 }}></Divider>
      </Header>
      <Content
        style={{
          padding: '12px 20px',
          minHeight: 120,
          backgroundColor: '#fff',
        }}
      >
        <Row justify='center'>
          <div className='steps-container'>
            <Steps current={step}>
              <Step title='上传' />
              <Step title='数据处理' />
            </Steps>
          </div>
        </Row>
        {step === 0 && (
          <Importing1
            onSubmit={(tasks: TaskDocumentProcess[]) => {
              apiProcess(tasks)
            }}
          ></Importing1>
        )}
        {step === 1 && <Importing2 tasks={tasks}></Importing2>}
      </Content>
    </Layout>
  )
}

export default PageImporting
