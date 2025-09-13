import { StringResult } from '@/dto'
import { ModelProvider, ProviderManifest } from '@/models'
import api from '../..'

const url = '/ai/admin/settings/configsModels'
const urlSubmitProvider = `${url}/actions/submitProvider`
const urlSubmitModel = `${url}/actions/submitModel`
const urlDeleteProvider = `${url}/actions/deleteProvider`
const urlDeleteModel = `${url}/actions/deleteModel`
const urlGetDefaults = `${url}/actions/getDefaults`
const urlSubmitDefaults = `${url}/actions/submitDefaults`
const urlStatus = `${url}/actions/status`

interface GetResult {
  providers: ModelProvider[]
  configValues: {
    defaultLLMProviderId: string
    defaultLLMModelId: string
    defaultTextEmbeddingProviderId: string
    defaultTextEmbeddingModelId: string
    defaultRerankProviderId: string
    defaultRerankModelId: string
    defaultSpeech2TextProviderId: string
    defaultSpeech2TextModelId: string
    defaultTTSProviderId: string
    defaultTTSModelId: string
  }
  tenantId: string
}

interface GetManifestsResult {
  manifests: ProviderManifest[]
}

interface SubmitProviderRequest extends Record<string, unknown> {
  providerId: string
  providerName: string
  iconUrl: string
  description: string
  credentials: Record<string, unknown>
}

interface SubmitModelRequest extends Record<string, unknown> {
  id: number
  providerId: string
  modelType: string
  modelId: string
  skills: string[]
  extendValues: Record<string, unknown>
}

interface DeleteProviderRequest extends Record<string, unknown> {
  providerId: string
}

interface DeleteModelRequest extends Record<string, unknown> {
  providerId: string
  modelId: string
}

export interface GetDefaultsResult {
  defaultLLMProviderId: string
  defaultLLMModelId: string
  defaultTextEmbeddingProviderId: string
  defaultTextEmbeddingModelId: string
  defaultRerankProviderId: string
  defaultRerankModelId: string
  defaultSpeech2TextProviderId: string
  defaultSpeech2TextModelId: string
  defaultTTSProviderId: string
  defaultTTSModelId: string
  llmProviders: ModelProvider[]
  textEmbeddingProviders: ModelProvider[]
  rerankProviders: ModelProvider[]
  speech2TextProviders: ModelProvider[]
  ttsProviders: ModelProvider[]
}

interface SubmitDefaultsRequest extends Record<string, unknown> {
  defaultLLMProviderId: string
  defaultLLMModelId: string
  defaultTextEmbeddingProviderId: string
  defaultTextEmbeddingModelId: string
  defaultRerankProviderId: string
  defaultRerankModelId: string
  defaultSpeech2TextProviderId: string
  defaultSpeech2TextModelId: string
  defaultTTSProviderId: string
  defaultTTSModelId: string
}

const configsModelsApi = {
  get: async () => {
    return await api.get<GetResult>(url)
  },

  getManifests: async () => {
    // return await api.get<GetManifestsResult>(`${url}/manifests`)
    return await fetch('/assets/json/manifests.json').then((response) => {
      return response.json() as Promise<GetManifestsResult>
    })
  },

  submitProvider: async (request: SubmitProviderRequest) => {
    return await api.post<SubmitProviderRequest>(urlSubmitProvider, request)
  },

  submitModel: async (request: SubmitModelRequest) => {
    return await api.post<SubmitModelRequest>(urlSubmitModel, request)
  },

  deleteProvider: async (request: DeleteProviderRequest) => {
    return await api.post<DeleteProviderRequest>(urlDeleteProvider, request)
  },

  deleteModel: async (request: DeleteModelRequest) => {
    return await api.post<DeleteModelRequest>(urlDeleteModel, request)
  },

  getDefaults: async () => {
    return await api.post<GetDefaultsResult>(urlGetDefaults)
  },

  submitDefaults: async (request: SubmitDefaultsRequest) => {
    return await api.post<StringResult>(urlSubmitDefaults, request)
  },

  status: async (request: { taskId: string }) => {
    return await api.post<{
      state: string
      result: {
        count: number
        current: number
      }
    }>(urlStatus, request)
  },
}

export default configsModelsApi
