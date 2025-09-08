export enum DisplayType {
  Home = 'Home',
  Chat = 'Chat',
  Copilot = 'Copilot',
}

export function getDisplayTypeName(siteType: DisplayType) {
  switch (siteType) {
    case DisplayType.Home:
      return '独立式'
    case DisplayType.Chat:
      return '嵌入式'
    case DisplayType.Copilot:
      return '助手式'
    default:
      return '独立式'
  }
}
