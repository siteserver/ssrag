export enum NodeType {
  Start = 'Start',
  Input = 'Input',
  LLM = 'LLM',
  Intent = 'Intent',
  Dataset = 'Dataset',
  Output = 'Output',
  Http = 'Http',
  WebSearch = 'WebSearch',
  Text = 'Text',
  Sql = 'Sql',
  Ask = 'Ask',
}

export function getNodeTypeName(nodeType: NodeType) {
  switch (nodeType) {
    case NodeType.Start:
      return '开始'
    case NodeType.Input:
      return '输入'
    case NodeType.LLM:
      return '大模型'
    case NodeType.Intent:
      return '意图识别'
    case NodeType.Dataset:
      return '知识库'
    case NodeType.Output:
      return '输出'
    case NodeType.Http:
      return 'HTTP 调用'
    case NodeType.WebSearch:
      return '联网搜索'
    case NodeType.Sql:
      return 'SQL 查询'
    case NodeType.Text:
      return '文本处理'
    case NodeType.Ask:
      return '提问'
    default:
      return '节点'
  }
}
