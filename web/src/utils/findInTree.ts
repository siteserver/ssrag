import { Cascade } from '@/dto'

export function findInTree<T>(
  array: Cascade<T>[],
  predicate: (item: Cascade<T>) => boolean
): Cascade<T> | null {
  for (const item of array) {
    if (predicate(item)) {
      return item
    }
    if (item.children && item.children.length > 0) {
      const found = findInTree(item.children, predicate)
      if (found) {
        return found
      }
    }
  }
  return null
}
