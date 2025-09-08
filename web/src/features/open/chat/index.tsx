import { DisplayType } from '@/enums'
import { getQueryInt, getQueryString } from '@/utils'
import ChatApp from '@/components/chat/ChatApp'

const siteId = getQueryInt('siteId')
const id = getQueryString('id')

const Chat: React.FC<{ sessionId: string | null }> = ({ sessionId }) => {
  return (
    <ChatApp
      id={id}
      siteId={siteId}
      sessionId={sessionId}
      displayType={DisplayType.Chat}
    />
  )
}

export default Chat
