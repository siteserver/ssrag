import { Model } from '@/dto'
import { Prompt, Site, SiteSummary, SiteValues } from '@/models'
import { getQueryInt } from '@/utils'
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

interface State {
  siteId: number
  site: Site
  values: SiteValues
  models: Model[]
  defaultModel: Model | null
  datasetSites: SiteSummary[]
  hotPrompts: Prompt[]
  functionPrompts: Prompt[]
  inputPrompts: Prompt[]
  initialized: boolean
  init: (
    site: Site,
    values: SiteValues,
    models: Model[],
    defaultModel: Model | null,
    datasetSites: SiteSummary[],
    hotPrompts: Prompt[],
    functionPrompts: Prompt[],
    inputPrompts: Prompt[]
  ) => void
  addDatasetSite: (site: SiteSummary) => void
  updateDatasetSites: (sites: SiteSummary[]) => void
}

export const useStore = create(
  immer<State>((set) => ({
    siteId: getQueryInt('siteId') || 0,
    site: {} as Site,
    values: {} as SiteValues,
    models: [],
    defaultModel: null,
    datasetSites: [],
    hotPrompts: [],
    functionPrompts: [],
    inputPrompts: [],
    initialized: false,

    init: (
      site: Site,
      values: SiteValues,
      models: Model[],
      defaultModel: Model | null,
      datasetSites: SiteSummary[],
      hotPrompts: Prompt[],
      functionPrompts: Prompt[],
      inputPrompts: Prompt[]
    ) => {
      set((state) => {
        state.site = site
        state.values = values
        state.models = models
        state.defaultModel = defaultModel
        state.datasetSites = datasetSites
        state.hotPrompts = hotPrompts
        state.functionPrompts = functionPrompts
        state.inputPrompts = inputPrompts
        state.initialized = true
      })
    },

    addDatasetSite: (site: SiteSummary) => {
      set((state) => {
        state.datasetSites = [...state.datasetSites, site]
      })
    },

    updateDatasetSites: (sites: SiteSummary[]) => {
      set((state) => {
        state.datasetSites = sites
      })
    },
  }))
)
