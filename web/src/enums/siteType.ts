export enum SiteType {
  Web = 'Web',
  Markdown = 'Markdown',
  Document = 'Document',
  Chat = 'Chat',
  Chatflow = 'Chatflow',
  Agent = 'Agent',
}

export function getSiteTypeName(siteType: SiteType) {
  switch (siteType) {
    case SiteType.Web:
      return '网站知识库'
    case SiteType.Markdown:
      return 'Markdown知识库'
    case SiteType.Document:
      return '文档知识库'
    case SiteType.Chat:
      return '对话应用'
    case SiteType.Chatflow:
      return '工作流应用'
    case SiteType.Agent:
      return '智能体应用'
    default:
      return '项目'
  }
}

export function isAppSite(siteType: SiteType) {
  return (
    siteType === SiteType.Chat ||
    siteType === SiteType.Chatflow ||
    siteType === SiteType.Agent
  )
}
