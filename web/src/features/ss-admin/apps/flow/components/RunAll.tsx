import React, { useRef, useState } from 'react'
import { ReactFlowNode, RunVariable } from '@/dto'
import { NodeType, VariableDataType } from '@/enums'
import { getSeconds, mdToHtml } from '@/utils'
import {
  CloudUploadOutlined,
  CopyOutlined,
  DislikeOutlined,
  LikeOutlined,
  OpenAIFilled,
  PaperClipOutlined,
  ReloadOutlined,
  UserOutlined,
} from '@ant-design/icons'
import {
  Attachments,
  type AttachmentsProps,
  Bubble,
  Sender,
  Suggestion,
  useXAgent,
  XRequest,
} from '@ant-design/x'
import type { BubbleDataType } from '@ant-design/x/es/bubble/BubbleList'
import type { RolesType } from '@ant-design/x/es/bubble/BubbleList'
import { Button, GetProp, GetRef, Space, Spin } from 'antd'
import { createStyles } from 'antd-style'
import IconChat from '@/assets/agents/chat.svg?react'
import flowApi from '@/api/admin/apps/flowApi'
import '@/components/chat/chat.css'
import { useStore } from '../store'

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

const MOCK_SUGGESTIONS = [
  { label: 'Write a report', value: 'report' },
  { label: 'Draw a picture', value: 'draw' },
  {
    label: 'Check some knowledge',
    value: 'knowledge',
    icon: <OpenAIFilled />,
    children: [
      { label: 'About React', value: 'react' },
      { label: 'About Ant Design', value: 'antd' },
    ],
  },
]

