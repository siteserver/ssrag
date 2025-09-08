import { ValidateType } from '../enums/validateType'

export interface InputStyleRule {
  type: ValidateType
  value: string
  message: string
}
