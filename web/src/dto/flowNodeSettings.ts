import {
  DatabaseType,
  NodeType,
  OutputFormat,
  SearchType,
  TextProcessType,
} from '../enums'

export interface FlowNodeSettings extends Record<string, unknown> {
  id?: string
  uuid?: string
  createdDate?: Date
  lastModifiedDate?: Date
  siteId?: number
  nodeId?: number
  parentId?: number
  nodeType?: NodeType
  title?: string
  description?: string
  pluginIds?: string[]

  height?: number
  width?: number
  positionX?: number
  positionY?: number
  isFixed?: boolean

  providerModelId?: string
  modelTemperature?: number
  modelTopP?: number
  modelMaxResponseLength?: number
  isIgnoreExceptions?: boolean
  outputFormat?: OutputFormat
  chatHistoryEnabled?: boolean
  chatHistoryCount?: number

  // LLM
  llmSystemPrompt?: string
  llmUserPrompt?: string
  llmIsReply?: boolean
  llmOutputFormat?: string

  // WebSearch
  webSearchApiKey?: string
  webSearchFreshness?: string
  webSearchSummary?: boolean
  webSearchInclude?: string
  webSearchExclude?: string
  webSearchCount?: number

  // Dataset
  datasetSearchType?: SearchType
  datasetMaxCount?: number
  datasetMinScore?: number

  // HTTP
  httpMethod?: string
  httpUrl?: string
  httpSecurityKey?: string

  // 问答
  question?: string
  options?: string[]
  isMultiSelect?: boolean
  isDirectReply?: boolean

  // Intent
  intentPrompt?: string
  intentions?: string[]

  // 输出
  isStreaming?: boolean
  output?: string

  // 运行
  isRun?: boolean
  runEvery?: number
  runWeekdays?: number[]
  runSpecifiedDate?: string
  runFormFieldDate?: string
  runFormFieldDateOffset?: number
  runStartTime?: string
  runTimeout?: number

  // SQL 查询
  isSqlDatabase?: boolean
  sqlDatabaseType?: DatabaseType
  sqlDatabaseHost?: string
  isSqlDatabasePort?: boolean
  sqlDatabasePort?: number
  sqlDatabaseUserName?: string
  sqlDatabasePassword?: string
  sqlDatabaseName?: string
  sqlQueryString?: string

  // 文本处理
  textProcessType?: TextProcessType
  textSplit?: string
  textReplace?: string
  textTo?: string
  isTextCaseIgnore?: boolean
  isTextRegex?: boolean
  textJoint?: string

  type?: NodeType | string
  position?: { x: number; y: number }
  deletable?: boolean
  style?: {
    width?: number
    height?: number
    zIndex?: number
  }
}
