import { useState } from 'react'
import { Prompt } from '@/models'
import { DeleteOutlined, EditOutlined, PlusOutlined } from '@ant-design/icons'
import { Row, Col, Button, Card, Space } from 'antd'
import PromptAdd from './promptAdd'

const { Meta } = Card

const SettingsPrompts: React.FC<{
  siteId: number
  prompts: Prompt[]
  onChange: (prompts: Prompt[]) => void
}> = ({ siteId, prompts, onChange }) => {
  const [modalPromptConfig, setModalPromptConfig] = useState(false)
  const [prompt, setPrompt] = useState<Prompt | null>(null)

  const handleAdd = () => {
    setPrompt({
      uuid: '',
      title: '',
      iconUrl: '',
      text: '',
    })
    setModalPromptConfig(true)
  }

  const handleEdit = (prompt: Prompt) => {
    setPrompt(prompt)
    setModalPromptConfig(true)
  }

  const handleDelete = (prompt: Prompt) => {
    const newPrompts = prompts.filter((p) => p.uuid !== prompt.uuid)
    onChange(newPrompts)
  }

  return (
    <Row gutter={[16, 16]}>
      {prompts.map((prompt) => {
        const icon = (
          <img
            src={prompt.iconUrl}
            className='h-10 w-10 rounded object-contain'
            alt={`${prompt.title} icon`}
          />
        )
        return (
          <Col key={prompt.uuid} span={12}>
            <Card
              hoverable
              className='apps-card'
              variant='outlined'
              onClick={() => handleEdit(prompt)}
              actions={[
                <EditOutlined
                  key='edit'
                  onClick={(e: React.MouseEvent<HTMLAnchorElement>) => {
                    e.stopPropagation()
                    handleEdit(prompt)
                  }}
                />,
                <DeleteOutlined
                  key='delete'
                  onClick={(e: React.MouseEvent<HTMLAnchorElement>) => {
                    e.stopPropagation()
                    handleDelete(prompt)
                  }}
                />,
              ]}
            >
              <Meta
                avatar={icon}
                title={
                  <Space>
                    <a>{prompt.title}</a>
                  </Space>
                }
                description={prompt.text || '暂无介绍'}
              />
            </Card>
          </Col>
        )
      })}

      <Col span={12}>
        <Button
          type='dashed'
          size='large'
          className='apps-add-button'
          icon={<PlusOutlined />}
          onClick={() => handleAdd()}
        >
          添加提示
        </Button>
      </Col>

      {modalPromptConfig && prompt && (
        <PromptAdd
          siteId={siteId}
          prompt={prompt}
          onAdd={(prompt) => {
            setModalPromptConfig(false)
            onChange([...prompts, prompt])
          }}
          onEdit={(prompt) => {
            setModalPromptConfig(false)
            onChange(prompts.map((p) => (p.uuid === prompt.uuid ? prompt : p)))
          }}
          onClose={() => {
            setModalPromptConfig(false)
          }}
        />
      )}
    </Row>
  )
}

export default SettingsPrompts
