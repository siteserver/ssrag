export function decodeHTMLEntities(encodedStr: string) {
  const parser = new DOMParser()
  const doc = parser.parseFromString(encodedStr, 'text/html')
  return doc.documentElement.textContent
}

export function textToHtml(text: string) {
  return (text || '').replace(/\n/g, '<br>').replace(/ /g, '&nbsp;')
}
