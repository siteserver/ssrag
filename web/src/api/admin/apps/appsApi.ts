import { BoolResult, IntResult, SiteTemplate } from '@/dto'
import { SiteSummary } from '@/models'
import api from '../..'

const URL_AI = '/ai/admin/apps'
const urlDisable = `${URL_AI}/actions/disable`
const urlDeleteAI = `${URL_AI}/actions/deleteAI`
const urlOrder = `${URL_AI}/actions/order`
const urlUpload = `${URL_AI}/actions/upload`
const urlInit = `${URL_AI}/actions/init`

const URL_CORE = '/admin/apps'
const urlGetTemplates = `${URL_CORE}/actions/getTemplates`
const urlUploadTemplate = `${URL_CORE}/actions/uploadTemplate`
const urlDeleteCore = `${URL_CORE}/actions/deleteCore`

interface GetResult {
  sites: SiteSummary[]
  rootSiteId: number
}

interface InitRequest extends Record<string, unknown> {
  siteId: number
}

interface SubmitCoreRequest extends Record<string, unknown> {
  siteId: number
  siteName: string
  description?: string
  iconUrl?: string
  siteDir?: string
}

interface DisableRequest extends Record<string, unknown> {
  siteId: number
}

interface DeleteAIRequest extends Record<string, unknown> {
  siteId: number
  siteDir: string
}

interface DeleteCoreRequest extends Record<string, unknown> {
  siteId: number
  siteDir: string
}

interface OrderRequest extends Record<string, unknown> {
  siteId: number
  isUp: boolean
  rows: number
}

interface GetTemplatesRequest extends Record<string, unknown> {
  siteType: string
}

interface GetTemplatesResult extends Record<string, unknown> {
  siteTemplates: SiteTemplate[]
}

const appsApi = {
  getUploadUrl: (siteId: number) => {
    return api.getUploadUrl(urlUpload, { siteId })
  },

  get: async () => {
    return await api.get<GetResult>(URL_AI)
  },

  init: async (request: InitRequest) => {
    return await api.post<BoolResult>(urlInit, request)
  },

  submitCore: async (request: SubmitCoreRequest) => {
    return await api.post<IntResult>(URL_CORE, request)
  },

  disable: async (request: DisableRequest) => {
    return await api.post(urlDisable, request)
  },

  deleteAI: async (request: DeleteAIRequest) => {
    return await api.post(urlDeleteAI, request)
  },

  deleteCore: async (request: DeleteCoreRequest) => {
    return await api.post(urlDeleteCore, request)
  },

  order: async (request: OrderRequest) => {
    return await api.post(urlOrder, request)
  },

  getTemplates: async (request: GetTemplatesRequest) => {
    return await api.post<GetTemplatesResult>(urlGetTemplates, request)
  },

  getUploadTemplateUrl: () => {
    return api.getUploadUrl(urlUploadTemplate)
  },
}

export default appsApi
