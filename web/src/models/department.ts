export interface Department {
  id: number
  code: string
  name: string
  parentId: number
  parentsPath: number[]
  parentsCount: number
  childrenCount: number
  count: number
  wxCount: number
  wxPartyId: number
  managerUserNames: string
  taxis: number
  description: string
  fullName: string
  children: Department[]
}
