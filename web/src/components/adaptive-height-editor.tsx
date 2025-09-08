import { useState, useRef, useEffect } from 'react'
import { Editor, loader } from '@monaco-editor/react'
import * as monaco from 'monaco-editor'
import editorWorker from 'monaco-editor/esm/vs/editor/editor.worker?worker'
import cssWorker from 'monaco-editor/esm/vs/language/css/css.worker?worker'
import htmlWorker from 'monaco-editor/esm/vs/language/html/html.worker?worker'
import jsonWorker from 'monaco-editor/esm/vs/language/json/json.worker?worker'
import tsWorker from 'monaco-editor/esm/vs/language/typescript/ts.worker?worker'

// 配置monaco不从CDN加载
loader.config({
  paths: {
    vs: '/assets/monaco-editor/min/vs',
  },
})

const AdaptiveHeightEditor: React.FC<{
  language: string
  value?: string
  height?: string
  onChange: (value: string) => void
}> = ({ language, value, height, onChange }) => {
  const [editorHeight, setEditorHeight] = useState(height || '100px') // 初始高度
  const editorRef = useRef<monaco.editor.IStandaloneCodeEditor | null>(null)

  useEffect(() => {
    self.MonacoEnvironment = {
      getWorker() {
        if (language === 'json') {
          return new jsonWorker()
        }
        if (language === 'css' || language === 'scss' || language === 'less') {
          return new cssWorker()
        }
        if (
          language === 'html' ||
          language === 'handlebars' ||
          language === 'razor'
        ) {
          return new htmlWorker()
        }
        if (language === 'typescript' || language === 'javascript') {
          return new tsWorker()
        }
        return new editorWorker()
      },
    }

    loader.config({ monaco })
  }, [language])

  // 初始化编辑器
  function handleEditorDidMount(editor: monaco.editor.IStandaloneCodeEditor) {
    editorRef.current = editor

    // 监听内容变化
    editor.onDidContentSizeChange(() => {
      let contentHeight = editor.getContentHeight() // 获取内容高度
      if (contentHeight > 500) {
        contentHeight = 500
      }
      if (contentHeight > 100) {
        setEditorHeight(`${contentHeight}px`) // 动态设置高度
      }
    })
  }

  return (
    <div>
      <Editor
        height={editorHeight} // 动态高度
        defaultLanguage={language}
        theme='vs-dark'
        value={value}
        options={{
          wordWrap: 'on', // 开启自动换行
          scrollBeyondLastLine: false, // 禁止滚动到最后一行之后
        }}
        onMount={handleEditorDidMount}
        onChange={(value) => onChange(value ?? '')}
      />
    </div>
  )
}

export default AdaptiveHeightEditor
