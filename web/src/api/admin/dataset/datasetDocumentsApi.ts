import { Cascade, TaskDocumentProcess } from '@/dto'
import { Document, ChunkConfig } from '@/models'
import api from '../..'

const url = '/ai/admin/dataset/documents'
const urlList = `${url}/actions/list`
const urlRemove = `${url}/actions/remove`
const urlUpload = `${url}/actions/upload`
const urlGetMarkdown = `${url}/actions/get_markdown`
const urlDownload = `${url}/actions/download`
const urlProcess = `${url}/actions/process`
const urlStatus = `${url}/actions/status`
const urlChunkAndEmbed = `${url}/actions/chunk_and_embed`
const urlUploadRemove = `${url}/actions/upload_remove`

interface GetRequest extends Record<string, unknown> {
  siteId: number
  channelId: number
  contentId: number
}

interface GetResult {
  isModelReady: boolean
  siteUrl: string
  channels: Cascade<number>[]
}

interface ListRequest extends Record<string, unknown> {
  siteId: number
  channelId: number
  contentId: number
  search: string
  page: number
}

interface ListResult {
  documents: Document[]
  total: number
  channelIds: number[]
}

interface RemoveRequest extends Record<string, unknown> {
  documentId: number
}

interface GetMarkdownRequest extends Record<string, unknown> {
  documentId: number
}

interface GetMarkdownResult {
  value: string
}

interface DownloadRequest extends Record<string, unknown> {
  documentId: number
}

interface DownloadResult {
  value: string
}

interface ProcessRequest extends Record<string, unknown> {
  tasks: TaskDocumentProcess[]
}

interface ProcessResult {
  taskIds: string[]
}

interface StatusRequest extends Record<string, unknown> {
  taskIds: string[]
}

interface StatusResult {
  results: {
    taskId: string
    state: string
    detail: string
    result: {
      id: number
      fileName: string
      extName: string
      fileSize: number
      title: string
    } | null
  }[]
}

interface ChunkAndEmbedRequest extends Record<string, unknown> {
  siteId: number
  channelId: number
  contentId: number
  documentId: number
  config: ChunkConfig
}

interface ChunkAndEmbedResult {
  document: Document
}

interface UploadRemoveRequest extends Record<string, unknown> {
  task: TaskDocumentProcess
}

const datasetDocumentsApi = {
  getUploadUrl: (request: GetRequest) => {
    return api.getUploadUrl(urlUpload, request)
  },

  getUploadHeaders: () => {
    return {
      Authorization: `Bearer ${api.token}`,
    }
  },

  get: async (request: GetRequest) => {
    return await api.get<GetResult>(url, request)
  },

  list: async (request: ListRequest) => {
    return await api.post<ListResult>(urlList, request)
  },

  remove: async (request: RemoveRequest) => {
    return await api.post(urlRemove, request)
  },

  getMarkdown: async (request: GetMarkdownRequest) => {
    return await api.post<GetMarkdownResult>(urlGetMarkdown, request)
  },

  download: async (request: DownloadRequest) => {
    return await api.post<DownloadResult>(urlDownload, request)
  },

  process: async (request: ProcessRequest) => {
    return await api.post<ProcessResult>(urlProcess, request)
  },

  status: async (request: StatusRequest) => {
    return await api.post<StatusResult>(urlStatus, request)
  },

  chunkAndEmbed: async (request: ChunkAndEmbedRequest) => {
    return await api.post<ChunkAndEmbedResult>(urlChunkAndEmbed, request)
  },

  uploadRemove: async (request: UploadRemoveRequest) => {
    return await api.post(urlUploadRemove, request)
  },
}

export default datasetDocumentsApi
