import { NodeType } from '@/enums'
import IconAsk from '@/assets/nodes/ask.svg?react'
import IconDataset from '@/assets/nodes/dataset.svg?react'
import IconEnd from '@/assets/nodes/end.svg?react'
import IconHttp from '@/assets/nodes/http.svg?react'
import IconInput from '@/assets/nodes/input.svg?react'
import IconIntent from '@/assets/nodes/intent.svg?react'
import IconLLM from '@/assets/nodes/llm.svg?react'
import IconOutput from '@/assets/nodes/output.svg?react'
import IconSql from '@/assets/nodes/sql.svg?react'
import IconStart from '@/assets/nodes/start.svg?react'
import IconText from '@/assets/nodes/text.svg?react'
import IconWebSearch from '@/assets/nodes/websearch.svg?react'

const NodeIcon: React.FC<{
  type: NodeType | string
  isFixed?: boolean
  className?: string
}> = ({ type, isFixed, className }) => {
  if (type === NodeType.WebSearch) {
    return <IconWebSearch className={className} />
  } else if (type === NodeType.Start) {
    return <IconStart className={className} />
  } else if (type === NodeType.Input) {
    return <IconInput className={className} />
  } else if (type === NodeType.LLM) {
    return <IconLLM className={className} />
  } else if (type === NodeType.Intent) {
    return <IconIntent className={className} />
  } else if (type === NodeType.Dataset) {
    return <IconDataset className={className} />
  } else if (type === NodeType.Output && isFixed) {
    return <IconEnd className={className} />
  } else if (type === NodeType.Output) {
    return <IconOutput className={className} />
  } else if (type === NodeType.Http) {
    return <IconHttp className={className} />
  } else if (type === NodeType.Sql) {
    return <IconSql className={className} />
  } else if (type === NodeType.Text) {
    return <IconText className={className} />
  } else if (type === NodeType.Ask) {
    return <IconAsk className={className} />
  }
  return null
}

export default NodeIcon
