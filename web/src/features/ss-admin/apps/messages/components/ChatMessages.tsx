import React, { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { ChatMessage } from '@/models'
import { mdToHtml } from '@/utils'
import {
  CopyOutlined,
  DislikeOutlined,
  LikeOutlined,
  ReloadOutlined,
  UserOutlined,
} from '@ant-design/icons'
import { Bubble } from '@ant-design/x'
import type { RolesType } from '@ant-design/x/es/bubble/BubbleList'
import { Button, Skeleton, Space, Spin } from 'antd'
import { createStyles } from 'antd-style'
import IconChat from '@/assets/agents/chat.svg?react'
import '@/assets/github-markdown.css'
import messagesApi from '@/api/admin/apps/messagesApi'

const useStyle = createStyles(({ token, css }) => {
  return {
    page: css`
      height: 100vh;
      min-width: 50%;
    `,
    layout: css`
      width: 100%;
      height: 100%;
      border-radius: ${token.borderRadius}px;
      display: flex;
      background: ${token.colorBgContainer};
      font-family: AlibabaPuHuiTi, ${token.fontFamily}, sans-serif;

      .ant-prompts {
        color: ${token.colorText};
      }
    `,
    chat: css`
      height: 100%;
      width: 100%;
      margin: 0 auto;
      box-sizing: border-box;
      display: flex;
      flex-direction: column;
      padding: ${token.paddingLG}px 0;
      gap: 16px;
    `,
    messages: css`
      padding-inline: calc(50% - 350px);
      flex: 1;
    `,
  }
})

const roles: RolesType = {
  assistant: {
    placement: 'start' as const,
    avatar: {
      icon: <IconChat />,
      style: {
        background: 'none',
        fontSize: 60,
      },
    },
    typing: {
      step: 5,
      interval: 20,
    },
    styles: {
      content: {
        borderRadius: 16,
        width: 'auto',
        fontSize: 16,
      },
    },
    footer: (
      <div style={{ display: 'flex' }}>
        <Button type='text' size='small' icon={<ReloadOutlined />} />
        <Button type='text' size='small' icon={<CopyOutlined />} />
        <Button type='text' size='small' icon={<LikeOutlined />} />
        <Button type='text' size='small' icon={<DislikeOutlined />} />
      </div>
    ),
    loadingRender: () => (
      <Space>
        <Spin size='small' />
        生成中，请稍等...
      </Space>
    ),
  },
  user: {
    placement: 'end' as const,
    avatar: {
      icon: <UserOutlined />,
      style: {
        background: '#1677ff',
      },
    },
    styles: {
      content: {
        borderRadius: 16,
        backgroundColor: '#eff6ff',
        width: 'auto',
        fontSize: 16,
      },
    },
    variant: 'shadow' as const,
  },
}

const ChatMessages: React.FC<{ siteId: number; sessionId: string }> = ({
  siteId,
  sessionId,
}) => {
  const { styles } = useStyle()
  const [messages, setMessages] = useState<ChatMessage[]>([])

  const { isPending } = useQuery({
    queryKey: ['ss-admin', 'apps', 'messages', siteId, sessionId],
    queryFn: async () => {
      const res = await messagesApi.messages({
        siteId,
        sessionId,
      })
      if (res) {
        setMessages(res.messages)
      }
      return res
    },
  })

  const items = messages.map((message) => ({
    key: message.id,
    loading: false,
    role: message.role,
    content:
      message.role === 'assistant' ? (
        <div className='markdown-body' style={{ background: 'transparent' }}>
          <div className='reasoner'>
            <blockquote>
              <div className='reasoner-title'>已深度思考</div>
              <div
                dangerouslySetInnerHTML={{
                  __html: mdToHtml(message.reasoning),
                }}
              />
            </blockquote>
          </div>
          <div
            dangerouslySetInnerHTML={{
              __html: mdToHtml(message.content),
            }}
          />
        </div>
      ) : (
        <div
          dangerouslySetInnerHTML={{
            __html: mdToHtml(message.content),
          }}
        />
      ),
  }))

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <div className={styles.page}>
      <div className={styles.layout}>
        <div className={styles.chat}>
          {items.length > 0 && (
            <Bubble.List
              items={items}
              roles={roles}
              className={styles.messages}
            />
          )}
        </div>
      </div>
    </div>
  )
}

export default ChatMessages
