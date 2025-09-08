import { BoolResult, Cascade } from '@/dto'
import { Dataset } from '@/models'
import api from '../../..'

const URL = '/ai/admin/apps/modals/datasetConfig'

interface GetRequest extends Record<string, unknown> {
  siteId: number
  nodeId: string
  datasetSiteId: number
}

interface GetResult {
  dataset: Dataset
  channels: Cascade<number>[]
  datasetChannelIds: Array<number[]>
}

interface SubmitRequest extends Record<string, unknown> {
  siteId: number
  nodeId: string
  datasetSiteId: number
  datasetAllChannels: boolean
  datasetChannelIds: Array<number[]>
}

const datasetConfigApi = {
  get: async (request: GetRequest) => {
    return await api.get<GetResult>(URL, request)
  },

  submit: async (request: SubmitRequest) => {
    return await api.post<BoolResult>(URL, request)
  },
}

export default datasetConfigApi
