export function toInt(val: string, defaultValue: number = 0) {
  if (!val) return defaultValue
  try {
    return parseInt(val)
  } catch {
    return defaultValue
  }
}
