import { useState, useEffect } from 'react'
import { ReactFlowNode } from '@/dto'
import { NodeType } from '@/enums'
import { PlayCircleOutlined } from '@ant-design/icons'
import { Splitter, Button, Drawer, Input, Space, Form } from 'antd'
import NodeIcon from '../components/NodeIcon'
import Run from '../components/Run'
import { isNodeRunnable } from '../core'
import AskSettings from '../settings/AskSettings'
import DatasetSettings from '../settings/DatasetSettings'
import HttpSettings from '../settings/HttpSettings'
import InputSettings from '../settings/InputSettings'
import IntentSettings from '../settings/IntentSettings'
import LLMSettings from '../settings/LLMSettings'
import OutputSettings from '../settings/OutputSettings'
import SqlSettings from '../settings/SqlSettings'
import TextSettings from '../settings/TextSettings'
import WebSearchSettings from '../settings/WebSearchSettings'
import { useStore } from '../store'

const Settings: React.FC<{
  selectedNode: ReactFlowNode
  onClose: () => void
  onSave: (isRun: boolean) => void
}> = ({ selectedNode, onClose, onSave }) => {
  const store = useStore()
  const [form] = Form.useForm()
  const [isRun, setIsRun] = useState<boolean>(false)

  const settings = store.getNodeSettings(selectedNode.id)
  const running = store.getNodeRunning(selectedNode.id)

  useEffect(() => {
    if (running) {
      setIsRun(running.isRun || false)
    }
  }, [running])

  const handleRunOnOff = () => {
    setIsRun(!isRun)
    store.onRun(selectedNode)
  }

  const handleSubmit = async () => {
    try {
      await form.validateFields()
      const values = form.getFieldsValue()
      const variables = values.variables || []
      store.setNodeInVariables(selectedNode.id, variables)
      delete values.variables

      const settings = store.getNodeSettings(selectedNode.id)
      store.setNodeSettings(selectedNode.id, {
        ...settings,
        ...values,
      })

      const running = store.getNodeRunning(selectedNode.id)
      if (running) {
        store.setNodeRunning(selectedNode.id, {
          ...running,
          isRun: isRun,
        })
      }

      if (!isRun) {
        onClose()
      }
    } catch {
      /* empty */
    }
  }

  let element = null
  if (selectedNode.type == NodeType.Input) {
    element = <InputSettings nodeId={selectedNode.id} />
  } else if (selectedNode.type == NodeType.LLM) {
    element = <LLMSettings nodeId={selectedNode.id} form={form} />
  } else if (selectedNode.type == NodeType.Dataset) {
    element = <DatasetSettings nodeId={selectedNode.id} form={form} />
  } else if (selectedNode.type == NodeType.Output) {
    element = <OutputSettings nodeId={selectedNode.id} form={form} />
  } else if (selectedNode.type == NodeType.Http) {
    element = <HttpSettings nodeId={selectedNode.id} form={form} />
  } else if (selectedNode.type == NodeType.WebSearch) {
    element = <WebSearchSettings nodeId={selectedNode.id} form={form} />
  } else if (selectedNode.type == NodeType.Sql) {
    element = <SqlSettings nodeId={selectedNode.id} form={form} />
  } else if (selectedNode.type == NodeType.Text) {
    element = <TextSettings nodeId={selectedNode.id} form={form} />
  } else if (selectedNode.type == NodeType.Intent) {
    element = <IntentSettings nodeId={selectedNode.id} form={form} />
  } else if (selectedNode.type == NodeType.Ask) {
    element = <AskSettings nodeId={selectedNode.id} form={form} />
  }
  if (!element) {
    return null
  }

  if (isRun) {
    element = (
      <Splitter
        style={{ height: '100%', boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)' }}
      >
        <Splitter.Panel defaultSize='50%' min='20%' max='70%'>
          {element}
        </Splitter.Panel>
        <Splitter.Panel>
          <Run
            id={selectedNode.id}
            type={selectedNode.type as NodeType}
            onSubmit={handleSubmit}
            onSave={onSave}
          ></Run>
        </Splitter.Panel>
      </Splitter>
    )
  }

  if (!settings) {
    return null
  }

  return (
    <Drawer
      title={
        <>
          <NodeIcon
            type={selectedNode.type as NodeType}
            isFixed={settings.isFixed}
          />
          <Input
            placeholder='Filled'
            size='large'
            variant='borderless'
            value={settings.title}
            onChange={(e) => {
              store.setNodeSettings(selectedNode.id, {
                ...settings,
                title: e.target.value,
              })
            }}
            style={{
              width: '80%',
            }}
          />
        </>
      }
      onClose={onClose}
      open={true}
      width={isRun ? '60%' : '720px'}
      mask={false}
      destroyOnHidden={true}
      extra={
        <Space>
          {isRun && <Button onClick={handleRunOnOff}>关闭试运行</Button>}
          {!isRun && isNodeRunnable(selectedNode.type as NodeType) && (
            <Button
              icon={<PlayCircleOutlined />}
              onClick={handleRunOnOff}
              type='primary'
              style={{ marginRight: 5, background: '#52c41a' }}
            >
              试运行
            </Button>
          )}
          <Button onClick={handleSubmit} type='primary'>
            确 定
          </Button>
        </Space>
      }
    >
      <Form
        form={form}
        // onValuesChange={handleValuesChange}
        autoComplete='off'
        layout='vertical'
      >
        {element}
      </Form>
    </Drawer>
  )
}

export default Settings
