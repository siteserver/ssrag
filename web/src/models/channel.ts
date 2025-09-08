export interface Channel {
  id: number
  uuid: string
  createdDate: Date
  lastModifiedDate: Date
  siteId: number
  siteDir: string
  siteName: string
  siteType: string
  imageUrl: string
  keywords: string
  description: string
  tableName: string
  root: boolean
  parentId: number
  taxis: number

  separators: string[]
  chunkSize: number
  chunkOverlap: number
  isChunkReplaces: boolean
  isChunkDeletes: boolean
}
