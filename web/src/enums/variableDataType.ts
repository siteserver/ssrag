export enum VariableDataType {
  String = 'String',
  Integer = 'Integer',
  Number = 'Number',
  Boolean = 'Boolean',
  Object = 'Object',
  ArrayString = 'ArrayString',
  ArrayInteger = 'ArrayInteger',
  ArrayNumber = 'ArrayNumber',
  ArrayBoolean = 'ArrayBoolean',
  ArrayObject = 'ArrayObject',
}

export const VariableDataTypeOptions = [
  { label: '字符串', value: VariableDataType.String },
  { label: '整数', value: VariableDataType.Integer },
  { label: '数字', value: VariableDataType.Number },
  { label: '布尔值', value: VariableDataType.Boolean },
  { label: '对象', value: VariableDataType.Object },
  { label: '字符串数组', value: VariableDataType.ArrayString },
  { label: '整数数组', value: VariableDataType.ArrayInteger },
  { label: '数字数组', value: VariableDataType.ArrayNumber },
  { label: '布尔值数组', value: VariableDataType.ArrayBoolean },
  { label: '对象数组', value: VariableDataType.ArrayObject },
]
