import { DocumentStatus } from '../enums/documentStatus'
import { Segment } from './segment'

export interface Document {
  id: number
  uuid: string
  createdDate: Date
  lastModifiedDate: Date
  siteId: number
  channelId: number
  contentId: number
  title: string
  fileName: string
  extName: string
  fileSize: number
  status: DocumentStatus
  separators?: string
  chunkSize?: number
  chunkOverlap?: number
  isChunkReplaces?: boolean
  isChunkDeletes?: boolean

  segments: Segment[]
  chunks: string[]
}
