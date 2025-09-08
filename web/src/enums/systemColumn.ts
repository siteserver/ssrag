export enum SystemColumn {
  Id = 'id',
  No = 'sys_no',
  CreatedAt = 'sys_created_at',
  Status = 'sys_status',
  UserName = 'sys_user_name',
  NodeId = 'sys_node_id',
  Comment = 'sys_comment',
}

export const SystemColumnDisplayName = {
  [SystemColumn.Id]: '唯一标识（主键）',
  [SystemColumn.No]: '审批编号',
  [SystemColumn.CreatedAt]: '创建时间',
  [SystemColumn.Status]: '审批状态',
  [SystemColumn.UserName]: '提交人',
  [SystemColumn.NodeId]: '当前审批节点',
  [SystemColumn.Comment]: '审批意见',
}
