import { BoolResult, SiteRequest } from '@/dto'
import { Site } from '@/models'
import api from '../..'

const url = '/ai/admin/dataset/settings'

interface GetResult {
  settings: Site
}

interface SubmitRequest extends SiteRequest {
  separators: string[]
  chunkSize: number
  chunkOverlap: number
  isChunkReplaces: boolean
  isChunkDeletes: boolean
  isRechunk: boolean
}

const datasetSettingsApi = {
  get: async (request: SiteRequest) => {
    return await api.get<GetResult>(url, request)
  },

  submit: async (request: SubmitRequest) => {
    return await api.post<BoolResult>(url, request)
  },
}

export default datasetSettingsApi
