import { useQuery } from '@tanstack/react-query'
import {
  Form,
  Select,
  InputNumber,
  Checkbox,
  Row,
  Button,
  Divider,
  App,
  Card,
  Skeleton,
  Space,
} from 'antd'
import datasetSettingsApi from '@/api/admin/dataset/datasetSettingsApi'
import { getQueryInt } from '@/utils/query'
import './app.css'

const { Option } = Select

const siteId = getQueryInt('siteId')

function Settings() {
  const { modal, message } = App.useApp()
  const [form] = Form.useForm()

  const { isPending } = useQuery({
    queryKey: ['dataset', 'settings', siteId],
    queryFn: async () => {
      const res = await datasetSettingsApi.get({ siteId })
      if (res) {
        form.setFieldsValue(res.settings)
      }
      return res
    },
  })

  const handleSubmit = async (isRechunk: boolean) => {
    const valid = await form.validateFields()
    if (!valid) {
      return
    }
    const settings = form.getFieldsValue()
    if (isRechunk) {
      modal.confirm({
        title: '确定修改分段策略并重新分段吗？',
        content:
          '系统将根据原始文档对当前知识库自动重新分段，手动修改的分段内容不会保留。',
        okText: '修改并重新分段',
        okType: 'danger',
        cancelText: '取消',
        onOk: async () => {
          const res = await datasetSettingsApi.submit({
            siteId,
            ...settings,
            isRechunk,
          })
          if (res) {
            message.success('分段策略修改成功!')
            setTimeout(() => {
              location.href = '/ss-admin/dataset/status?siteId=' + siteId
            }, 1000)
          }
        },
      })
    } else {
      const res = await datasetSettingsApi.submit({
        siteId,
        ...settings,
        isRechunk,
      })
      if (res) {
        message.success('分段策略修改成功!')
      }
    }
  }

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <Row justify='center'>
      <Card style={{ width: '700px', margin: '20px auto' }}>
        <Card.Meta title='分段策略' />
        <Form form={form} layout='vertical' style={{ marginTop: 20 }}>
          <Form.Item
            label='分段标识符'
            name='separators'
            rules={[{ required: true, message: '必填项' }]}
          >
            <Select mode='tags' style={{ width: '100%' }}>
              <Option value='double_new_line'>2个换行</Option>
              <Option value='new_line'>换行</Option>
              <Option value='。'>中文句号</Option>
              <Option value='，'>中文逗号</Option>
              <Option value='！'>中文叹号</Option>
              <Option value='.'>英文句号</Option>
              <Option value=','>英文逗号</Option>
              <Option value='!'>英文叹号</Option>
              <Option value='？'>中文问号</Option>
              <Option value='?'>英文问号</Option>
              <Option value=' '>空格</Option>
            </Select>
          </Form.Item>

          <Form.Item
            label='分段最大长度'
            name='chunkSize'
            rules={[{ required: true, message: '请填写分段最大长度' }]}
          >
            <InputNumber style={{ width: '100%' }} min={100} max={5000} />
          </Form.Item>

          <Form.Item
            label='分段重叠长度'
            name='chunkOverlap'
            rules={[{ required: true, message: '请填写分段重叠长度' }]}
          >
            <InputNumber style={{ width: '100%' }} min={10} max={2000} />
          </Form.Item>

          <Form.Item label='文本预处理规则'>
            <Form.Item name='isChunkReplaces' valuePropName='checked' noStyle>
              <Checkbox>替换掉连续的空格、换行符和制表符</Checkbox>
            </Form.Item>
            <Form.Item
              name='isChunkDeletes'
              valuePropName='checked'
              noStyle
              style={{ marginLeft: 10 }}
            >
              <Checkbox>删除所有 URL 和电子邮箱地址</Checkbox>
            </Form.Item>
          </Form.Item>
          <Divider></Divider>
          <Row align='middle' justify='center' style={{ marginTop: '20px 0' }}>
            <Space>
              <Button
                type='primary'
                onClick={() => handleSubmit(false)}
                style={{ marginRight: 10 }}
              >
                保 存
              </Button>
              <Button
                color='danger'
                variant='outlined'
                onClick={() => handleSubmit(true)}
                style={{ marginRight: 10 }}
              >
                保存并重新分段
              </Button>
            </Space>
          </Row>
        </Form>
      </Card>
    </Row>
  )
}

export default Settings
