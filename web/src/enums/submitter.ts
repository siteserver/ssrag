export enum Submitter {
  All = 'All',
  Authenticated = 'Authenticated',
  Users = 'Users',
  None = 'None',
}

export const SubmitterOptions = [
  { label: '所有人', value: Submitter.All },
  { label: '已登录用户', value: Submitter.Authenticated },
  { label: '指定用户', value: Submitter.Users },
  { label: '禁止提交', value: Submitter.None },
]
