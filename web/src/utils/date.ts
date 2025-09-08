import dayjs from 'dayjs'

export function getSeconds(startTime: Date, endTime?: Date) {
  endTime = endTime ?? new Date()
  const elapsedTime = endTime.getTime() - startTime.getTime()
  return (elapsedTime / 1000).toFixed(2)
}

export function getDatejs(value: string | null): dayjs.Dayjs | null {
  if (!value) return null
  value = value.trim()
  if (value.match(/^\d{2}:\d{2}$/)) {
    const today = new Date()
    const [hours, minutes] = value.split(':')
    today.setHours(parseInt(hours), parseInt(minutes))
    today.setFullYear(today.getFullYear(), today.getMonth(), today.getDate())
    return getDatejs(today.toISOString())
  }
  return dayjs(value)
}

export function getCurrent(): dayjs.Dayjs {
  return dayjs()
}

function getDateFormat(picker?: string, showTime?: boolean) {
  if (picker === 'week') return 'YYYY-w周'
  if (picker === 'month') return 'YYYY-MM'
  if (picker === 'quarter') return 'YYYY-Q季度'
  if (picker === 'year') return 'YYYY'
  if (showTime) return 'YYYY-MM-DD HH:mm:ss'
  return 'YYYY-MM-DD'
}

export function getDateString(
  dayjsObj: dayjs.Dayjs,
  picker?: string,
  showTime?: boolean
) {
  if (!dayjsObj) return ''
  return dayjsObj.format(getDateFormat(picker, showTime))
}

export function getTimeString(
  dayjsObj: dayjs.Dayjs,
  use12Hours?: boolean,
  showSeconds?: boolean
) {
  if (!dayjsObj) return ''

  let format = showSeconds ? 'HH:mm:ss' : 'HH:mm'
  if (use12Hours) {
    format = showSeconds ? 'h:mm:ss a' : 'h:mm a'
  }
  return dayjsObj.format(format)
}
