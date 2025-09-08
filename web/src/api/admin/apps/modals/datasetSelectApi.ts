import { BoolResult } from '@/dto'
import { SiteSummary } from '@/models'
import api from '../../..'

const URL = '/ai/admin/apps/modals/datasetSelect'
const URL_DELETE = `${URL}/actions/delete`

interface DeleteRequest extends Record<string, unknown> {
  siteId: number
  nodeId: string
  datasetSiteId: number
}

interface GetRequest extends Record<string, unknown> {
  siteId: number
  nodeId: string
}

interface GetResult {
  sites: SiteSummary[]
  selectedSiteIds: number[]
}

interface SubmitRequest extends Record<string, unknown> {
  siteId: number
  nodeId: string
  datasetSiteId: number
}

const datasetSelectApi = {
  get: async (request: GetRequest) => {
    return await api.get<GetResult>(URL, request)
  },

  submit: async (request: SubmitRequest) => {
    return await api.post<BoolResult>(URL, request)
  },

  delete: async (request: DeleteRequest) => {
    return await api.post<BoolResult>(URL_DELETE, request)
  },
}

export default datasetSelectApi
