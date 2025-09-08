import { AiParameters } from '@/dto/aiMessages'
import { SiteValues, Site } from '@/models'
import { getQueryInt } from '@/utils'
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export interface ChatState {
  siteId: number
  sessionId: string
  site: Site
  values: SiteValues
  aiParameters: AiParameters
  aiContent: string
  aiLoading: boolean
  init: (sessionId: string, site: Site, values: SiteValues) => void
  setAiParameters: (parameters: AiParameters) => void
  setAiContent: (value: string) => void
  setAiLoading: (value: boolean) => void
}

export const useChatStore = create<ChatState>()(
  immer((set) => ({
    siteId: getQueryInt('siteId'),
    sessionId: '',
    site: {} as Site,
    values: {} as SiteValues,
    aiParameters: {} as AiParameters,
    aiContent: '',
    aiLoading: false,

    init: (sessionId: string, site: Site, values: SiteValues) => {
      set((state) => {
        state.siteId = site.id
        state.sessionId = sessionId
        state.site = site
        state.values = values
        state.aiParameters = {
          thinking: true,
          searching: true,
        }
      })
    },

    setAiParameters: (parameters: AiParameters) => {
      set((state) => {
        state.aiParameters = parameters
      })
    },

    setAiContent: (value: string) => {
      set((state) => {
        if (value) {
          state.aiLoading = true
        }
        state.aiContent = value
      })
    },

    setAiLoading: (value: boolean) => {
      set((state) => {
        state.aiLoading = value
      })
    },
  }))
)
