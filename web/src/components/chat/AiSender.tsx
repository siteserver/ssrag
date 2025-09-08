import React, { useEffect, useRef, useState } from 'react'
import { settingsDefaults } from '@/config/settingsConfigs'
import { Sender } from '@ant-design/x'
import { Divider, Flex, Switch, theme, Typography, GetRef } from 'antd'
import { useChatStore } from '@/stores/chatStore'

const { Text } = Typography
const AiSender: React.FC<{
  className?: string
  loading?: boolean
  onSubmit: (value: string) => void
}> = ({ className, loading, onSubmit }) => {
  const chatStore = useChatStore()
  const [value, setValue] = useState('')
  const { token } = theme.useToken()
  const senderRef = useRef<GetRef<typeof Sender>>(null)

  useEffect(() => {
    setTimeout(() => {
      senderRef.current?.focus({
        cursor: 'start',
      })
    }, 100)
  }, [])

  const iconStyle = {
    fontSize: 18,
    color: token.colorText,
  }

  const handleSubmit = () => {
    setValue('')
    chatStore.setAiContent(value)
    onSubmit(value)
  }

  return (
    <Sender
      ref={senderRef}
      value={value}
      className={className}
      onChange={(value) => setValue(value)}
      autoSize={{ minRows: 1, maxRows: 6 }}
      placeholder={
        chatStore.values?.senderPlaceholder ||
        settingsDefaults.senderPlaceholder
      }
      footer={({ components }) => {
        const { SendButton, LoadingButton, SpeechButton } = components
        return (
          <Flex justify='space-between' align='center'>
            <Flex align='center'>
              <Switch
                size='small'
                checked={chatStore.aiParameters.thinking}
                onChange={(value) =>
                  chatStore.setAiParameters({
                    ...chatStore.aiParameters,
                    thinking: value,
                  })
                }
              />
              <Text
                style={{
                  cursor: 'pointer',
                  marginLeft: 5,
                  marginRight: 10,
                }}
                onClick={() =>
                  chatStore.setAiParameters({
                    ...chatStore.aiParameters,
                    thinking: !chatStore.aiParameters.thinking,
                  })
                }
              >
                深度思考
              </Text>
              <Switch
                size='small'
                checked={chatStore.aiParameters.searching}
                onChange={(value) =>
                  chatStore.setAiParameters({
                    ...chatStore.aiParameters,
                    searching: value,
                  })
                }
              />
              <Text
                style={{
                  cursor: 'pointer',
                  marginLeft: 5,
                  marginRight: 10,
                }}
                onClick={() =>
                  chatStore.setAiParameters({
                    ...chatStore.aiParameters,
                    searching: !chatStore.aiParameters.searching,
                  })
                }
              >
                联网搜索
              </Text>
            </Flex>
            <Flex align='center'>
              {chatStore.values?.senderAllowSpeech && (
                <>
                  <SpeechButton style={iconStyle} />
                  <Divider type='vertical' />
                </>
              )}
              {loading ? (
                <LoadingButton
                  type='primary'
                  disabled={false}
                  onClick={() => {
                    chatStore.setAiLoading(false)
                  }}
                />
              ) : (
                <SendButton type='primary' />
              )}
            </Flex>
          </Flex>
        )
      }}
      onSubmit={handleSubmit}
      onCancel={() => {
        chatStore.setAiLoading(false)
      }}
      loading={loading}
      // autoFocus={true}
      actions={false}
      allowSpeech={chatStore.values?.senderAllowSpeech}
    />
  )
}

export default AiSender
