import { SiteSummary } from '@/models'
import api from '../..'

const url = '/ai/admin/apps/flow/datasets'
interface GetRequest extends Record<string, unknown> {
  siteId: number
  nodeId: string
}

interface GetResult {
  datasetSites: SiteSummary[]
}

const datasetsApi = {
  get: async (request: GetRequest) => {
    return await api.get<GetResult>(url, request)
  },
}

export default datasetsApi
