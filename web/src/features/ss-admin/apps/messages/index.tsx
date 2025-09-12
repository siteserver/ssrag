import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { ChatGroup } from '@/models'
import { getQueryInt } from '@/utils'
import {
  Splitter,
  Typography,
  Space,
  Layout,
  Form,
  Skeleton,
  Table,
  Input,
  DatePicker,
} from 'antd'
import dayjs, { Dayjs } from 'dayjs'
import messagesApi from '@/api/admin/apps/messagesApi'
import './app.css'
import ChatMessages from './components/ChatMessages'

const { Header, Content } = Layout
const { Text } = Typography

const siteId = getQueryInt('siteId')

const Messages: React.FC = () => {
  const [form] = Form.useForm()
  const [chatGroups, setChatGroups] = useState<ChatGroup[]>([])
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const [dateRange, setDateRange] = useState<[Dayjs | null, Dayjs | null]>([
    null,
    null,
  ])
  const [title, setTitle] = useState('')
  const [selectedSessionId, setSelectedSessionId] = useState<
    string | undefined
  >()

  const { isPending } = useQuery({
    queryKey: [
      'ss-admin',
      'apps',
      'messages',
      siteId,
      page,
      pageSize,
      dateRange,
      title,
    ],
    queryFn: async () => {
      const res = await messagesApi.get({
        siteId,
        page,
        pageSize,
        dateStart: dateRange[0]?.format('YYYY-MM-DD') || null,
        dateEnd: dateRange[1]?.format('YYYY-MM-DD') || null,
        title,
      })
      if (res) {
        setChatGroups(res.chatGroups)
      }
      return res
    },
  })

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <Form layout='vertical' size='middle' form={form}>
      <Splitter
        style={{
          height: '100vh',
          boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)',
        }}
      >
        <Splitter.Panel
          defaultSize='40%'
          min='20%'
          max='70%'
          style={{ backgroundColor: '#fff' }}
        >
          <Header
            style={{
              backgroundColor: '#f0f2f5',
              height: '57px',
              padding: '0 16px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'space-between',
              borderBottom: '1px solid #f0f0f0',
              position: 'sticky',
              top: 0,
              zIndex: 1,
            }}
          >
            <div className='settings-header-title'>
              <Text strong className='settings-header-title-text'>
                对话消息日志
              </Text>
            </div>
          </Header>
          <Content>
            <Space
              direction='vertical'
              style={{ width: '100%', padding: '16px' }}
            >
              <Space>
                <Form.Item label='时间' name='dateRange'>
                  <DatePicker.RangePicker
                    showTime={false}
                    value={dateRange}
                    onChange={(value) => {
                      if (value && value[0] && value[1]) {
                        setDateRange([value[0], value[1]])
                      }
                    }}
                  />
                </Form.Item>
                <Form.Item label='标题' name='title'>
                  <Input
                    placeholder='请输入标题关键词'
                    style={{ width: 200 }}
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                  />
                </Form.Item>
              </Space>
            </Space>
            <Table
              dataSource={chatGroups}
              onRow={(record) => ({
                onClick: () => {
                  setSelectedSessionId(record.sessionId)
                },
                style: { cursor: 'pointer' },
                className:
                  selectedSessionId === record.sessionId
                    ? 'ant-table-row-selected'
                    : '',
              })}
              columns={[
                {
                  title: '对话人',
                  dataIndex: 'name',
                  key: 'name',
                  width: 150,
                  render: (_, record: ChatGroup) => {
                    if (record.adminName) {
                      return `${record.adminName}（管理员）`
                    } else if (record.userName) {
                      return `${record.userName}（用户）`
                    }
                    return '匿名用户'
                  },
                },
                {
                  title: '标题',
                  dataIndex: 'title',
                  key: 'title',
                },
                {
                  title: '对话时间',
                  dataIndex: 'lastModifiedDate',
                  key: 'lastModifiedDate',
                  width: 150,
                  render: (date: Date) =>
                    dayjs(date).format('YYYY-MM-DD HH:mm:ss'),
                },
              ]}
              rowKey='id'
              pagination={{
                pageSize,
                showSizeChanger: true,
                showQuickJumper: true,
                showTotal: (total) => `共 ${total} 条`,
                position: ['bottomCenter'],
                onChange(page, pageSize) {
                  setPage(page)
                  setPageSize(pageSize)
                },
              }}
              size='small'
            />
          </Content>
        </Splitter.Panel>
        <Splitter.Panel>
          {selectedSessionId && (
            <ChatMessages siteId={siteId} sessionId={selectedSessionId} />
          )}
        </Splitter.Panel>
      </Splitter>
    </Form>
  )
}

export default Messages
