import React, { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { settingsDefaults } from '@/config'
import { DisplayType, SiteType } from '@/enums'
import { Prompt } from '@/models'
import { mdToHtml } from '@/utils'
import {
  CopyOutlined,
  DislikeOutlined,
  LikeOutlined,
  ReloadOutlined,
  UserOutlined,
} from '@ant-design/icons'
import {
  Bubble,
  Prompts,
  PromptProps,
  Welcome,
  useXAgent,
  XRequest,
} from '@ant-design/x'
import type { BubbleDataType } from '@ant-design/x/es/bubble/BubbleList'
import type { RolesType } from '@ant-design/x/es/bubble/BubbleList'
import { Button, Skeleton, Space, Spin } from 'antd'
import { createStyles } from 'antd-style'
import IconChat from '@/assets/agents/chat.svg?react'
import '@/assets/github-markdown.css'
import chatApi from '@/api/open/chatApi'
import { useAiStore } from '@/stores/aiStore'
import { useChatStore } from '@/stores/chatStore'
import SiteIcon from '../SiteIcon'
import AiSender from './AiSender'
import './chat.css'

const useStyle = createStyles(({ token, css }) => {
  return {
    page: css`
      padding: 20px 10px;
      padding-bottom: 10px;
      width: 100%;
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
      gap: 8px;
    `,
    chatPrompt: css`
      .ant-prompts-label {
        color: #000000e0 !important;
      }
      .ant-prompts-desc {
        color: #000000a6 !important;
        width: 100%;
      }
      .ant-prompts-icon {
        color: #000000a6 !important;
      }
    `,
    messages: css`
      flex: 1;
    `,
    placeholder: css`
      flex: 1;
    `,
    senderPrompt: css`
      width: 100%;
      max-width: 700px;
      margin: 0 auto;
      color: ${token.colorText};
    `,
    sender: css`
      width: 100%;
      max-width: 700px;
      margin: 0 auto;
      box-shadow: ${token.boxShadow};
    `,
    logo: css`
      display: flex;
      height: 72px;
      align-items: center;
      justify-content: start;
      padding: 0 10px;
      box-sizing: border-box;

      img {
        width: 24px;
        height: 24px;
        display: inline-block;
      }

      span {
        display: inline-block;
        margin: 0 8px;
        font-weight: bold;
        color: ${token.colorText};
        font-size: 16px;
        line-height: 28px;
      }
    `,
    addBtn: css`
      background: #1677ff0f;
      border: 1px solid #1677ff34;
      width: calc(100% - 24px);
      margin: 0 12px 24px 12px;
    `,
    footer: css`
      text-align: center;
      color: ${token.colorTextSecondary};
      font-size: 12px;
    `,
  }
})

interface SSEFields {
  data: string
}

const roles: RolesType = {
  ai: {
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
  local: {
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

interface Message {
  id: string
  status: 'local' | 'ai' | 'loading'
  message: string
  reasoning_content: string
  content: string
}

const ChatApp: React.FC<{
  id: string | null
  siteId: number | null
  sessionId: string | null
  displayType: DisplayType
}> = ({ id, siteId, sessionId, displayType }) => {
  // ==================== Style ====================
  const { styles } = useStyle()
  const aiStore = useAiStore()
  const chatStore = useChatStore()

  // ==================== State ====================
  const [loading, setLoading] = useState(false)
  const [messages, setMessages] = useState<Message[]>([])
  const [hotPrompts, setHotPrompts] = useState<Prompt[]>([])
  const [functionPrompts, setFunctionPrompts] = useState<Prompt[]>([])
  const [inputPrompts, setInputPrompts] = useState<PromptProps[]>([])

  const { isPending } = useQuery({
    queryKey: ['chat', id, siteId, sessionId],
    queryFn: async () => {
      const res = await chatApi.get({
        id,
        siteId,
        sessionId,
      })
      if (res) {
        chatStore.init(res.sessionId, res.site, res.values)

        if (res.messages.length > 0) {
          setMessages(
            res.messages.map((message) => ({
              id: message.id.toString(),
              status: message.role === 'user' ? 'local' : 'ai',
              message: message.content,
              reasoning_content: message.reasoning,
              content: message.content,
            }))
          )
        } else {
          setMessages([])
        }

        // const chatPresetQuestions = split(res.values.chatPresetQuestions, '\n')

        if (res.values.isHotPrompts) {
          setHotPrompts(res.hotPrompts)
        }
        if (res.values.isFunctionPrompts) {
          setFunctionPrompts(res.functionPrompts)
        }
        if (res.values.isInputPrompts) {
          const inputPrompts = res.inputPrompts.map((prompt: Prompt) => ({
            key: prompt.uuid,
            description: prompt.text,
            icon: prompt.iconUrl,
          }))
          setInputPrompts(inputPrompts)
        }
      }
      return res
    },
    staleTime: 0,
    refetchOnMount: false,
    refetchOnWindowFocus: false,
    refetchOnReconnect: false,
  })

  // ==================== Runtime ====================
  const { create } = XRequest({
    baseURL: chatApi.getUrl(),
  })

  const [agent] = useXAgent<BubbleDataType>({
    request: async (info, callbacks) => {
      try {
        create(
          {
            siteId: info.siteId,
            messageType: info.messageType,
            providerModelId: info.providerModelId,
            thinking: info.thinking,
            searching: info.searching,
            message: info.message,
          },
          {
            onSuccess: () => {
              callbacks.onSuccess([])
            },
            onError: (error) => {
              callbacks.onError(error)
            },
            onUpdate: (chunk: Partial<SSEFields>) => {
              callbacks.onUpdate(chunk)
            },
          }
        )
      } catch (error) {
        callbacks.onError(error as Error)
      }
    },
  })

  const handleHistory = async (
    message: string,
    reasoning: string,
    content: string
  ) => {
    const res = await chatApi.history({
      siteId: chatStore.site?.id,
      sessionId: chatStore.sessionId,
      thinking: chatStore.aiParameters.thinking,
      searching: chatStore.aiParameters.searching,
      message: message,
      reasoning: reasoning,
      content: content,
    })
    if (res) {
      aiStore.addChatGroup(res.chatGroup)
    }
  }

  // ==================== Event ====================
  const handleSubmit = (nextContent: string) => {
    if (!nextContent) return
    setMessages([
      ...messages,
      {
        id: `round-${messages.length}-local`,
        status: 'local',
        message: nextContent,
        reasoning_content: '',
        content: '',
      },
      {
        id: `round-${messages.length}-ai`,
        status: 'loading',
        message: '',
        reasoning_content: '',
        content: '',
      },
    ])
    setLoading(true)
    let reasoning_content = ''
    let content = ''
    agent.request(
      {
        siteId: chatStore.site?.id,
        thinking: chatStore.aiParameters.thinking,
        searching: chatStore.aiParameters.searching,
        message: nextContent,
      } as BubbleDataType,
      {
        onUpdate: (chunk: Partial<SSEFields>) => {
          if (!chunk.data) return
          if (chunk.data === ' [DONE]') {
            handleHistory(nextContent, reasoning_content, content)
            return
          }
          const data = JSON.parse(chunk.data)
          const delta = data?.choices[0].delta

          if (delta.reasoning_content || delta.content) {
            if (delta.reasoning_content) {
              reasoning_content += delta.reasoning_content
            }
            if (delta.content) {
              content += delta.content
            }
          } else {
            return
          }
          setMessages([
            ...messages,
            {
              id: `round-${messages.length}-local`,
              status: 'local',
              message: nextContent,
              reasoning_content: '',
              content: '',
            },
            {
              id: `round-${messages.length}-ai`,
              status: 'ai',
              content,
              reasoning_content,
              message: '',
            },
          ])
        },
        onSuccess: () => {
          setLoading(false)
          chatStore.setAiLoading(false)
        },
        onError: (error) => {
          setLoading(false)
          // eslint-disable-next-line no-console
          console.error(error)
        },
      }
    )
  }

  const icon = chatStore.site?.iconUrl ? (
    <img src={chatStore.site.iconUrl} alt='icon' />
  ) : (
    <SiteIcon
      type={chatStore.site?.siteType as SiteType}
      style={{ width: 65, height: 65 }}
    />
  )

  // ==================== Nodes ====================
  const items = messages.map(
    ({ id, message, reasoning_content, content, status }) => ({
      key: id,
      loading: status === 'loading',
      role: status === 'local' ? 'local' : 'ai',
      content:
        status === 'ai' ? (
          <div className='markdown-body' style={{ background: 'transparent' }}>
            {reasoning_content && (
              <div className='reasoner'>
                <blockquote>
                  <div className='reasoner-title'>已深度思考</div>
                  <div
                    dangerouslySetInnerHTML={{
                      __html: mdToHtml(reasoning_content),
                    }}
                  />
                </blockquote>
              </div>
            )}
            <div
              dangerouslySetInnerHTML={{
                __html: mdToHtml(content),
              }}
            />
          </div>
        ) : (
          <div
            dangerouslySetInnerHTML={{
              __html: mdToHtml(message),
            }}
          />
        ),
    })
  )

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  // ==================== Render =================
  return (
    <div
      className={styles.page}
      style={{
        height:
          displayType === DisplayType.Copilot ? 'calc(100vh - 52px)' : '100vh',
      }}
    >
      <div className={styles.layout}>
        <div className={styles.chat}>
          {items.length > 0 ? (
            <Bubble.List
              items={items}
              roles={roles}
              className={styles.messages}
            />
          ) : (
            <Space
              direction='vertical'
              size={16}
              style={{
                paddingInline: 'calc(calc(100% - 700px) /2)',
              }}
              className={'welcome' + ' ' + styles.placeholder}
            >
              <Welcome
                style={{
                  marginTop:
                    chatStore.values?.welcomePosition == 'top'
                      ? '0'
                      : 'calc(50% - 124px)',
                  backgroundImage:
                    chatStore.values?.welcomeVariant == 'filled'
                      ? 'linear-gradient(97deg, #f2f9fe 0%, #f7f3ff 100%)'
                      : 'none',
                }}
                icon={icon}
                title={
                  chatStore.values?.welcomeTitle ||
                  settingsDefaults.welcomeTitle
                }
                description={
                  chatStore.site?.description || settingsDefaults.description
                }
                variant={
                  chatStore.values?.welcomeVariant ||
                  settingsDefaults.welcomeVariant
                }
              />

              {chatStore.values?.isHotPrompts && (
                <Prompts
                  items={[
                    {
                      key: 'hotPrompts',
                      label: chatStore.values?.hotPromptsTitle || '',
                      children: hotPrompts.map(
                        (prompt: Prompt, index: number) => {
                          let color = '#00000040'
                          if (index === 0) {
                            color = '#f93a4a'
                          } else if (index === 1) {
                            color = '#ff6565'
                          } else if (index === 2) {
                            color = '#ff8f1f'
                          }
                          return {
                            key: prompt.uuid,
                            description: prompt.text,
                            icon: (
                              <span style={{ color: color, fontWeight: 700 }}>
                                {index + 1}
                              </span>
                            ),
                          }
                        }
                      ),
                    },
                  ]}
                  styles={{
                    list: { height: '100%' },
                    item: {
                      flex: 1,
                      backgroundImage:
                        'linear-gradient(123deg, #e5f4ff 0%, #efe7ff 100%)',
                      borderRadius: 12,
                      border: 'none',
                    },
                    subItem: { padding: 0, background: 'transparent' },
                  }}
                  onItemClick={(info) => {
                    handleSubmit(info.data.description as string)
                  }}
                  className={styles.chatPrompt}
                />
              )}

              {chatStore.values?.isFunctionPrompts && (
                <Prompts
                  items={[
                    {
                      key: 'functionPrompts',
                      label: chatStore.values?.functionPromptsTitle || '',
                      children: functionPrompts.map((prompt: Prompt) => ({
                        key: prompt.uuid,
                        icon: (
                          <img
                            src={prompt.iconUrl}
                            alt='product'
                            className='mr-2 h-12 w-12'
                          />
                        ),
                        label: <div className='font-bold'>{prompt.title}</div>,
                        description: prompt.text,
                      })),
                    },
                  ]}
                  styles={{
                    item: {
                      flex: 1,
                      backgroundImage:
                        'linear-gradient(123deg, #e5f4ff 0%, #efe7ff 100%)',
                      borderRadius: 12,
                      border: 'none',
                    },
                    subItem: { background: '#ffffffa6' },
                  }}
                  onItemClick={(info) => {
                    handleSubmit(info.data.description as string)
                  }}
                  className={styles.chatPrompt}
                />
              )}
            </Space>
          )}

          <Prompts
            items={inputPrompts}
            styles={{
              item: { padding: '6px 12px' },
            }}
            wrap
            onItemClick={(info) => {
              handleSubmit(info.data.description as string)
            }}
            className={styles.senderPrompt}
          />

          <AiSender
            className={styles.sender}
            loading={loading}
            onSubmit={handleSubmit}
          />
          <div className={styles.footer}>
            {chatStore.values?.footerText || settingsDefaults.footerText}
          </div>
        </div>
      </div>
    </div>
  )
}

export default ChatApp
