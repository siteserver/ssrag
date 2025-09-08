export interface Data {
  id: number
  sys_no?: string
  sys_created_at?: Date
  sys_status?: string
  sys_user_name?: string
  sys_node_id?: number
  sys_comment?: string
  values: Record<string, string | string[] | null | undefined>
}
