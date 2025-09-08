import { RuleType } from '@/enums'

export interface Rule {
  type?: RuleType
  required?: boolean
  message: string
  pattern?: RegExp | string
}
