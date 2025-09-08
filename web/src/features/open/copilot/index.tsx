import { useQuery } from '@tanstack/react-query'
import { DisplayType } from '@/enums'
import { getQueryInt, getQueryString } from '@/utils'
import { CloseOutlined } from '@ant-design/icons'
import { Button, Space, Skeleton } from 'antd'
import { createStyles } from 'antd-style'
import copilotApi from '@/api/open/copilotApi'
import { useAiStore } from '@/stores/aiStore'
import { useChatStore } from '@/stores/chatStore'
import ChatApp from '@/components/chat/ChatApp'

const siteId = getQueryInt('siteId')
const id = getQueryString('id')

const useCopilotStyle = createStyles(({ token, css }) => {
  return {
    copilotChat: css`
      width: 100%;
      display: flex;
      flex-direction: column;
      background: ${token.colorBgContainer};
      color: ${token.colorText};
    `,
    // chatHeader 样式
    chatHeader: css`
      height: 52px;
      box-sizing: border-box;
      border-bottom: 1px solid ${token.colorBorder};
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 10px 0 16px;
    `,
    headerTitle: css`
      font-weight: 600;
      font-size: 15px;
    `,
    headerButton: css`
      font-size: 18px;
    `,
    conversations: css`
      width: 300px;
      .ant-conversations-list {
        padding-inline-start: 0;
      }
    `,
  }
})

const Copilot: React.FC<{
  sessionId: string | null
}> = ({ sessionId }) => {
  const aiStore = useAiStore()
  const chatStore = useChatStore()

  const { styles } = useCopilotStyle()
  // ==================== State ====================

  const { isPending } = useQuery({
    queryKey: ['open', 'copilot', siteId, id, sessionId],
    queryFn: async () => {
      const res = await copilotApi.get({ siteId, id, sessionId })
      if (res) {
        aiStore.init(res.sites, res.chatGroups)
        chatStore.init(res.sessionId, res.site, res.values)
      }
      return res
    },
    // 强制刷新，每次显示时都重新获取数据
    refetchOnMount: false,
    refetchOnWindowFocus: false,
    refetchOnReconnect: false,
  })

  // ==================== Nodes ====================
  const chatHeader = (
    <div className={styles.chatHeader}>
      <div className={styles.headerTitle}>{chatStore.values?.headerText}</div>
      <Space size={0}>
        {/* <Button
          type='text'
          icon={<PlusOutlined />}
          onClick={() => {
            if (messages?.length) {
              const timeNow = dayjs().valueOf().toString()
              abortController.current?.abort()
              // The abort execution will trigger an asynchronous requestFallback, which may lead to timing issues.
              // In future versions, the sessionId capability will be added to resolve this problem.
              setTimeout(() => {
                setSessionList([
                  { key: timeNow, label: 'New session', group: 'Today' },
                  ...sessionList,
                ])
                setCurSession(timeNow)
                setMessages([])
              }, 100)
            } else {
              message.error('It is now a new conversation.')
            }
          }}
          className={styles.headerButton}
        /> */}
        {/* <Popover
          placement='bottom'
          styles={{ body: { padding: 0, maxHeight: 600 } }}
          content={
            <Conversations
              items={sessionList?.map((i) =>
                i.key === curSession
                  ? { ...i, label: `[current] ${i.label}` }
                  : i
              )}
              activeKey={curSession}
              groupable
              onActiveChange={async (val) => {
                abortController.current?.abort()
                // The abort execution will trigger an asynchronous requestFallback, which may lead to timing issues.
                // In future versions, the sessionId capability will be added to resolve this problem.
                setTimeout(() => {
                  setCurSession(val)
                  // setMessages(
                  //   (messageHistory?.[val] as MessageInfo<BubbleDataType>[]) ||
                  //     []
                  // )
                }, 100)
              }}
              styles={{ item: { padding: '0 8px' } }}
              className={styles.conversations}
            />
          }
        >
          <Button
            type='text'
            icon={<CommentOutlined />}
            className={styles.headerButton}
          />
        </Popover> */}
        {window.parent !== window && (
          <Button
            type='text'
            icon={<CloseOutlined />}
            onClick={() => {
              // 当前页面为iframe嵌入，关闭当前iframe页面
              window.parent.postMessage({ type: 'closeChatbot' }, '*')
            }}
            className={styles.headerButton}
          />
        )}
      </Space>
    </div>
  )

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <div className={styles.copilotChat}>
      {/** 对话区 - header */}
      {chatHeader}

      <ChatApp
        id={id}
        siteId={siteId}
        sessionId={sessionId}
        displayType={DisplayType.Copilot}
      />
    </div>
  )
}

export default Copilot
