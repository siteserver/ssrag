export interface CeleryTask {
  id: number
  uuid: string
  createdDate: Date
  lastModifiedDate: Date
  siteId: number
  channelId: number
  contentId: number
  taskId: string
  taskName: string
  taskStatus: string
  taskResult: string
  resultObj: {
    id: number
    fileName: string
    extName: string
    fileSize: number
    title: string
  } | null
}
