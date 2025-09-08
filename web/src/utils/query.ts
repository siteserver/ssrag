import { toInt } from './toInt'

export function addQuery(url: string, query?: Record<string, unknown>) {
  if (!url) {
    return ''
  }
  url += url.indexOf('?') === -1 ? '?' : '&'
  if (query) {
    for (const [key, value] of Object.entries(query)) {
      url += key + '=' + encodeURIComponent(String(value)) + '&'
    }
  }
  return url.slice(0, -1)
}

export function getQueryString(name: string, defaultValue: string = '') {
  const result = window.location.search.match(
    new RegExp('[?&]' + name + '=([^&]+)', 'i')
  )
  if (!result || result.length < 1) {
    return defaultValue
  }
  return result[1]
}

export function getQueryInt(name: string, defaultValue: number = 0) {
  const result = window.location.search.match(
    new RegExp('[?&]' + name + '=([^&]+)', 'i')
  )
  if (!result || result.length < 1) {
    return defaultValue
  }
  return toInt(result[1], defaultValue)
}
