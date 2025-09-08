import { ModelProvider, ProviderManifest } from '@/models'
import { FormInstance, Modal } from 'antd'
import { Form, Input, Select, Radio, Button } from 'antd'
import { ExternalLink } from 'lucide-react'

interface ModalProviderProps {
  selectedManifest: ProviderManifest | undefined
  selectedProvider: ModelProvider | null
  handleCancel: () => void
  handleSubmit: (values: Record<string, unknown>) => void
  form: FormInstance
}
const ModalProvider: React.FC<ModalProviderProps> = ({
  selectedManifest,
  selectedProvider,
  handleCancel,
  handleSubmit,
  form,
}) => {
  return (
    <Modal
      open={true}
      title={`${selectedProvider ? '设置' : '添加'}${selectedManifest?.label?.zh_Hans || selectedManifest?.label?.en_US || ''}`}
      onCancel={handleCancel}
      footer={null}
      destroyOnHidden
    >
      <Form form={form} layout='vertical' onFinish={handleSubmit}>
        {selectedManifest?.model.provider_credential_schema?.credential_form_schemas.map(
          (schema) => (
            <Form.Item
              key={schema.variable}
              name={schema.variable}
              label={schema.label?.zh_Hans || schema.label?.en_US}
              rules={[
                {
                  required: schema.required,
                  message: `请输入 ${schema.label?.zh_Hans || schema.label?.en_US}`,
                },
              ]}
            >
              {schema.type === 'text-input' && (
                <Input
                  placeholder={
                    schema.placeholder?.zh_Hans || schema.placeholder?.en_US
                  }
                  maxLength={
                    schema.max_length > 0 ? schema.max_length : undefined
                  }
                />
              )}
              {schema.type === 'secret-input' && (
                <Input.Password
                  placeholder={
                    schema.placeholder?.zh_Hans || schema.placeholder?.en_US
                  }
                  maxLength={
                    schema.max_length > 0 ? schema.max_length : undefined
                  }
                />
              )}
              {schema.type === 'select' && (
                <Select
                  placeholder={
                    schema.placeholder?.zh_Hans || schema.placeholder?.en_US
                  }
                  options={schema.options?.map((option) => ({
                    label: option.label?.zh_Hans || option.label?.en_US,
                    value: option.value,
                  }))}
                />
              )}
              {schema.type === 'radio' && (
                <Radio.Group>
                  {schema.options?.map((option) => (
                    <Radio key={option.value} value={option.value}>
                      {option.label?.zh_Hans || option.label?.en_US}
                    </Radio>
                  ))}
                </Radio.Group>
              )}
            </Form.Item>
          )
        )}
        {(selectedManifest?.model?.help?.url?.zh_Hans ||
          selectedManifest?.model?.help?.url?.en_US) && (
          <div style={{ marginBottom: 16 }}>
            <a
              href={
                selectedManifest?.model?.help?.url?.zh_Hans ||
                selectedManifest?.model?.help?.url?.en_US
              }
              target='_blank'
              rel='noopener noreferrer'
            >
              <span className='flex items-center'>
                {selectedManifest?.model?.help?.title?.zh_Hans ||
                  selectedManifest?.model?.help?.title?.en_US}
                <ExternalLink className='ml-1 h-4 w-4' />
              </span>
            </a>
          </div>
        )}
        <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8 }}>
          <Button onClick={handleCancel}>取 消</Button>
          <Button type='primary' htmlType='submit'>
            确 定
          </Button>
        </div>
        <div
          style={{
            marginTop: 24,
            textAlign: 'center',
            color: '#888',
            fontSize: 12,
          }}
        >
          <span role='img' aria-label='lock'>
            🔒
          </span>{' '}
          您的凭据将安全存储
        </div>
      </Form>
    </Modal>
  )
}

export default ModalProvider
