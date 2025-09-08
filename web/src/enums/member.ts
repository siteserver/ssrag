export enum Member {
  Users = 'Users',
  Departments = 'Departments',
  Groups = 'Groups',
}

export const MemberOptions = [
  { label: '人员', value: Member.Users },
  { label: '部门', value: Member.Departments },
  { label: '用户组', value: Member.Groups },
]
