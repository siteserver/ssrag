import { SiteRequest } from '@/dto'
import { CeleryTask } from '@/models'
import api from '../..'

const url = '/ai/admin/dataset/status'

interface GetResult {
  tasks: CeleryTask[]
  pendingCount: number
  documentCount: number
}

interface SubmitRequest extends SiteRequest {
  taskIds: string[]
}

const datasetStatusApi = {
  get: async (request: SiteRequest) => {
    return await api.get<GetResult>(url, request)
  },

  submit: async (request: SubmitRequest) => {
    return await api.post<GetResult>(url, request)
  },
}

export default datasetStatusApi
