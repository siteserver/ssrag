import { useEffect } from 'react'
import { ChunkConfig } from '@/models'
import { Form, Select, InputNumber, Checkbox, Row, Collapse } from 'antd'

const { Option } = Select
const { Panel } = Collapse

const Settings: React.FC<{
  config: ChunkConfig | null
  onChange: (values: ChunkConfig) => void
}> = ({ config, onChange }) => {
  const [form] = Form.useForm()

  useEffect(() => {
    form.setFieldsValue({ ...config })
  }, [config, form])

  const handleChange = (_changeValues: ChunkConfig, allValues: ChunkConfig) => {
    onChange(allValues)
  }

  return (
    <Form form={form} layout='vertical' onValuesChange={handleChange}>
      <Row justify='center'>
        <Collapse defaultActiveKey={['1']} style={{ width: '100%' }}>
          <Panel header='分段策略' key='1'>
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
          </Panel>
        </Collapse>
      </Row>
    </Form>
  )
}

export default Settings
