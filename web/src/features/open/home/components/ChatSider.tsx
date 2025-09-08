import React, { useMemo, useRef, useState } from 'react'
import { SiteSummary } from '@/models'
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons'
import { Conversations } from '@ant-design/x'
import { Button, MenuProps, Menu, Divider, Modal, Input } from 'antd'
import { createStyles } from 'antd-style'
import homeApi from '@/api/open/homeApi'
import { useAiStore } from '@/stores/aiStore'
import { useChatStore } from '@/stores/chatStore'
import { uuid } from '@/utils/strings'
import SiteIcon from '@/components/SiteIcon'

const useStyle = createStyles(({ token, css }) => {
  return {
    layout: css`
      width: 100%;
      min-width: 1000px;
      height: 100vh;
      display: flex;
      background: ${token.colorBgContainer};
      font-family: AlibabaPuHuiTi, ${token.fontFamily}, sans-serif;
    `,
    // sider 样式
    sider: css`
      background: ${token.colorBgLayout}80;
      width: 280px;
      height: 100%;
      display: flex;
      flex-direction: column;
      padding: 0 12px;
      box-sizing: border-box;
    `,
    logo: css`
      display: flex;
      align-items: center;
      justify-content: start;
      padding: 0 16px;
      box-sizing: border-box;
      gap: 8px;
      margin: 16px 0;

      span {
        font-weight: bold;
        color: ${token.colorText};
        font-size: 16px;
      }
    `,
    addBtn: css`
      background: #1677ff0f;
      border: 1px solid #1677ff34;
      height: 36px;
    `,
    conversations: css`
      flex: 1;
      overflow-y: auto;
      margin-top: 12px;
      padding: 0;

      .ant-conversations-list {
        padding-inline-start: 0;
      }
    `,
    siderFooter: css`
      border-top: 1px solid ${token.colorBorderSecondary};
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: space-between;
    `,
    // chat list 样式
    chat: css`
      height: 100%;
      width: 100%;
      box-sizing: border-box;
      display: flex;
      flex-direction: column;
      padding-block: ${token.paddingLG}px;
      gap: 16px;
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
    chatList: css`
      flex: 1;
      overflow: auto;
    `,
    loadingMessage: css`
      background-image: linear-gradient(
        90deg,
        #ff6b23 0%,
        #af3cb8 31%,
        #53b6ff 89%
      );
      background-size: 100% 2px;
      background-repeat: no-repeat;
      background-position: bottom;
    `,
    placeholder: css`
      padding-top: 32px;
    `,
    // sender 样式
    sender: css`
      width: 100%;
      max-width: 700px;
      margin: 0 auto;
    `,
    speechButton: css`
      font-size: 18px;
      color: ${token.colorText} !important;
    `,
    senderPrompt: css`
      width: 100%;
      max-width: 700px;
      margin: 0 auto;
      color: ${token.colorText};
    `,
  }
})

const ChatSider: React.FC<{
  isSite: boolean
  selectedSession: string | null
  onAppSelect: (site: SiteSummary) => void
  onSessionSelect: (sessionId: string) => void
}> = ({ isSite, selectedSession, onAppSelect, onSessionSelect }) => {
  const aiStore = useAiStore()
  const chatStore = useChatStore()

  const { styles } = useStyle()
  const abortController = useRef<AbortController>(null)
  const [renameTitle, setRenameTitle] = useState<string | null>(null)

  const items = useMemo(() => {
    return aiStore.chatGroups.map((group) => {
      return {
        key: group.sessionId,
        label: group.title,
        siteId: group.siteId,
        group: '聊天记录',
      }
    })
  }, [aiStore.chatGroups])

  const [conversations, setConversations] = useState(items)
  const [curConversation, setCurConversation] = useState(selectedSession)

  const menuItems = aiStore.sites.map((site) => {
    const icon = site.iconUrl ? (
      <img
        src={site.iconUrl}
        className='h-4 w-4'
        alt={`${site.siteName} icon`}
      />
    ) : (
      <SiteIcon type={site.siteType} className='h-4 w-4' />
    )

    // const icon = site.iconUrl ? (
    //   <img
    //     src={site.iconUrl}
    //     className='h-4 w-4'
    //     alt={`${site.siteName} icon`}
    //   />
    // ) : (
    //   <IconChat className='h-5 w-5' />
    // )
    return {
      key: site.id,
      icon,
      label: site.siteName,
    }
  }) as MenuProps['items']

  return (
    <div className={styles.sider}>
      {/* 🌟 Logo */}
      <div className={styles.logo}>
        {isSite ? (
          <span>{chatStore.values.headerText}</span>
        ) : (
          <>
            <img
              src='/assets/images/logo.svg'
              draggable={false}
              alt='logo'
              width={24}
              height={24}
            />
            <span>应用对话</span>
          </>
        )}
      </div>

      {!isSite && (
        <>
          <Divider style={{ marginTop: 0, marginBottom: '5px' }} />
          <Menu
            mode='vertical'
            className='left-menus h-auto border-0 bg-transparent'
            selectedKeys={[chatStore.siteId + '']}
            items={menuItems}
            onClick={(item) =>
              onAppSelect(aiStore.sites.find((site) => site.id === +item.key)!)
            }
            onSelect={(item) => onSessionSelect(item.key)}
          />
        </>
      )}

      <Divider style={{ marginTop: '5px', marginBottom: '10px' }} />

      {/* 🌟 添加会话 */}
      <Button
        onClick={() => {
          const key = uuid()
          setConversations([
            {
              key,
              siteId: chatStore.siteId,
              label: '新对话',
              group: '聊天记录',
            },
            ...conversations,
          ])
          setCurConversation(key)
          onSessionSelect(key)
        }}
        type='link'
        className={styles.addBtn}
        icon={<PlusOutlined />}
      >
        新建会话
      </Button>

      {/* 🌟 会话管理 */}
      <Conversations
        items={conversations}
        className={styles.conversations}
        activeKey={curConversation || ''}
        onActiveChange={async (val) => {
          abortController.current?.abort()
          // The abort execution will trigger an asynchronous requestFallback, which may lead to timing issues.
          // In future versions, the sessionId capability will be added to resolve this problem.
          setTimeout(() => {
            setCurConversation(val)
            onSessionSelect(val)
          }, 100)
        }}
        groupable
        styles={{ item: { padding: '0 8px' } }}
        menu={(conversation) => ({
          items: [
            {
              label: '重命名',
              key: 'rename',
              icon: <EditOutlined />,
              onClick: () => {
                setRenameTitle(conversation.label as string)
              },
            },
            {
              label: '删除',
              key: 'delete',
              icon: <DeleteOutlined />,
              danger: true,
              onClick: () => {
                homeApi.delete({
                  siteId: conversation.siteId + '',
                  sessionId: conversation.key,
                })

                const newList = conversations.filter(
                  (item) => item.key !== conversation.key
                )
                const newKey = newList?.[0]?.key
                setConversations(newList)
                // The delete operation modifies curConversation and triggers onActiveChange, so it needs to be executed with a delay to ensure it overrides correctly at the end.
                // This feature will be fixed in a future version.
                setTimeout(() => {
                  if (conversation.key === curConversation) {
                    setCurConversation(newKey)
                    onSessionSelect(newKey)
                  }
                }, 200)
              },
            },
          ],
        })}
      />

      {/* <div className={styles.siderFooter}>
        <Avatar size={24} />
        <Button type='text' icon={<QuestionCircleOutlined />} />
      </div> */}

      <Modal
        open={renameTitle !== null}
        title='重命名会话'
        onCancel={() => {
          setRenameTitle(null)
        }}
        onOk={() => {
          if (renameTitle !== null) {
            const conversation = conversations.find(
              (item) => item.key === curConversation
            )
            if (conversation) {
              homeApi.rename({
                siteId: conversation.siteId + '',
                sessionId: conversation.key,
                title: renameTitle,
              })
              const newList = conversations.map((item) => {
                if (item.key === conversation.key) {
                  return { ...item, label: renameTitle }
                }
                return item
              })
              setConversations(newList)
            }
          }
          setRenameTitle(null)
        }}
      >
        <Input
          placeholder='请输入新标题'
          defaultValue={renameTitle || ''}
          onChange={(e) => setRenameTitle(e.target.value)}
        />
      </Modal>
    </div>
  )
}

export default ChatSider
