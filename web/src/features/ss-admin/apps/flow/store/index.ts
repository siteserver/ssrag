import {
  RunVariable,
  ReactFlowNode,
  NodeRunning,
  FlowNodeSettings,
  Model,
} from '@/dto'
import { FlowVariable } from '@/models'
import { getQueryInt } from '@/utils'
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

interface State {
  siteId: number
  siteName: string
  flowNodesSettings: Record<string, FlowNodeSettings>
  nodesInVariables: Record<string, FlowVariable[]>
  nodesOutVariables: Record<string, FlowVariable[]>
  nodesRunning: Record<string, NodeRunning>
  models: Model[]
  defaultModel: Model | null
  loading: boolean
  page: string
  initialized: boolean
  init: (
    siteName: string,
    flowNodesSettings: Record<string, FlowNodeSettings>,
    nodesInVariables: Record<string, FlowVariable[]>,
    nodesOutVariables: Record<string, FlowVariable[]>,
    models: Model[],
    defaultModel: Model | null,
    onRun: (node: ReactFlowNode) => void,
    onRunning: (id: string) => void,
    onRunned: (id: string, running: NodeRunning) => void,
    onRunAll: (
      previousNodeId: string,
      nodeId: string,
      success: boolean,
      inVariables?: RunVariable[],
      outVariables?: RunVariable[],
      seconds?: string,
      errorMessage?: string
    ) => void,
    onSave: (silence: boolean) => void
  ) => void
  setLoading: (loading: boolean) => void
  setPage: (page: string) => void
  getNodeSettings: (nodeId: string) => FlowNodeSettings
  setNodeSettings: (nodeId: string, settings: FlowNodeSettings) => void
  getNodeInVariables: (nodeId: string) => FlowVariable[]
  setNodeInVariables: (nodeId: string, settings: FlowVariable[]) => void
  getNodeOutVariables: (nodeId: string) => FlowVariable[]
  setNodeOutVariables: (nodeId: string, settings: FlowVariable[]) => void
  getNodeRunning: (nodeId: string) => NodeRunning
  setNodeRunning: (nodeId: string, settings: NodeRunning) => void
  onRun: (node: ReactFlowNode) => void
  onRunning: (id: string) => void
  onRunned: (id: string, running: NodeRunning) => void
  onRunAll: (
    previousNodeId: string,
    nodeId: string,
    success: boolean,
    inVariables?: RunVariable[],
    outVariables?: RunVariable[],
    seconds?: string,
    errorMessage?: string
  ) => void
  onSave: (silence: boolean) => void
}

export const useStore = create(
  immer<State>((set, get) => ({
    siteId: getQueryInt('siteId') || 0,
    siteName: '',
    flowNodesSettings: {},
    nodesInVariables: {},
    nodesOutVariables: {},
    nodesRunning: {},
    models: [],
    defaultModel: null,
    defaultSettings: {},
    onRun: () => {},
    onRunning: () => {},
    onRunned: () => {},
    onRunAll: () => {},
    onSave: () => {},
    loading: false,
    page: 'documents',
    initialized: false,

    init: (
      siteName: string,
      flowNodesSettings: Record<string, FlowNodeSettings>,
      nodesInVariables: Record<string, FlowVariable[]>,
      nodesOutVariables: Record<string, FlowVariable[]>,
      models: Model[],
      defaultModel: Model | null,
      onRun: (node: ReactFlowNode) => void,
      onRunning: (id: string) => void,
      onRunned: (id: string, running: NodeRunning) => void,
      onRunAll: (
        previousNodeId: string,
        nodeId: string,
        success: boolean,
        inVariables?: FlowVariable[],
        outVariables?: FlowVariable[],
        seconds?: string,
        errorMessage?: string
      ) => void,
      onSave: (silence: boolean) => void
    ) => {
      set((state) => {
        state.siteName = siteName
        state.flowNodesSettings = flowNodesSettings
        state.nodesInVariables = nodesInVariables
        state.nodesOutVariables = nodesOutVariables
        state.nodesRunning = {} as Record<string, NodeRunning>
        state.models = models
        state.defaultModel = defaultModel
        state.onRun = onRun
        state.onRunning = onRunning
        state.onRunned = onRunned
        state.onRunAll = onRunAll
        state.onSave = onSave
        state.initialized = true
      })
    },

    setLoading: (loading: boolean) => {
      set((state) => {
        state.loading = loading
      })
    },

    setPage: (page: string) => {
      set((state) => {
        state.page = page
      })
    },

    getNodeSettings: (nodeId: string): FlowNodeSettings => {
      return get().flowNodesSettings[nodeId] || ({} as FlowNodeSettings)
    },

    setNodeSettings: (nodeId: string, settings: FlowNodeSettings) => {
      set((state) => {
        state.flowNodesSettings[nodeId] = settings
      })
    },

    getNodeInVariables: (nodeId: string): FlowVariable[] => {
      return get().nodesInVariables[nodeId]
    },

    setNodeInVariables: (nodeId: string, variables: FlowVariable[]) => {
      set((state) => {
        state.nodesInVariables[nodeId] = variables
      })
    },

    getNodeOutVariables: (nodeId: string): FlowVariable[] => {
      return get().nodesOutVariables[nodeId] || []
    },

    setNodeOutVariables: (nodeId: string, variables: FlowVariable[]) => {
      set((state) => {
        state.nodesOutVariables[nodeId] = variables
      })
    },

    getNodeRunning: (nodeId: string): NodeRunning => {
      return get().nodesRunning[nodeId] || ({} as NodeRunning)
    },

    setNodeRunning: (nodeId: string, running: NodeRunning) => {
      set((state) => {
        state.nodesRunning[nodeId] = running
      })
    },
  }))
)
