export function getFileSize(size: number) {
  if (!size) return ''
  let displaySize = size + ' B'
  if (size >= 1024 * 1024) {
    displaySize = (size / (1024 * 1024)).toFixed(2) + ' MB'
  } else if (size >= 1024) {
    displaySize = (size / 1024).toFixed(2) + ' KB'
  }
  return displaySize
}
