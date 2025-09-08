import { RunVariable } from '.'

export interface NodeRunning {
  isRun?: boolean
  isRunning?: boolean
  inVariables?: RunVariable[]
  outVariables?: RunVariable[]
  success?: boolean
  errorMessage?: string
  seconds?: string
}
