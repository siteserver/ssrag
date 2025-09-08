import { Schema } from '@/dto'
import { Edge } from '@atlaskit/pragmatic-drag-and-drop-hitbox/closest-edge'

declare global {
  interface Window {
    draggingKeys: string[] | null
    closestEdge: Edge | null
    targetKey: string | null
    widget: Schema | null
    iframe: HTMLIFrameElement | null
    utils: {
      addTab: (title: string, url: string) => void
      getSettingsUrl: (name: string, params: Record<string, unknown>) => string
    }
    closeChatbot?: () => void
  }
}
