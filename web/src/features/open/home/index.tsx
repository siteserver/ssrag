import React, { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { DisplayType } from '@/enums'
import { SiteSummary } from '@/models'
import { getQueryInt, getQueryString } from '@/utils'
import { Skeleton } from 'antd'
import homeApi from '@/api/open/homeApi'
import { useAiStore } from '@/stores/aiStore'
import { useChatStore } from '@/stores/chatStore'
import ChatApp from '@/components/chat/ChatApp'
import './app.css'
import ChatSider from './components/ChatSider'

const siteId = getQueryInt('siteId')
const id = getQueryString('id')

const Home: React.FC<{ sessionId: string | null }> = ({ sessionId }) => {
  const aiStore = useAiStore()
  const chatStore = useChatStore()

  const [selectedApp, setSelectedApp] = useState<SiteSummary | null>(null)
  const [selectedSession, setSelectedSession] = useState<string | null>(
    sessionId
  )

  const { isPending } = useQuery({
    queryKey: ['open', 'home', siteId, id, sessionId],
    queryFn: async () => {
      const res = await homeApi.get({ siteId, id, sessionId })
      if (res) {
        aiStore.init(res.sites, res.chatGroups)
        if (res.site) {
          chatStore.init(res.sessionId, res.site, res.values)
        }
        if (res.sites.length > 0) {
          setSelectedApp(res.sites[0])
        }
        if (res.sessionId) {
          setSelectedSession(res.sessionId)
        }
      }
      return res
    },
  })

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <div className='flex h-screen'>
      <ChatSider
        isSite={siteId || id ? true : false}
        selectedSession={selectedSession}
        onAppSelect={setSelectedApp}
        onSessionSelect={setSelectedSession}
      />
      <div className='flex h-screen flex-1 flex-col items-center justify-center'>
        {selectedApp ? (
          <ChatApp
            id={null}
            siteId={selectedApp.id}
            sessionId={selectedSession}
            displayType={DisplayType.Home}
          />
        ) : (
          <div></div>
        )}
      </div>
    </div>
  )
}

export default Home
