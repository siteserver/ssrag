export interface UserGroup {
  id: number
  isDefault: boolean
  isManager: boolean
  homePermissions: string[]
  groupName: string
  taxis: number
  description: string
}
