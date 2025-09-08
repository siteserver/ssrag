export enum DatabaseType {
  MySql = 'MySql',
  SqlServer = 'SqlServer',
  PostgreSql = 'PostgreSql',
  KingbaseES = 'KingbaseES',
  Dm = 'Dm',
}

export const DatabaseTypeOptions = [
  { label: 'MySql', value: DatabaseType.MySql },
  { label: 'SqlServer', value: DatabaseType.SqlServer },
  { label: 'PostgreSql', value: DatabaseType.PostgreSql },
  { label: '人大金仓', value: DatabaseType.KingbaseES },
  { label: '达梦', value: DatabaseType.Dm },
]
