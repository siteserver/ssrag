import { InputConditionType } from '@/enums'

export interface InputStyleCondition {
  attributeName: string
  conditionType: InputConditionType
  values: string[]
  and: boolean
}
