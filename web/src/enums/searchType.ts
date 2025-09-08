export enum SearchType {
  Semantic = 'Semantic',
  FullText = 'FullText',
  Hybrid = 'Hybrid',
}

export const SearchTypeDisplayName = {
  [SearchType.Semantic]: '语义检索',
  [SearchType.FullText]: '全文检索',
  [SearchType.Hybrid]: '混合检索',
}
