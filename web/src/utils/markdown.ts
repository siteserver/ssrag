import hljs from 'highlight.js'
import { marked, type MarkedOptions } from 'marked'

const renderer = new marked.Renderer()
renderer.link = ({ href, title, text }) => {
  if (!href) return text // 处理无效链接
  const titleAttr = title ? `title="${title}"` : ''
  // 添加 target 和 rel 属性
  return `<a href="${href}" ${titleAttr} target="_blank" rel="noopener noreferrer">${text}</a>`
}

marked.use({ renderer })

marked.setOptions({
  highlight: function (code: string, language: string) {
    const validLanguage = hljs.getLanguage(language) ? language : 'plaintext'
    return hljs.highlight(validLanguage, code).value
  },
} as MarkedOptions)

export function parseText(text: string, siteUrl: string) {
  return text.replace(/src="@\/([^"]*)"/g, `src="${siteUrl}$1"`)
}

export function mdToHtml(md: string, siteUrl = ''): string {
  if (!md) return ''
  md = md.replace(/ssrag:\/\/doc-(\d+)/g, '/knowledge_base/document/$1')
  md = marked.parse(md) as string
  return siteUrl ? parseText(md, siteUrl) : md
}

export function extractMarkdownCode(text: string, codeType: string) {
  if (!text || !codeType) return text

  const codeBlockPattern = new RegExp(
    `\`\`\`${codeType}([\\s\\S]*?)\`\`\``,
    'i'
  )
  const match = text.match(codeBlockPattern)

  if (match && match[match.length - 1]) {
    return match[match.length - 1].trim()
  }

  return text
}
