export interface ChatMessage {
  id: number
  uuid: string
  createdDate: Date
  lastModifiedDate: Date
  siteId: number
  sessionId: string
  role: 'user' | 'assistant'
  reasoning: string
  content: string
}
