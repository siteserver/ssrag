import { Cascade } from '@/dto'
import { Document } from '@/models'
import { getQueryInt } from '@/utils'
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

interface DocumentsState {
  siteId: number
  channelId: number
  contentId: number
  siteUrl: string
  channels: Cascade<number>[]
  document: Document | null
  loading: boolean
  page: string
  prevPage: string
  initialized: boolean
  init: (siteUrl: string, channels: Cascade<number>[]) => void
  setLoading: (loading: boolean) => void
  setDocument: (document: Document | null) => void
  setPage: (page: 'split' | 'list' | 'card' | 'importing' | 'view') => void
  goBack: () => void
  setChannelId: (channelId: number) => void
}

export const useDocumentsStore = create(
  immer<DocumentsState>((set) => ({
    siteId: getQueryInt('siteId'),
    channelId: getQueryInt('channelId'),
    contentId: getQueryInt('contentId'),
    siteUrl: '',
    channels: [],
    document: null,
    loading: false,
    page: 'split',
    prevPage: 'split',
    initialized: false,

    init: (siteUrl: string, channels: Cascade<number>[]) => {
      set((state) => {
        state.siteUrl = siteUrl
        state.channels = channels
        state.initialized = true
      })
    },

    setLoading: (loading: boolean) => {
      set((state) => {
        state.loading = loading
      })
    },

    setDocument: (document: Document | null) => {
      set((state) => {
        state.document = document
        if (document) {
          state.prevPage = state.page
          state.page = 'view'
        }
      })
    },

    setChannelId: (channelId: number) => {
      set((state) => {
        state.channelId = channelId
        state.contentId = 0
      })
    },

    setPage: (page: string) => {
      set((state) => {
        state.prevPage = state.page
        state.page = page
      })
    },

    goBack: () => {
      set((state) => {
        state.page = state.prevPage
      })
    },
  }))
)
