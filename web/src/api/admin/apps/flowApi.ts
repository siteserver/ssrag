import { RunVariable, FlowNodeSettings, Model } from '@/dto'
import { FlowEdge, SiteSummary } from '@/models'
import api from '../..'

const url = '/ai/admin/apps/flow'
const urlOptimize = `${url}/actions/optimize`
const urlRun = `${url}/actions/run`
const urlRunNode = `${url}/actions/runNode`
const urlRunOutput = `${url}/actions/runOutput`
const urlDataset = `${url}/actions/dataset`

interface GetRequest extends Record<string, unknown> {
  siteId: number
}

interface GetResult {
  siteName: string
  flowNodesSettings: Record<string, FlowNodeSettings>
  flowNodesInVariables: Record<string, RunVariable[]>
  flowNodesOutVariables: Record<string, RunVariable[]>
  nodes: FlowNodeSettings[]
  edges: FlowEdge[]
  models: Model[]
  defaultModel: Model | null
}

interface SubmitRequest extends Record<string, unknown> {
  siteId: number
  nodes: FlowNodeSettings[]
  edges: FlowEdge[]
  variables: RunVariable[]
}

interface SubmitResult {
  nodes: FlowNodeSettings[]
  edges: FlowEdge[]
}

interface OptimizeRequest extends Record<string, unknown> {
  siteId: number
  items: string[]
}

interface OptimizeResult {
  items: string[]
}

interface RunRequest extends Record<string, unknown> {
  siteId: number
  nodeId: string
  inVariables: RunVariable[]
}

interface RunResult {
  success: boolean
  errorMessage: string
  outVariables: RunVariable[]
}

interface RunNodeRequest extends Record<string, unknown> {
  siteId: number
  nodeId: string
  inVariablesDict: Record<string, RunVariable[]>
  outVariablesDict: Record<string, RunVariable[]>
}

interface RunNodeResult {
  isOutput: boolean
  nextNodeId: string
  outVariables: RunVariable[]
}

interface DatasetRequest extends Record<string, unknown> {
  siteId: number
  nodeId: string
}

interface DatasetResult {
  datasetSites: SiteSummary[]
}

const flowApi = {
  get: async (request: GetRequest) => {
    return await api.get<GetResult>(url, request)
  },

  submit: async (request: SubmitRequest) => {
    return await api.post<SubmitResult>(url, request)
  },

  optimize: async (request: OptimizeRequest) => {
    return await api.post<OptimizeResult>(urlOptimize, request)
  },

  run: async (request: RunRequest) => {
    return await api.post<RunResult>(urlRun, request)
  },

  runNode: async (request: RunNodeRequest) => {
    return await api.post<RunNodeResult>(urlRunNode, request)
  },

  getRunOutputUrl: () => {
    return '/api' + urlRunOutput
  },

  dataset: async (request: DatasetRequest) => {
    return await api.post<DatasetResult>(urlDataset, request)
  },
}

export default flowApi
