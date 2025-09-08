import { Site, SiteValues } from '@/models'
import api from '../..'

const url = '/ai/admin/apps/publish'
const urlUpload = `${url}/actions/upload`

interface GetRequest extends Record<string, unknown> {
  siteId: number
}

interface GetResult {
  site: Site
  values: SiteValues
}

const publishApi = {
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
}

export default publishApi
