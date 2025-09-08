import { BoolResult } from '@/dto'
import { DisplayType, SearchType } from '@/enums'
import { SiteValues, Site, Model, SiteSummary, Prompt } from '@/models'
import api from '../..'

const url = '/ai/admin/apps/settings'
const urlUpload = `${url}/actions/upload`
const urlStyles = `${url}/actions/styles`
const urlAi = `${url}/actions/ai`

interface GetRequest extends Record<string, unknown> {
  siteId: number
}

interface GetResult {
  site: Site
  values: SiteValues
  models: Model[]
  defaultModel: Model | null
  datasetSites: SiteSummary[]
  hotPrompts: Prompt[]
  functionPrompts: Prompt[]
  inputPrompts: Prompt[]
}

interface StylesRequest extends Record<string, unknown> {
  siteId: number

  displayType: DisplayType
  headerText: string
  footerText: string

  welcomeTitle: string
  welcomeVariant: 'borderless' | 'filled'
  welcomePosition: 'top' | 'center'
  iconUrl: string
  description: string

  isHotPrompts: boolean
  hotPromptsTitle: string
  hotPrompts: Prompt[]
  isFunctionPrompts: boolean
  functionPromptsTitle: string
  functionPrompts: Prompt[]
  isInputPrompts: boolean
  inputPrompts: Prompt[]

  senderPlaceholder: string
  senderAllowSpeech: boolean
}

interface AiRequest extends Record<string, unknown> {
  siteId: number
  providerModelId: string
  llmSystemPrompt: string
  datasetSearchType: SearchType
  datasetMaxCount: number
  datasetMinScore: number
}

const settingsApi = {
  getUploadUrl: (siteId: number) => {
    return api.getUploadUrl(urlUpload, { siteId })
  },

  getUploadHeaders: () => {
    return {
      Authorization: `Bearer ${api.token}`,
    }
  },

  get: async (request: GetRequest) => {
    return await api.get<GetResult>(url, request)
  },

  styles: async (request: StylesRequest) => {
    return await api.post<BoolResult>(urlStyles, request)
  },

  ai: async (request: AiRequest) => {
    return await api.post<BoolResult>(urlAi, request)
  },
}

export default settingsApi
