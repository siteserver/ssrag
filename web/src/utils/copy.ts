import { message } from 'antd'

export function copy(content: string) {
  navigator.clipboard.writeText(content).then(() => {
    message.open({
      type: 'success',
      content: '复制成功！',
    })
  })
}
