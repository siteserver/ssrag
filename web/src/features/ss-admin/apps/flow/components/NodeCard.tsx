import { useState } from 'react'
import { ReactFlowNode } from '@/dto'
import { NodeType } from '@/enums'
import { uuid } from '@/utils'
import {
  MoreOutlined,
  PlayCircleOutlined,
  LoadingOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
  DownOutlined,
} from '@ant-design/icons'
import { useReactFlow } from '@xyflow/react'
import {
  Card,
  Dropdown,
  Input,
  Tooltip,
  Typography,
  Space,
  Tag,
  Button,
  Divider,
  Alert,
  App,
} from 'antd'
import { isNodeRunnable } from '../core'
import { useStore } from '../store'
import NodeIcon from './NodeIcon'
import VariableList from './VariableList'

const { Text } = Typography

const NodeCard: React.FC<{
  id: string
  type: NodeType
  children: React.ReactNode
}> = ({ id, type, children }) => {
  const { modal } = App.useApp()
  const store = useStore()
  const settings = store.getNodeSettings(id)
  const [isCollapsed, setIsCollapsed] = useState(false)
  const { deleteElements, setNodes, getNode } = useReactFlow()
  const [isEditing, setIsEditing] = useState(false)
  const [newTitle, setNewTitle] = useState(settings.title)
  const running = store.getNodeRunning(id)

  const handleRename = () => {
    setIsEditing(true)
    setNewTitle(settings.title)
  }

  const handleTitleSubmit = () => {
    store.setNodeSettings(id, {
      ...settings,
      title: newTitle,
    })
    setIsEditing(false)
  }

  const handleCopy = () => {
    const currentNode = getNode(id)
    if (!currentNode || settings.isFixed) return

    const newNode = {
      ...currentNode,
      id: uuid(),
      position: {
        x: currentNode.position.x + 350,
        y: currentNode.position.y + 0,
      },
      selected: false,
    }
    const newSettings = {
      id: newNode.id,
      title: `${settings.title} 副本`,
    }
    store.setNodeSettings(newNode.id, newSettings)
    setNodes((nodes) => [...nodes, newNode])
  }

  const handleRun = () => {
    const currentNode = getNode(id)
    if (!currentNode) return
    store.onRun(currentNode as ReactFlowNode)
  }

  const handleDelete = () => {
    modal.confirm({
      title: '确认删除',
      content: '是否确认删除此节点？',
      onOk: () => {
        deleteElements({ nodes: [{ id }] })
      },
    })
  }

  const items = [
    {
      key: '1',
      label: '重命名',
      onClick: handleRename,
    },
    {
      key: '2',
      label: '创建副本',
      disabled: settings.isFixed,
      onClick: handleCopy,
    },
    {
      key: '3',
      label: '删除',
      disabled: settings.isFixed,
      danger: true,
      onClick: handleDelete,
    },
  ]

  let isShow =
    running &&
    running.isRun &&
    ((running.inVariables && running.inVariables.length > 0) ||
      (running.outVariables && running.outVariables))
  if (!isShow) {
    isShow = running && running.isRun && running.isRunning
  }

  return (
    <>
      <Card
        id={`node-${id}`}
        size='small'
        className='node-card'
        title={
          <div onClick={(e) => isEditing && e.stopPropagation()}>
            <NodeIcon type={type} isFixed={settings.isFixed} />
            {isEditing ? (
              <Input
                size='small'
                value={newTitle}
                onChange={(e) => setNewTitle(e.target.value)}
                onPressEnter={handleTitleSubmit}
                onBlur={handleTitleSubmit}
                autoFocus
              />
            ) : (
              <span>{settings.title}</span>
            )}
          </div>
        }
        extra={
          <div onClick={(e) => e.stopPropagation()}>
            {isNodeRunnable(type) && (
              <Tooltip title='试运行'>
                <PlayCircleOutlined
                  onClick={handleRun}
                  style={{ marginRight: 5 }}
                />
              </Tooltip>
            )}
            <Dropdown menu={{ items }} placement='bottom'>
              <MoreOutlined />
            </Dropdown>
          </div>
        }
      >
        {children}
      </Card>
      {isShow && (
        <Card
          style={{ marginTop: 5 }}
          className='node-run-card'
          size='small'
          loading={running?.isRunning}
          title={
            <Space>
              {running?.isRunning && (
                <>
                  <LoadingOutlined style={{ color: '#d9d9d9', fontSize: 12 }} />
                  <Text strong>运行中...</Text>
                </>
              )}
              {!running?.isRunning && running?.success && (
                <>
                  <CheckCircleOutlined
                    style={{ color: '#3EC254', fontSize: 12 }}
                  />
                  <Text strong>运行成功</Text>
                  <Tag style={{ padding: '0 8px' }}>{running?.seconds}s</Tag>
                </>
              )}
              {!running?.isRunning && !running?.success && (
                <>
                  <CloseCircleOutlined
                    style={{ color: '#ff4d4f', fontSize: 11 }}
                  />
                  <Text strong>{running?.errorMessage || '运行失败'}</Text>
                  <Tag style={{ padding: '0 8px' }}>{running?.seconds}s</Tag>
                </>
              )}
            </Space>
          }
          extra={
            !running?.isRunning && (
              <Button
                type='text'
                icon={
                  <DownOutlined
                    rotate={isCollapsed ? 270 : 0}
                    style={{ width: 12 }}
                  />
                }
                onClick={(e) => {
                  e.preventDefault()
                  e.stopPropagation()
                  setIsCollapsed(!isCollapsed)
                }}
              >
                {isCollapsed ? '展开结果' : '收起结果'}
              </Button>
            )
          }
        >
          {!running?.isRunning && !isCollapsed && (
            <Space
              direction='vertical'
              style={{ width: '100%' }}
              onClick={(e) => e.stopPropagation()}
            >
              <div>
                <Text strong>输入</Text>
              </div>
              <VariableList variables={running?.inVariables || []} />

              <Divider style={{ margin: 0 }}></Divider>

              <div>
                <Text strong>输出</Text>
              </div>
              {!running?.success && (
                <Alert
                  message='运行出错'
                  description={running?.errorMessage}
                  type='error'
                  showIcon
                />
              )}
              <VariableList variables={running?.outVariables || []} />
            </Space>
          )}
        </Card>
      )}
    </>
  )
}

export default NodeCard
