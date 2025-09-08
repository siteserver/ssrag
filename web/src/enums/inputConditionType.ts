export enum InputConditionType {
  Any = 'Any',
  All = 'All',
  NotAny = 'NotAny',
  NotAll = 'NotAll',
}

export const InputConditionTypeDisplayName: Record<InputConditionType, string> =
  {
    [InputConditionType.Any]: '选择任一选项',
    [InputConditionType.All]: '选择全部选项',
    [InputConditionType.NotAny]: '没有选择任一',
    [InputConditionType.NotAll]: '没有选择全部',
  }
