export enum Status {
  Default = '',
  Checking = 'Checking',
  Doing = 'Doing',
  Revoked = 'Revoked',
  Checked = 'Checked',
  Rejected = 'Rejected',
}

export const StatusColor = {
  [Status.Checking]: 'warning',
  [Status.Doing]: 'warning',
  [Status.Revoked]: 'error',
  [Status.Checked]: 'success',
  [Status.Rejected]: 'error',
}

export const StatusDisplayName = {
  [Status.Default]: '默认',
  [Status.Checking]: '审批中',
  [Status.Doing]: '办理中',
  [Status.Revoked]: '已撤回',
  [Status.Checked]: '审批通过',
  [Status.Rejected]: '审批拒绝',
}
