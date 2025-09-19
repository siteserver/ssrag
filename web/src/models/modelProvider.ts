import { Model } from './model'

export interface ModelProvider {
  id: number
  uuid: string
  createdDate: Date
  lastModifiedDate: Date
  providerId: string
  providerName: string
  iconUrl: string
  description: string
  credentials: Record<string, unknown>
  models: Model[]
}

export interface ManifestModelJson {
  model: string
  label?: {
    en_US?: string
    zh_Hans?: string
  }
  model_type: string
  model_properties: Record<string, unknown>
  skills: string[]
  deprecated?: boolean
  order?: number
}

export interface ProviderManifestModel {
  background: string
  configurate_methods: string[]
  help: {
    title: {
      en_US: string
      zh_Hans: string
    }
    url: {
      en_US: string
      zh_Hans: string
    }
  }
  icon_large: {
    en_US: string
  }
  icon_small: {
    en_US: string
  }
  label: {
    en_US: string
    zh_Hans: string
  }
  models: ManifestModelJson[]
  provider: string
  model_credential_schema: {
    credential_form_schemas: Array<{
      default: string
      label: {
        en_US: string
        zh_Hans?: string
      }
      placeholder: {
        en_US: string
        zh_Hans?: string
      }
      max_length: number
      options: {
        label: {
          en_US: string
          zh_Hans: string
        }
        value: string
      }[]
      required: boolean
      type: string
      variable: string
      model_types: string[]
    }>
  }
  provider_credential_schema: {
    credential_form_schemas: Array<{
      default: string
      label: {
        en_US: string
        zh_Hans?: string
      }
      placeholder: {
        en_US: string
        zh_Hans?: string
      }
      max_length: number
      options: {
        label: {
          en_US: string
          zh_Hans: string
        }
        value: string
      }[]
      required: boolean
      type: string
      variable: string
    }>
  }
  supported_model_types: string[]
}

export interface ProviderManifest {
  name: string
  model: ProviderManifestModel
  label: {
    en_US: string
    zh_Hans: string
  }
  description: {
    en_US: string
    zh_Hans: string
  }
  icon: string
  meta: {
    arch: string[]
    runner: {
      entrypoint: string
      language: string
      version: string
    }
    version: string
  }
  plugins: {
    models: string[]
  }
  resource: {
    memory: number
    permission: {
      model: {
        enabled: boolean
        llm: boolean
        moderation: boolean
        rerank: boolean
        speech2text: boolean
        text_embedding: boolean
        tts: boolean
      }
      tool: {
        enabled: boolean
      }
    }
  }
  type: string
  version: string
}

export function getModelLabel(model: ManifestModelJson): string {
  return model.label?.zh_Hans || model.label?.en_US || model.model
}
