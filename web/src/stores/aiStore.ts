import { ChatGroup, SiteSummary } from '@/models'
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export interface AiState {
  sites: SiteSummary[]
  chatGroups: ChatGroup[]

  init: (sites: SiteSummary[], chatGroups: ChatGroup[]) => void
  addChatGroup: (chatGroup: ChatGroup) => void
}

export const useAiStore = create<AiState>()(
  immer((set) => ({
    sites: [],
    chatGroups: [],

    init: (sites: SiteSummary[], chatGroups: ChatGroup[]) => {
      set((state) => {
        state.sites = sites
        state.chatGroups = chatGroups
      })
    },

    addChatGroup: (chatGroup: ChatGroup) => {
      set((state) => {
        if (
          !state.chatGroups.find(
            (group) => group.sessionId === chatGroup.sessionId
          )
        ) {
          state.chatGroups.push(chatGroup)
        } else {
          const group = state.chatGroups.find(
            (group) => group.sessionId === chatGroup.sessionId
          )
          if (group) {
            group.title = chatGroup.title
          }
        }
      })
    },
  }))
)
