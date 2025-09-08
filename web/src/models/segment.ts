export interface Segment {
  id: string
  createdDate: Date
  lastModifiedDate: Date
  siteId: number
  channelId: number
  contentId: number
  documentId: number
  taxis: number
  text: string
  textHash?: string
}
