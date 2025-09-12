import { ChatGroup, ChatMessage } from '@/models'
import api from '../..'

const url = '/ai/admin/apps/messages'
const urlMessages = `${url}/actions/messages`

interface GetRequest extends Record<string, unknown> {
  siteId: number
  page: number
  pageSize: number
  dateStart: string | null
  dateEnd: string | null
  title: string
}

interface GetResult {
  chatGroups: ChatGroup[]
}

interface MessagesRequest extends Record<string, unknown> {
  siteId: number
  sessionId: string
}

interface MessagesResult {
  messages: ChatMessage[]
}

const messagesApi = {
  get: async (request: GetRequest) => {
    return await api.get<GetResult>(url, request)
  },

  messages: async (request: MessagesRequest) => {
    return await api.post<MessagesResult>(urlMessages, request)
  },
}

export default messagesApi
