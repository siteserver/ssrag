import { v4 as uuidv4 } from 'uuid'

export function uuid() {
  return uuidv4()
}

export function getUniqueId() {
  return 'a' + uuid()
}

export function contains(strs: string | string[] | undefined, str: string) {
  if (!strs || !str) return false
  const strsArray = Array.isArray(strs) ? strs : strs.split(',')
  return strsArray.some((s) => s === str)
}

export function split(
  str: string | undefined,
  separator: string = ','
): string[] {
  if (!str) return []
  return str.split(separator).map((s) => s.trim())
}

export function join(
  strs: string[] | undefined,
  separator: string = ','
): string {
  if (!strs) return ''
  return strs.join(separator).trim()
}
