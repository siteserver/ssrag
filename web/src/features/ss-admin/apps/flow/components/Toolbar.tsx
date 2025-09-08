import {
  PlayCircleOutlined,
  SaveOutlined,
  SendOutlined,
  FullscreenOutlined,
  FullscreenExitOutlined,
  PartitionOutlined,
} from '@ant-design/icons'
import { Divider, Button, Card } from 'antd'

interface VueInstance {
  isFullscreen: boolean
}

const Toolbar: React.FC<{
  fullscreen: boolean
  onFullscreen: () => void
  onFix: () => void
  onSave: () => void
  onRun: () => void
  onPublish: () => void
}> = ({ fullscreen, onFullscreen, onFix, onSave, onRun, onPublish }) => {
  return (
    <Card
      title={null}
      variant='borderless'
      styles={{
        body: {
          padding: 5,
        },
      }}
    >
      <Button
        size='middle'
        icon={fullscreen ? <FullscreenExitOutlined /> : <FullscreenOutlined />}
        type={fullscreen ? 'primary' : 'default'}
        onClick={(e) => {
          e.stopPropagation()
          onFullscreen()
          const topVue = window.top
            ? (window.top as { $vue?: VueInstance })['$vue']
            : null
          if (topVue) {
            topVue.isFullscreen = !fullscreen
          }
        }}
      />

      <Divider style={{ borderColor: '#999' }} type='vertical' />

      <Button
        color='primary'
        style={{ background: '#52c41a' }}
        variant='solid'
        size='middle'
        onClick={onRun}
      >
        <PlayCircleOutlined />
        试运行
      </Button>
      <Divider style={{ borderColor: '#999' }} type='vertical' />

      <Button
        color='default'
        variant='filled'
        size='middle'
        style={{ marginRight: 5 }}
        onClick={onFix}
      >
        <PartitionOutlined />
        对齐
      </Button>
      <Button
        color='default'
        variant='filled'
        size='middle'
        style={{ marginRight: 5 }}
        onClick={onSave}
      >
        <SaveOutlined />
        保存
      </Button>
      <Button
        color='default'
        variant='filled'
        size='middle'
        onClick={onPublish}
      >
        <SendOutlined />
        发布
      </Button>
    </Card>
  )
}

export default Toolbar
