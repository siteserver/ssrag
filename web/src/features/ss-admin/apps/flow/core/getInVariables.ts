import { RunVariable } from '@/dto'
import { VariableDataType, VariableType } from '@/enums'

export function getInVariables(variables?: RunVariable[]): RunVariable[] {
  const list = variables || []
  if (list.length === 0) {
    list.push({
      type: VariableType.Input,
      name: 'input',
      dataType: VariableDataType.String,
      value: '',
    })
  }
  return list.map((item, index) => {
    return { ...item, key: index.toString() }
  })
}
