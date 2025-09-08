import { ModelType } from '@/enums'

export interface Model {
  id: number
  uuid: string
  createdDate: Date
  lastModifiedDate: Date
  providerId: string
  providerName: string
  iconUrl: string
  modelType: ModelType
  modelId: string
  skills: string[]
  extendValues: Record<string, unknown>
}
