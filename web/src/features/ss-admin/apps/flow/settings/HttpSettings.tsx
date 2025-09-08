import { useEffect } from 'react'
import { uuid } from '@/utils'
import { Form, Flex, Input, Radio, FormInstance } from 'antd'
import { useStore } from '../store'

const HttpSettings: React.FC<{
  nodeId: string
  form: FormInstance
}> = ({ nodeId, form }) => {
  const store = useStore()
  const settings = store.getNodeSettings(nodeId)

  useEffect(() => {
    form.setFieldsValue({
      httpMethod: settings.httpMethod,
      httpUrl: settings.httpUrl,
      httpSecurityKey: settings.httpSecurityKey || uuid(),
    })
  }, [settings, form])

  return (
    <Flex gap='middle' vertical style={{ padding: '20px' }}>
      <Form.Item
        label='调用方法'
        name='httpMethod'
        rules={[
          {
            required: true,
            message: '请选择调用方法',
          },
        ]}
      >
        <Radio.Group
          options={[
            { value: 'GET', label: 'GET' },
            { value: 'POST', label: 'POST' },
          ]}
        ></Radio.Group>
      </Form.Item>
      <Form.Item
        label='URL 地址'
        name='httpUrl'
        rules={[
          {
            required: true,
            message: '',
          },
        ]}
      >
        <Form.Item
          name='httpUrl'
          rules={[
            {
              required: true,
              message: '请输入URL 地址',
            },
          ]}
          noStyle
        >
          <Input placeholder='请输入URL 地址' />
        </Form.Item>
        <div className='tips'>
          请填写完整的 URL 地址，例如：http://www.example.com/api/
        </div>
      </Form.Item>
      <Form.Item label='安全令牌'>
        <Form.Item name='httpSecurityKey' noStyle>
          <Input placeholder='请输入安全令牌' />
        </Form.Item>
        <div className='tips'>
          可选择使用 Authorization Bearer 安全令牌校验签名提高安全性
        </div>
      </Form.Item>
    </Flex>
  )
}

export default HttpSettings
