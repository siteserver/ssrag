import { Cascade } from '@/dto'
import { User, UserGroup } from '@/models'
import api from '..'

const url = '/admin/apps/common/modalSelectMembers'
const urlUsers = `${url}/actions/users`
const urlDepartments = `${url}/actions/departments`

export interface GetResult {
  departments: Cascade<number>[]
  groups: UserGroup[]
  count: number
  users: User[]
  pageSize: number
  userLevelCount: number
  userLevelName1: string
  userLevelName2: string
  userLevelName3: string
  userLevelName4: string
  userLevelName5: string
  userLevelName6: string
  userLevelName7: string
  userLevelName8: string
  userLevelName9: string
  [key: string]:
    | string
    | number
    | undefined
    | Cascade<number>[]
    | UserGroup[]
    | User[]
}

interface UsersRequest extends Record<string, unknown> {
  departmentIds: number[]
  groupId: number
  keyword: string
  page: number
  pageSize: number
}

interface UsersResult extends Record<string, unknown> {
  count: number
  users: User[]
}

interface DepartmentsRequest extends Record<string, unknown> {
  keyword: string
}

interface DepartmentsResult extends Record<string, unknown> {
  departments: Cascade<number>[]
}

const modalSelectMembersApi = {
  get: async () => {
    return await api.get<GetResult>(url)
  },

  users: async (request: UsersRequest) => {
    return await api.post<UsersResult>(urlUsers, request)
  },

  departments: async (request: DepartmentsRequest) => {
    return await api.post<DepartmentsResult>(urlDepartments, request)
  },
}

export default modalSelectMembersApi
