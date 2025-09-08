import { Cascade, Result } from '@/dto'
import { SearchType } from '@/enums'
import api from '../..'

const url = '/ai/admin/dataset/testing'

interface GetRequest extends Record<string, unknown> {
  siteId: number
}

interface GetResult {
  siteUrl: string
  channels: Cascade<number>[]
}

interface SubmitRequest extends Record<string, unknown> {
  siteId: number
  channelId: number
  contentId: number
  searchType: SearchType
  maxCount: number
  minScore: number
  query: string
}

interface SubmitResult {
  cuts: string[]
  results: Result[]
  channelIds: number[]
}

const datasetTestingApi = {
  get: async (request: GetRequest) => {
    return await api.get<GetResult>(url, request)
  },

  submit: async (request: SubmitRequest) => {
    return await api.post<SubmitResult>(url, request)
  },
}

export default datasetTestingApi
