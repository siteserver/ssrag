import { DisplayType, SearchType } from '@/enums'

export interface Site {
  id: number
  uuid: string
  createdDate: Date
  lastModifiedDate: Date
  siteDir: string
  siteName: string
  siteType: string
  iconUrl: string
  imageUrl: string
  keywords: string
  description: string
  tableName: string
  root: boolean
  parentId: number
  taxis: number

  separators: string[]
  chunkSize: number
  chunkOverlap: number
  isChunkReplaces: boolean
  isChunkDeletes: boolean
}

export interface SiteValues {
  providerModelId: string
  llmSystemPrompt: string
  datasetSearchType: SearchType
  datasetMaxCount: number
  datasetMinScore: number

  displayType: DisplayType
  headerText: string
  footerText: string

  welcomeTitle: string
  welcomeVariant: 'borderless' | 'filled'
  welcomePosition: 'top' | 'center'

  isHotPrompts: boolean
  hotPromptsTitle: string
  isFunctionPrompts: boolean
  functionPromptsTitle: string
  isInputPrompts: boolean
  inputPromptsLimit: number

  senderPlaceholder: string
  senderAllowSpeech: boolean

  isChatDefaultOpen: boolean
  isChatIconDraggable: boolean
  chatCloseIconUrl: string
  chatDescription: string
  chatOpenIconUrl: string
}
