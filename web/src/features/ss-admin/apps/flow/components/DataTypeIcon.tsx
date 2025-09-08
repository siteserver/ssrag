import { VariableDataType } from '@/enums'
import IconArrayObject from '@/assets/dataTypes/arrayObject.svg?react'
import IconBoolean from '@/assets/dataTypes/boolean.svg?react'
import IconInteger from '@/assets/dataTypes/integer.svg?react'
import IconNumber from '@/assets/dataTypes/number.svg?react'
import IconObject from '@/assets/dataTypes/object.svg?react'
import IconString from '@/assets/dataTypes/string.svg?react'

const DataTypeIcon: React.FC<{
  type: VariableDataType | undefined
  className?: string
}> = ({ type, className }) => {
  if (type === VariableDataType.String) {
    return (
      <IconString
        className={className}
        style={{ color: '#999', marginLeft: '3px' }}
      />
    )
  } else if (type === VariableDataType.Integer) {
    return (
      <IconInteger
        className={className}
        style={{ color: '#999', marginLeft: '3px' }}
      />
    )
  } else if (type === VariableDataType.Boolean) {
    return (
      <IconBoolean
        className={className}
        style={{ color: '#999', marginLeft: '3px' }}
      />
    )
  } else if (type === VariableDataType.Number) {
    return (
      <IconNumber
        className={className}
        style={{ color: '#999', marginLeft: '3px' }}
      />
    )
  } else if (type === VariableDataType.Object) {
    return (
      <IconObject
        className={className}
        style={{ color: '#999', marginLeft: '3px' }}
      />
    )
  } else if (type === VariableDataType.ArrayObject) {
    return (
      <IconArrayObject
        className={className}
        style={{ color: '#999', marginLeft: '3px' }}
      />
    )
  }
  return null
}

export default DataTypeIcon
