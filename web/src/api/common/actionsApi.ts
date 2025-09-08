import api from '..'

const url = '/apps/common/actions'
export const urlUpload = `${url}/upload`
export const urlGetStorageUrl = `${url}/getStorageUrl`

interface GetStorageUrlRequest extends Record<string, unknown> {
  siteId: number
  fileUrl: string
  isPreview: boolean
}

interface GetStorageUrlResult extends Record<string, unknown> {
  fileUrl: string
}

const actionsApi = {
  getUploadUrl: (siteId: number, isPrivate: boolean) => {
    return `/api${urlUpload}?siteId=${siteId}&isPrivate=${isPrivate}`
  },

  getUploadHeaders: () => {
    return {
      Authorization: `Bearer ${api.token}`,
    }
  },

  getStorageUrl: async (request: GetStorageUrlRequest) => {
    return await api.post<GetStorageUrlResult>(urlGetStorageUrl, request)
  },
}

export default actionsApi
