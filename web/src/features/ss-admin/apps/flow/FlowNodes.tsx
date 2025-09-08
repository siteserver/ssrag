import { useCallback, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import {
  RunVariable,
  NodeRunning,
  ReactFlowNode,
  ReactFlowEdge,
  FlowNodeSettings,
} from '@/dto'
import { VariableType, NodeType } from '@/enums'
import { FlowEdge } from '@/models'
import { PlusOutlined, CloseOutlined, LoadingOutlined } from '@ant-design/icons'
import {
  ReactFlow,
  Background,
  MiniMap,
  Panel,
  addEdge,
  useNodesState,
  useEdgesState,
  Connection,
  NodeTypes,
} from '@xyflow/react'
import { Spin, Button, Drawer, Tooltip, App, Skeleton } from 'antd'
import flowApi from '@/api/admin/apps/flowApi'
import AddNodes from './components/AddNodes'
import CustomEdge from './components/CustomEdge'
import RunAll from './components/RunAll'
import Toolbar from './components/Toolbar'
import { updateLayout, getNewNode, getNodesByRes, getEdgesByRes } from './core'
import AskNode from './nodes/AskNode'
import DatasetNode from './nodes/DatasetNode'
import HttpNode from './nodes/HttpNode'
import InputNode from './nodes/InputNode'
import IntentNode from './nodes/IntentNode'
import LLMNode from './nodes/LLMNode'
import OutputNode from './nodes/OutputNode'
import SqlNode from './nodes/SqlNode'
import StartNode from './nodes/StartNode'
import TextNode from './nodes/TextNode'
import WebSearchNode from './nodes/WebSearchNode'
import Settings from './settings'
import { useStore } from './store'

const nodeTypes: NodeTypes = {
  [NodeType.Start]: StartNode,
  [NodeType.Input]: InputNode,
  [NodeType.LLM]: LLMNode,
  [NodeType.Output]: OutputNode,
  [NodeType.Http]: HttpNode,
  [NodeType.WebSearch]: WebSearchNode,
  [NodeType.Sql]: SqlNode,
  [NodeType.Text]: TextNode,
  [NodeType.Intent]: IntentNode,
  [NodeType.Dataset]: DatasetNode,
  [NodeType.Ask]: AskNode,
}

const edgeTypes = {
  'custom-edge': CustomEdge,
}

const FlowNodes: React.FC = () => {
  const { message } = App.useApp()
  const store = useStore()
  const [loading, setLoading] = useState(false)
  const [nodes, setNodes, onNodesChange] = useNodesState<ReactFlowNode>([])
  const [edges, setEdges, onEdgesChange] = useEdgesState<ReactFlowEdge>([])
  const [selectedNode, setSelectedNode] = useState<ReactFlowNode | null>(null)
  const [fullscreen, setFullscreen] = useState(false)
  const [openNodeAdd, setOpenNodeAdd] = useState(false)
  const [runAll, setRunAll] = useState<boolean>(false)

  const { isPending } = useQuery({
    queryKey: ['flow', store.siteId],
    queryFn: async () => {
      const res = await flowApi.get({ siteId: store.siteId })
      if (res) {
        store.init(
          res.siteName,
          res.flowNodesSettings,
          res.flowNodesInVariables,
          res.flowNodesOutVariables,
          res.models,
          res.defaultModel,
          handleRun,
          handleRunning,
          handleRunned,
          handleRunAll,
          handleSave
        )
        const resNodes = getNodesByRes(res)
        setNodes(resNodes)
        const resEdges = getEdgesByRes(res)
        setEdges(resEdges)
      }
      return res
    },
  })

  const apiSubmit = async (silence: boolean) => {
    if (!silence) {
      setLoading(true)
    }

    const submitNodes: FlowNodeSettings[] = nodes.map((node) => {
      const settings = store.getNodeSettings(node.id)
      return {
        ...settings,
        id: node.id,
        parentId: node.parentId ? parseInt(node.parentId) : undefined,
        position: {
          x: node.position.x,
          y: node.position.y,
        },
      }
    })

    const submitEdges: FlowEdge[] = edges.map((edge) => {
      return {
        ...edge,
      }
    })

    const variables: RunVariable[] = []
    nodes.forEach((node) => {
      const inVariables = store.getNodeInVariables(node.id)
      const outVariables = store.getNodeOutVariables(node.id)
      if (inVariables || outVariables) {
        inVariables.forEach((variable: RunVariable) => {
          variables.push({
            id: variable.id || 0,
            nodeId: node.id,
            type: VariableType.Input,
            name: variable.name,
            dataType: variable.dataType,
            isDisabled: variable.isDisabled,
            isReference: variable.isReference,
            referenceNodeId: variable.referenceNodeId,
            referenceName: variable.referenceName,
            value: variable.value,
          })
        })
        outVariables.forEach((variable: RunVariable) => {
          variables.push({
            id: variable.id || 0,
            nodeId: node.id,
            type: VariableType.Output,
            name: variable.name,
            dataType: variable.dataType,
            isDisabled: variable.isDisabled,
            isReference: variable.isReference,
            referenceNodeId: variable.referenceNodeId,
            referenceName: variable.referenceName,
            value: variable.value,
          })
        })
      }
    })

    const res = await flowApi.submit({
      siteId: store.siteId,
      nodes: submitNodes,
      edges: submitEdges,
      variables,
    })
    if (res) {
      const resNodes = getNodesByRes(res)
      setNodes(resNodes)
      const resEdges = getEdgesByRes(res)
      setEdges(resEdges)
      if (!silence) {
        message.open({
          type: 'success',
          content: '流程设计保存成功！',
        })
      }
    }

    setLoading(false)
  }

  const handleRunAll = (
    previousNodeId?: string,
    nodeId?: string,
    success?: boolean,
    inVariables?: RunVariable[],
    outVariables?: RunVariable[],
    seconds?: string,
    errorMessage?: string
  ) => {
    setNodes((nds) =>
      nds.map((node) => {
        if (node.id === `${nodeId}`) {
          const running = store.getNodeRunning(node.id)
          if (running) {
            store.setNodeRunning(node.id, {
              ...running,
              isRun: true,
              isRunning: false,
              inVariables: inVariables,
              outVariables: outVariables,
              success: success,
            })
          }
          if (success) {
            if (inVariables && outVariables) {
              store.setNodeRunning(node.id, {
                ...running,
                isRun: true,
                isRunning: false,
                inVariables,
                outVariables,
                success: true,
                seconds,
              })
            } else {
              store.setNodeRunning(node.id, {
                ...running,
                isRun: true,
                isRunning: true,
              })
            }
          } else {
            store.setNodeRunning(node.id, {
              ...running,
              inVariables,
              success: false,
              errorMessage: errorMessage,
              outVariables: [],
              seconds: seconds,
              isRun: true,
              isRunning: false,
            })
          }
        }
        return {
          ...node,
          selected: false,
        }
      })
    )
    setEdges((egs) =>
      egs.map((edge) => {
        const animated =
          inVariables && outVariables
            ? false
            : edge.source === `${previousNodeId}` && edge.target === `${nodeId}`
        return { ...edge, animated }
      })
    )
  }

  const handleSave = async (silence: boolean) => {
    await apiSubmit(silence)
  }

  const handlePublish = () => {
    // window.parent.utils.addTab(
    //   '发布流程',
    //   addQuery('/ss-admin/apps/flowsPublish', {
    //     siteId,
    //     flowId,
    //   })
    // )
  }

  const handleRun = (node: ReactFlowNode) => {
    setOpenNodeAdd(false)
    setSelectedNode(null)
    if (node) {
      const running = store.getNodeRunning(node.id)
      if (running) {
        store.setNodeRunning(node.id, {
          ...running,
          isRun: true,
        })
      } else {
        store.setNodeRunning(node.id, {
          isRun: true,
          isRunning: true,
        })
      }
      setSelectedNode(node)
      // setTimeout(() => {
      //   setSelectedNode(node)
      //   setNodes((nds) =>
      //     nds.map((nd) => {
      //       return { ...nd }
      //     })
      //   )
      // }, 100)
    }
  }

  const handleRunning = (id: string) => {
    setNodes((nds) =>
      nds.map((node) => {
        if (node.id === id) {
          const running = store.getNodeRunning(node.id)
          if (running) {
            store.setNodeRunning(node.id, {
              ...running,
              isRun: true,
              isRunning: true,
            })
          }
        }
        return { ...node }
      })
    )
  }

  const handleRunned = (id: string, running: NodeRunning) => {
    setNodes((nds) =>
      nds.map((node) => {
        if (node.id === id) {
          store.setNodeRunning(node.id, {
            ...running,
            isRun: true,
            isRunning: false,
          })
        }
        return { ...node }
      })
    )
  }

  const handlePaneClick = useCallback(() => {
    setOpenNodeAdd(false)
    setSelectedNode(null)
    for (const node of nodes) {
      const running = store.getNodeRunning(node.id)
      if (running) {
        store.setNodeRunning(node.id, { ...running, isRun: false })
      }
    }
  }, [nodes, store])

  const handleNodeClick = (_: unknown, node: ReactFlowNode) => {
    setOpenNodeAdd(false)
    setSelectedNode(node)
    const running = store.getNodeRunning(node.id)
    if (running) {
      store.setNodeRunning(node.id, { ...running, isRun: false })
    }
    // setTimeout(() => {
    //   setSelectedNode(node)
    //   setNodes((nds) =>
    //     nds.map((nd) => {
    //       return { ...nd }
    //     })
    //   )
    // }, 100)
  }

  const handleNodesDelete = () => {
    setSelectedNode(null)
  }

  const handleDrop = (event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault()
    const type = event.dataTransfer.getData('application/reactflow') as NodeType
    const position = { x: event.clientX - 150, y: event.clientY - 50 }
    const newNode = getNewNode(type, position)
    setNodes((nds) => nds.concat(newNode.node))
    store.setNodeSettings(newNode.node.id, newNode.settings)
    store.setNodeInVariables(newNode.node.id, newNode.inVariables)
    store.setNodeOutVariables(newNode.node.id, newNode.outVariables)
    setOpenNodeAdd(false)
  }

  const handleDragOver = (event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault()
    event.dataTransfer.dropEffect = 'move'
  }

  const handleConnect = useCallback(
    (connection: Connection) => {
      const { source, sourceHandle, target, targetHandle } = connection
      const isSourceConnected = edges.some(
        (edge) => edge.source === source && edge.sourceHandle === sourceHandle
      )
      const isTargetConnected = edges.some(
        (edge) => edge.target === target && edge.targetHandle === targetHandle
      )
      if (isSourceConnected && isTargetConnected) return

      const isHorizontal =
        sourceHandle &&
        sourceHandle.startsWith('right') &&
        targetHandle &&
        targetHandle.startsWith('left')
      const isVertical =
        sourceHandle &&
        sourceHandle.startsWith('bottom') &&
        targetHandle &&
        targetHandle.startsWith('top')
      if (!isHorizontal && !isVertical) {
        return
      }

      const sourceNode = nodes.find((node) => node.id === source)
      const targetNode = nodes.find((node) => node.id === target)
      if (!sourceNode || !targetNode) {
        message.error('节点不存在')
        return
      }
      if (sourceNode.parentId != targetNode.parentId) {
        message.error('节点不能跨组连接')
        return
      }

      if (
        sourceNode.type === NodeType.Intent ||
        sourceNode.type === NodeType.Ask
      ) {
        if (isSourceConnected) {
          return
        }
      } else {
        if (isSourceConnected || isTargetConnected) {
          return
        }
      }

      setEdges((eds) =>
        addEdge(
          {
            id: `${source}-${target}`,
            type: 'custom-edge',
            source,
            sourceHandle,
            target,
            targetHandle,
          } as ReactFlowEdge,
          eds
        )
      )

      const sourceOutVariables = store.getNodeOutVariables(source)
      sourceOutVariables.forEach((variable) => {
        if (variable.name === 'output') {
          const targetInVariables = store.getNodeInVariables(target)
          targetInVariables.forEach((targetVariable) => {
            if (targetVariable.name === 'input') {
              const updatedTargetVariable = {
                ...targetVariable,
                isReference: true,
                referenceNodeId: source,
                referenceName: variable.name,
                dataType: variable.dataType,
              }
              const updatedVariables = targetInVariables.map((v) =>
                v === targetVariable ? updatedTargetVariable : v
              )
              store.setNodeInVariables(target, updatedVariables)
            }
          })
        }
      })
    },
    [nodes, edges]
  )

  const handleNodeDragStart = useCallback(
    () =>
      setNodes((nds) =>
        nds.map((node) => ({
          ...node,
          selected: false,
        }))
      ),
    [setNodes]
  )

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <>
      <Spin
        spinning={loading}
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
      <ReactFlow
        nodeTypes={nodeTypes}
        edgeTypes={edgeTypes}
        nodes={nodes}
        edges={edges}
        onPaneClick={handlePaneClick}
        onNodeClick={handleNodeClick}
        onNodesChange={onNodesChange}
        onNodesDelete={handleNodesDelete}
        onEdgesChange={onEdgesChange}
        onConnect={handleConnect}
        onNodeDragStart={handleNodeDragStart}
        onDrop={handleDrop}
        onDragOver={handleDragOver}
        minZoom={0.25}
        // fitView
        style={{ backgroundColor: '#f0f0f0' }}
      >
        <Background color='#666' />
        <MiniMap nodeStrokeWidth={3} zoomable pannable />
        <Panel position='top-left'>
          <Tooltip title='新增节点'>
            <Button
              type={openNodeAdd ? 'default' : 'primary'}
              size='large'
              shape='circle'
              icon={openNodeAdd ? <CloseOutlined /> : <PlusOutlined />}
              onClick={() => setOpenNodeAdd(!openNodeAdd)}
            ></Button>
          </Tooltip>
        </Panel>
        <Panel position='bottom-center'>
          <Toolbar
            fullscreen={fullscreen}
            onFullscreen={() => {
              setFullscreen(!fullscreen)
            }}
            onFix={() => {
              updateLayout(nodes, edges, setNodes)
            }}
            onSave={() => {
              handleSave(false)
            }}
            onRun={() => {
              handleSave(true)
              setRunAll(true)
            }}
            onPublish={() => {
              handlePublish()
            }}
          ></Toolbar>
        </Panel>
      </ReactFlow>
      <Drawer
        title={null}
        placement='left'
        closable={false}
        open={openNodeAdd}
        width={420}
        mask={false}
        destroyOnHidden={true}
      >
        <AddNodes
          onNodeClick={(nodeType) => {
            const newNode = getNewNode(nodeType)
            setNodes((nds) => nds.concat(newNode.node))
            store.setNodeSettings(newNode.node.id, newNode.settings)
            store.setNodeInVariables(newNode.node.id, newNode.inVariables)
            store.setNodeOutVariables(newNode.node.id, newNode.outVariables)
            setOpenNodeAdd(false)
          }}
        />
      </Drawer>
      {selectedNode && (
        <Settings
          selectedNode={selectedNode}
          // onSubmit={handleSettingsSubmit}
          onClose={handlePaneClick}
          onSave={handleSave}
        ></Settings>
      )}
      {runAll && (
        <Drawer
          title='试运行'
          placement='right'
          closable={true}
          open={true}
          width={720}
          mask={false}
          destroyOnHidden={true}
          onClose={() => setRunAll(false)}
        >
          <RunAll nodes={nodes}></RunAll>
        </Drawer>
      )}
    </>
  )
}

export default FlowNodes
