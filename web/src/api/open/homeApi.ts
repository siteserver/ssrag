import { BoolResult } from '@/dto'
import { ChatGroup, ChatMessage, Site, SiteSummary, SiteValues } from '@/models'
import api from '..'

const url = '/ai/open/home'
const urlDelete = `${url}/actions/delete`
const urlRename = `${url}/actions/rename`

interface GetRequest extends Record<string, unknown> {
  id: string | null
  siteId: number | null
  sessionId: string | null
}

interface GetResult {
  site: Site
  values: SiteValues
  sites: SiteSummary[]
  chatGroups: ChatGroup[]
  sessionId: string
  messages: ChatMessage[]
}

interface DeleteRequest extends Record<string, unknown> {
  siteId: string
  sessionId: string
}

interface RenameRequest extends Record<string, unknown> {
  siteId: string
  sessionId: string
  title: string
}

const aiApi = {
  get: async (request: GetRequest) => {
    return await api.get<GetResult>(url, request)
  },

  delete: async (request: DeleteRequest) => {
    return await api.post<BoolResult>(urlDelete, request)
  },

  rename: async (request: RenameRequest) => {
    return await api.post<BoolResult>(urlRename, request)
  },
}

export default aiApi
