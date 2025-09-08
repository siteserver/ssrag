import { SiteType } from '@/enums'
import IconAgent from '@/assets/sites/agent.svg?react'
import IconChat from '@/assets/sites/chat.svg?react'
import IconChatflow from '@/assets/sites/chatflow.svg?react'
import IconDocument from '@/assets/sites/document.svg?react'
import IconMarkdown from '@/assets/sites/markdown.svg?react'
import IconWeb from '@/assets/sites/web.svg?react'

const SiteIcon = ({
  type,
  className,
  style,
}: {
  type: SiteType
  className?: string
  style?: React.CSSProperties
}) => {
  if (type === SiteType.Chat) {
    return <IconChat className={className} style={style} />
  } else if (type === SiteType.Web) {
    return <IconWeb className={className} style={style} />
  } else if (type === SiteType.Markdown) {
    return <IconMarkdown className={className} style={style} />
  } else if (type === SiteType.Document) {
    return <IconDocument className={className} style={style} />
  } else if (type === SiteType.Chatflow) {
    return <IconChatflow className={className} style={style} />
  } else if (type === SiteType.Agent) {
    return <IconAgent className={className} style={style} />
  }
}

export default SiteIcon
