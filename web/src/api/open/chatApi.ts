import { ChatGroup, ChatMessage, SiteValues, Site, Prompt } from '@/models'
import api from '..'

const url = '/ai/open/chat'
const urlHistory = `${url}/actions/history`

interface GetRequest extends Record<string, unknown> {
  id: string | null
  siteId: number | null
  sessionId: string | null
}

interface GetResult {
  site: Site
  values: SiteValues
  sessionId: string
  messages: ChatMessage[]
  hotPrompts: Prompt[]
  functionPrompts: Prompt[]
  inputPrompts: Prompt[]
}

interface HistoryRequest extends Record<string, unknown> {
  siteId: number
  searching: boolean
  thinking: boolean
  sessionId: string
  message: string
  reasoning: string
  content: string
}

interface HistoryResult {
  chatGroup: ChatGroup
}

const chatApi = {
  getUrl: () => {
    return `/api${url}`
  },

  get: async (request: GetRequest) => {
    return await api.get<GetResult>(url, request)
  },

  history: async (request: HistoryRequest) => {
    return await api.post<HistoryResult>(urlHistory, request)
  },
}

export default chatApi
