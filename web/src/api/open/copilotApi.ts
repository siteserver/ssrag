import { ChatGroup, Site, SiteSummary, SiteValues, ChatMessage } from '@/models'
import api from '..'

const url = '/ai/open/copilot'

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

const aiApi = {
  get: async (request: GetRequest) => {
    return await api.get<GetResult>(url, request)
  },
}

export default aiApi