const useCopilotStyle = createStyles(({ token, css }) => {
  return {
    copilotWrapper: css`
      height: calc(100vh - 137px);
      display: flex;
    `,
    copilotChat: css`
      display: flex;
      flex-direction: column;
      background: ${token.colorBgContainer};
      color: ${token.colorText};
    `,
    // chatList 样式
    chatList: css`
      overflow: auto;
      padding-block: 16px;
      flex: 1;
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
    // chatSend 样式
    chatSend: css`
      padding: 12px;
    `,
    speechButton: css`
      font-size: 18px;
      color: ${token.colorText} !important;
    `,
    placeholder: css`
      padding-top: calc(50vh - 100px);
    `,
  }
})

const RunAll: React.FC<{ nodes: ReactFlowNode[] }> = ({ nodes }) => {
  const store = useStore()

  const [loading, setLoading] = useState(false)
  const [messages, setMessages] = useState<Message[]>([])
  const { styles } = useCopilotStyle()
  const attachmentsRef = useRef<GetRef<typeof Attachments>>(null)
  const abortController = useRef<AbortController>(null)

  // ==================== State ====================

  const [attachmentsOpen, setAttachmentsOpen] = useState(false)
  const [files, setFiles] = useState<GetProp<AttachmentsProps, 'items'>>([])

  const [inputValue, setInputValue] = useState('')

  let previousNodeId = ''
  let currentNodeId = ''
  let inVariablesDict: Record<string, RunVariable[]> = {}
  let outVariablesDict: Record<string, RunVariable[]> = {}

  const apiRunNode = async (nodeId: string, inVariables: RunVariable[]) => {
    inVariablesDict[nodeId] = inVariables
    const startTime = new Date()
    store.onRunAll(previousNodeId, nodeId, true)
    const res = await flowApi.runNode({
      siteId: store.siteId,
      nodeId,
      inVariablesDict,
      outVariablesDict,
    })
    if (res) {
      previousNodeId = nodeId
      outVariablesDict[nodeId] = res.outVariables
      store.onRunAll(
        previousNodeId,
        nodeId,
        true,
        inVariables,
        res.outVariables,
        getSeconds(startTime)
      )
      if (res.isOutput) {
        await apiRunOutput(res.nextNodeId, res.outVariables)
      } else {
        await apiRunNode(res.nextNodeId, res.outVariables)
      }
    } else {
      store.onRunAll(
        previousNodeId,
        nodeId,
        false,
        inVariables,
        [
          {
            name: 'output',
            dataType: VariableDataType.String,
            value: messages[messages.length - 1].content,
          },
        ],
        getSeconds(startTime),
        '执行失败'
      )
    }
  }

  const apiRunOutput = async (nodeId: string, inVariables: RunVariable[]) => {
    inVariablesDict[nodeId] = inVariables
    store.onRunAll(previousNodeId, nodeId, true)

    currentNodeId = nodeId

    postAndShowMessage(inputValue)
  }

  const { create } = XRequest({
    baseURL: flowApi.getRunOutputUrl(),
  })

  const [agent] = useXAgent<BubbleDataType>({
    request: async (info, callbacks) => {
      try {
        create(
          {
            siteId: store.siteId,
            nodeId: info.nodeId,
            inVariablesDict: info.inVariablesDict,
            outVariablesDict: info.outVariablesDict,
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

  const postAndShowMessage = async (nextContent: string) => {
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
    let reasoning_content = ''
    let content = ''
    const startTime = new Date()
    agent.request(
      {
        nodeId: currentNodeId,
        inVariablesDict,
        outVariablesDict,
      },
      {
        onUpdate: (chunk: Partial<SSEFields>) => {
          if (!chunk.data) return
          if (chunk.data === ' [DONE]') {
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
          store.onRunAll(
            previousNodeId,
            currentNodeId,
            true,
            inVariablesDict[currentNodeId],
            [
              {
                name: 'output',
                dataType: VariableDataType.String,
                value: reasoning_content + content,
              },
            ],
            getSeconds(startTime)
          )
        },
        onError: (error) => {
          setLoading(false)
          // eslint-disable-next-line no-console
          console.error(error)
        },
      }
    )
  }

  // ==================== Event ====================
  const handleUserSubmit = async (val: string) => {
    if (!val) return

    setLoading(true)
    inVariablesDict = {}
    outVariablesDict = {}
    const startNodeId = nodes.find((node) => node.type === NodeType.Start)?.id
    if (startNodeId) {
      await apiRunNode(startNodeId, [
        {
          name: 'input',
          value: val,
        },
      ])
    }
  }

  const onPasteFile = (_: File, files: FileList) => {
    for (const file of files) {
      attachmentsRef.current?.upload(file)
    }
    setAttachmentsOpen(true)
  }

  // ==================== Nodes ====================

  const items = messages.map(
    ({ id, message, reasoning_content, content, status }) => ({
      key: id,
      loading: status === 'loading',
      role: status === 'local' ? 'local' : 'ai',
      content:
        status === 'ai' ? (
          <div className='markdown-body'>
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
              className='content'
              dangerouslySetInnerHTML={{
                __html: mdToHtml(content),
              }}
            />
          </div>
        ) : (
          <div>{message}</div>
        ),
    })
  )

  return (
    <div className={styles.copilotChat}>
      <div className={styles.copilotWrapper}>
        <div className={styles.chatList}>
          {messages?.length > 0 && (
            <Bubble.List
              style={{ height: '100%', paddingInline: 16 }}
              items={items}
              roles={roles}
            />
          )}
        </div>
      </div>

      <div className={styles.chatSend}>
        <Suggestion
          items={MOCK_SUGGESTIONS}
          onSelect={(itemVal) => setInputValue(`[${itemVal}]:`)}
        >
          {({ onTrigger, onKeyDown }) => (
            <Sender
              loading={loading}
              value={inputValue}
              onChange={(v) => {
                onTrigger(v === '/')
                setInputValue(v)
              }}
              onSubmit={() => {
                handleUserSubmit(inputValue)
                setInputValue('')
              }}
              onCancel={() => {
                abortController.current?.abort()
              }}
              allowSpeech
              placeholder='有问题，尽管问，SHIFT+ENTER 可换行'
              onKeyDown={onKeyDown}
              header={
                <Sender.Header
                  title='Upload File'
                  styles={{ content: { padding: 0 } }}
                  open={attachmentsOpen}
                  onOpenChange={setAttachmentsOpen}
                  forceRender
                >
                  <Attachments
                    ref={attachmentsRef}
                    beforeUpload={() => false}
                    items={files}
                    onChange={({ fileList }) => setFiles(fileList)}
                    placeholder={(type) =>
                      type === 'drop'
                        ? { title: 'Drop file here' }
                        : {
                            icon: <CloudUploadOutlined />,
                            title: 'Upload files',
                            description:
                              'Click or drag files to this area to upload',
                          }
                    }
                  />
                </Sender.Header>
              }
              prefix={
                <Button
                  type='text'
                  icon={<PaperClipOutlined style={{ fontSize: 18 }} />}
                  onClick={() => setAttachmentsOpen(!attachmentsOpen)}
                />
              }
              onPasteFile={onPasteFile}
              actions={(_, info) => {
                const { SendButton, LoadingButton, SpeechButton } =
                  info.components
                return (
                  <div
                    style={{ display: 'flex', alignItems: 'center', gap: 4 }}
                  >
                    <SpeechButton className={styles.speechButton} />
                    {loading ? (
                      <LoadingButton type='default' />
                    ) : (
                      <SendButton type='primary' />
                    )}
                  </div>
                )
              }}
            />
          )}
        </Suggestion>
      </div>
    </div>
  )
}

export default RunAll
