import { BoolResult } from '@/dto'
import api from '../..'

const url = '/ai/admin/dataset/tasks'
const urlIndex = `${url}/actions/index`
const urlRemove = `${url}/actions/remove`

interface IndexRequest extends Record<string, unknown> {
  siteId: number
  channelId: number
  contentId: number
}

interface RemoveRequest extends Record<string, unknown> {
  siteId: number
  channelId: number
  contentId: number
}

const datasetTasksApi = {
  index: async (request: IndexRequest) => {
    return await api.get<BoolResult>(urlIndex, request)
  },

  remove: async (request: RemoveRequest) => {
    return await api.post<BoolResult>(urlRemove, request)
  },
}

export default datasetTasksApi
