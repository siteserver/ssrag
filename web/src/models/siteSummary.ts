import { SiteType } from '@/enums'

export interface SiteSummary {
  id: number
  siteName: string
  siteType: SiteType
  iconUrl: string
  siteDir: string
  description: string
  tableName: string
  root: boolean
  disabled: boolean
  taxis: number

  isMoving: boolean
  isDragOver: boolean
}
