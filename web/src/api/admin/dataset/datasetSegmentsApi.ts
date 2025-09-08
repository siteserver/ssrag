import { BoolResult } from '@/dto'
import { ChannelSummary, Segment } from '@/models'
import api from '../..'

const url = '/ai/admin/dataset/segments'
const urlUpdate = `${url}/actions/update`
const urlInsert = `${url}/actions/insert`

interface GetRequest extends Record<string, unknown> {
  siteId: number
  channelId: number
  documentId: number
  page: number
  pageSize: number
}

interface GetResult {
  segments: Segment[]
  count: number
  breadcrumb: ChannelSummary[]
}

interface UpdateRequest extends Record<string, unknown> {
  documentId: number
  segmentId: string
  content: string
}

interface InsertRequest extends Record<string, unknown> {
  documentId: number
  taxis: number
}

interface InsertResult {
  segment: Segment | null
}

const datasetSegmentsApi = {
  get: async (request: GetRequest) => {
    return await api.get<GetResult>(url, request)
  },

  update: async (request: UpdateRequest) => {
    return await api.post<BoolResult>(urlUpdate, request)
  },

  insert: async (request: InsertRequest) => {
    return await api.post<InsertResult>(urlInsert, request)
  },
}

export default datasetSegmentsApi
