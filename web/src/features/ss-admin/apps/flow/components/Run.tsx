import { useState } from 'react'
import { NodeRunning } from '@/dto'
import { RunVariable } from '@/dto/runVariable'
import { NodeType } from '@/enums'
import { VariableDataType } from '@/enums/variableDataType'
import { getSeconds } from '@/utils'
import { QuestionCircleOutlined, PlayCircleFilled } from '@ant-design/icons'
import {
  Button,
  Collapse,
  Typography,
  Row,
  Tooltip,
  Flex,
  Input,
  Card,
  Alert,
  Form,
} from 'antd'
import flowApi from '@/api/admin/apps/flowApi'
import { useStore } from '../store'
import NodeIcon from './NodeIcon'
import VariableList from './VariableList'

const { Text } = Typography

const Run: React.FC<{
  id: string
  type: NodeType
  onSubmit: () => void
  onSave: (isRun: boolean) => void
}> = ({ id, type, onSubmit, onSave }) => {
  const store = useStore()
  const initialValues: Record<string, string> = {}
  const settings = store.getNodeSettings(id)
  const nodeInVariables = store.getNodeInVariables(id)
  const nodeRunning = store.getNodeRunning(id)
  const [inVariables, setInVariables] = useState<RunVariable[]>(
    (nodeInVariables || []).map((v) => {
      const value =
        (nodeRunning?.inVariables || []).find((x) => x.name === v.name)
          ?.value || {}
      if (v.name) {
        initialValues[v.name] = JSON.stringify(value)
      }
      return {
        name: v.name || '',
        dataType: v.dataType || VariableDataType.String,
        value: value,
      }
    })
  )
  const [isRunning, setIsRunning] = useState(false)
  const [running, setRunning] = useState<NodeRunning | undefined>(nodeRunning)

  const handleRun = async () => {
    if (isRunning) return
    setIsRunning(true)

    try {
      await onSubmit()
      await onSave(true)
      const startTime = new Date()
      store.onRunning(id)
      const res = await flowApi.run({
        siteId: store.siteId,
        nodeId: id,
        inVariables,
      })
      if (res) {
        const newRunning = {
          isRun: true,
          isRunning: false,
          inVariables,
          success: res.success,
          outVariables: res.outVariables,
          errorMessage: res.errorMessage,
          seconds: getSeconds(startTime),
        }
        setRunning(newRunning)
        store.onRunned(id, newRunning)
      } else {
        const newRunning = {
          isRun: true,
          isRunning: false,
          inVariables,
          success: false,
          errorMessage: '运行失败',
          outVariables: [],
          seconds: getSeconds(startTime),
        }
        setRunning(newRunning)
        store.onRunned(id, newRunning)
      }
      setIsRunning(false)
    } catch {
      /* empty */
    }
  }

  const items = [
    {
      key: '1',
      label: (
        <Row align='middle'>
          <Text type='secondary' strong>
            试运行输入
          </Text>
          <Tooltip title='手动设置运行输入'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </Row>
      ),
      children: (
        <Card
          size='small'
          style={{ width: '100%' }}
          title={
            <div>
              <NodeIcon type={type} isFixed={settings.isFixed} />
              <span>{settings.title}</span>
            </div>
          }
        >
          {inVariables.map((variable, index) => {
            return (
              <Form.Item
                key={`in-${variable.name}-${index}`}
                name={variable.name}
                label={
                  <>
                    <Text>{variable.name}</Text>
                    <Text code>{variable.dataType}</Text>
                  </>
                }
                rules={[
                  {
                    required: true,
                    message: '请输入变量值',
                  },
                ]}
              >
                <Input.TextArea
                  placeholder='请输入变量值'
                  autoSize={{ minRows: 1, maxRows: 6 }}
                  onChange={(e) => {
                    const newVariables = inVariables.map((v) =>
                      v === variable
                        ? {
                            ...v,
                            value: e.target.value,
                          }
                        : v
                    )
                    setInVariables(newVariables)
                  }}
                />
              </Form.Item>
            )
          })}
        </Card>
      ),
    },
  ]

  if (running && running.outVariables) {
    items.push({
      key: '2',
      label: (
        <Row align='middle'>
          <Text type='secondary' strong>
            输出
          </Text>
        </Row>
      ),
      children: (
        <>
          {!running.success && (
            <Alert
              message='运行出错'
              description={running.errorMessage}
              type='error'
              showIcon
            />
          )}
          <VariableList variables={running.outVariables} />
        </>
      ),
    })
  }

  return (
    <Flex gap='middle' vertical style={{ margin: 0 }}>
      <Collapse
        items={items}
        bordered={false}
        collapsible='icon'
        defaultActiveKey={['1', '2']}
      />
      <Button
        color='primary'
        style={{ background: '#52c41a', margin: '0 15px 15px' }}
        variant='solid'
        size='small'
        onClick={handleRun}
        loading={isRunning}
      >
        {!isRunning && <PlayCircleFilled />}
        {isRunning ? '运行中...' : '运行'}
      </Button>
    </Flex>
  )
}

export default Run
