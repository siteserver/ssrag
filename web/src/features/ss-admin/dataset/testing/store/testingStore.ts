import { Cascade } from '@/dto'
import { Segment } from '@/models'
import { getQueryInt } from '@/utils'
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

interface TestingState {
  siteId: number
  channelId: number
  contentId: number
  siteUrl: string
  channels: Cascade<number>[]
  segments: Segment[]
  loading: boolean
  initialized: boolean
  init: (siteUrl: string, channels: Cascade<number>[]) => void
  setLoading: (loading: boolean) => void
  setSegments: (segments: Segment[]) => void
  setChannelId: (channelId: number) => void
}

export const useTestingStore = create(
  immer<TestingState>((set) => ({
    siteId: getQueryInt('siteId'),
    channelId: getQueryInt('channelId'),
    contentId: getQueryInt('contentId'),
    siteUrl: '',
    channels: [],
    segments: [],
    loading: false,
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

    setChannelId: (channelId: number) => {
      set((state) => {
        state.channelId = channelId
      })
    },

    setSegments: (segments: Segment[]) => {
      set((state) => {
        state.segments = segments
      })
    },
  }))
)
