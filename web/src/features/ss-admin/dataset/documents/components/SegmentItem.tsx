import { useState } from 'react'
import { Segment } from '@/models'
import { mdToHtml } from '@/utils'
import {
  EditOutlined,
  DeleteOutlined,
  PlusCircleOutlined,
  ArrowUpOutlined,
  ArrowDownOutlined,
} from '@ant-design/icons'
import { Input, Button, Card, Tooltip, Row } from 'antd'
import '@/assets/github-markdown.css'

const SegmentItem: React.FC<{
  siteUrl: string
  segment: Segment
  index: number
  onSubmit: (segment: Segment, op: string, value: string, index: number) => void
}> = ({ siteUrl, segment, index, onSubmit }) => {
  const [editing, setEditing] = useState(false)
  const [adding, setAdding] = useState(false)
  const [text, setText] = useState(segment.text)
  const [addText, setAddText] = useState('')

  const handleEditClick = () => {
    setEditing(true)
  }

  const handleAddClick = () => {
    setAdding(true)
    setAddText('')
  }

  const handleRemoveClick = () => {
    onSubmit(segment, 'remove', '', index)
  }

  return (
    <>
      {editing ? (
        <>
          <Input.TextArea
            value={text}
            onChange={(e) => setText(e.target.value)}
            autoFocus
            placeholder='请输入文本'
            autoSize={{ minRows: 3, maxRows: 10 }}
          />
          <div className='segment-item-buttons'>
            <Button
              type='primary'
              onClick={() => {
                onSubmit(segment, 'edit', text, index)
                setEditing(false)
              }}
            >
              确定
            </Button>
            <Button
              style={{ marginLeft: 8 }}
              onClick={() => {
                setText(segment.text)
                setEditing(false)
              }}
            >
              取消
            </Button>
          </div>
        </>
      ) : (
        <div className='chunk-section'>
          <Row
            className='chunk-buttons'
            style={{ position: 'absolute', top: 0, right: 0, zIndex: 10 }}
          >
            <Card
              size='small'
              styles={{ body: { padding: '8px 10px', borderRadius: '8px' } }}
            >
              <Tooltip title='编辑' placement='left'>
                <EditOutlined
                  style={{ marginRight: 15, cursor: 'pointer' }}
                  onClick={handleEditClick}
                />
              </Tooltip>
              <Tooltip title='添加' placement='left'>
                <PlusCircleOutlined
                  style={{ marginRight: 15, cursor: 'pointer' }}
                  onClick={handleAddClick}
                />
              </Tooltip>
              <Tooltip title='删除' placement='left'>
                <DeleteOutlined
                  style={{ marginRight: 15, cursor: 'pointer' }}
                  onClick={handleRemoveClick}
                />
              </Tooltip>
              <Tooltip title='向上移动' placement='left'>
                <ArrowUpOutlined
                  style={{ marginRight: 15, cursor: 'pointer' }}
                />
              </Tooltip>
              <Tooltip title='向下移动' placement='left'>
                <ArrowDownOutlined style={{ cursor: 'pointer' }} />
              </Tooltip>
            </Card>
          </Row>
          <div
            className='markdown-body'
            style={{ backgroundColor: 'transparent' }}
            dangerouslySetInnerHTML={{ __html: mdToHtml(text, siteUrl) }}
          />
        </div>
      )}

      {adding && (
        <>
          <Input.TextArea
            value={addText}
            onChange={(e) => setAddText(e.target.value)}
            autoFocus
            placeholder='请输入文本'
            autoSize={{ minRows: 3, maxRows: 10 }}
          />
          <div className='segment-item-buttons'>
            <Button
              type='primary'
              onClick={() => {
                if (addText) {
                  onSubmit(segment, 'add', addText, index)
                }
                setAdding(false)
              }}
            >
              确定
            </Button>
            <Button
              style={{ marginLeft: 8 }}
              onClick={() => {
                setAdding(false)
              }}
            >
              取消
            </Button>
          </div>
        </>
      )}
    </>
  )
}

export default SegmentItem
