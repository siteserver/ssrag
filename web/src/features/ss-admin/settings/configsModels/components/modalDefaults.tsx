import { useEffect, useState } from 'react'
import { useQuery, useQueryClient } from '@tanstack/react-query'
import { ModelType, getModelTypeDisplayName } from '@/enums'
import { ModelProvider } from '@/models'
import { Modal, Button, Select, Form, Space, message, Spin, Alert } from 'antd'
import configsModelsApi from '@/api/admin/settings/configsModelsApi'

interface ModalProviderProps {
  handleClose: (refresh: boolean) => void
  setTaskId: (taskId: string) => void
}

const ModalDefaults: React.FC<ModalProviderProps> = ({
  handleClose,
  setTaskId,
}) => {
  const queryClient = useQueryClient()
  const [form] = Form.useForm()
  const [llmProviders, setLlmProviders] = useState<ModelProvider[]>([])
  const [textEmbeddingProviders, setTextEmbeddingProviders] = useState<
    ModelProvider[]
  >([])
  const [rerankProviders, setRerankProviders] = useState<ModelProvider[]>([])
  const [toImageProviders, setToImageProviders] = useState<ModelProvider[]>([])
  const [speech2TextProviders, setSpeech2TextProviders] = useState<
    ModelProvider[]
  >([])
  const [ttsProviders, setTtsProviders] = useState<ModelProvider[]>([])
  const { isPending, data } = useQuery({
    queryKey: ['ss-admin', 'settings', 'configsDefaults'],
    queryFn: async () => {
      const res = await configsModelsApi.getDefaults()
      return res
    },
  })

  useEffect(() => {
    if (data) {
      const defaultLLMId =
        data.defaultLLMProviderId && data.defaultLLMModelId
          ? `${data.defaultLLMProviderId}:${data.defaultLLMModelId}`
          : ''
      const defaultTextEmbeddingId =
        data.defaultTextEmbeddingProviderId && data.defaultTextEmbeddingModelId
          ? `${data.defaultTextEmbeddingProviderId}:${data.defaultTextEmbeddingModelId}`
          : ''
      const defaultRerankId =
        data.defaultRerankProviderId && data.defaultRerankModelId
          ? `${data.defaultRerankProviderId}:${data.defaultRerankModelId}`
          : ''
      const defaultToImageId =
        data.defaultToImageProviderId && data.defaultToImageModelId
          ? `${data.defaultToImageProviderId}:${data.defaultToImageModelId}`
          : ''
      const defaultSpeech2TextId =
        data.defaultSpeech2TextProviderId && data.defaultSpeech2TextModelId
          ? `${data.defaultSpeech2TextProviderId}:${data.defaultSpeech2TextModelId}`
          : ''
      const defaultTTSId =
        data.defaultTTSProviderId && data.defaultTTSModelId
          ? `${data.defaultTTSProviderId}:${data.defaultTTSModelId}`
          : ''
      form.setFieldsValue({
        defaultLLMId,
        defaultTextEmbeddingId,
        defaultRerankId,
        defaultToImageId,
        defaultSpeech2TextId,
        defaultTTSId,
      })
      setLlmProviders(data.llmProviders)
      setTextEmbeddingProviders(data.textEmbeddingProviders)
      setRerankProviders(data.rerankProviders)
      setToImageProviders(data.toImageProviders)
      setSpeech2TextProviders(data.speech2TextProviders)
      setTtsProviders(data.ttsProviders)
    }
  }, [data, form])

  const handleSubmit = (isTextEmbeddingChanged: boolean) => {
    form.validateFields().then((values) => {
      let defaultLLMProviderId = ''
      let defaultLLMModelId = ''
      if (values.defaultLLMId) {
        const defaultLLMId = values.defaultLLMId.split(':')
        defaultLLMProviderId = defaultLLMId[0]
        defaultLLMModelId = defaultLLMId[1]
      }

      let defaultTextEmbeddingProviderId = ''
      let defaultTextEmbeddingModelId = ''
      if (values.defaultTextEmbeddingId) {
        const defaultTextEmbeddingId = values.defaultTextEmbeddingId.split(':')
        defaultTextEmbeddingProviderId = defaultTextEmbeddingId[0]
        defaultTextEmbeddingModelId = defaultTextEmbeddingId[1]
      }

      let defaultRerankProviderId = ''
      let defaultRerankModelId = ''
      if (values.defaultRerankId) {
        const defaultRerankId = values.defaultRerankId.split(':')
        defaultRerankProviderId = defaultRerankId[0]
        defaultRerankModelId = defaultRerankId[1]
      }

      let defaultToImageProviderId = ''
      let defaultToImageModelId = ''
      if (values.defaultToImageId) {
        const defaultToImageId = values.defaultToImageId.split(':')
        defaultToImageProviderId = defaultToImageId[0]
        defaultToImageModelId = defaultToImageId[1]
      }

      let defaultSpeech2TextProviderId = ''
      let defaultSpeech2TextModelId = ''
      if (values.defaultSpeech2TextId) {
        const defaultSpeech2TextId = values.defaultSpeech2TextId.split(':')
        defaultSpeech2TextProviderId = defaultSpeech2TextId[0]
        defaultSpeech2TextModelId = defaultSpeech2TextId[1]
      }

      let defaultTTSProviderId = ''
      let defaultTTSModelId = ''
      if (values.defaultTTSId) {
        const defaultTTSId = values.defaultTTSId.split(':')
        defaultTTSProviderId = defaultTTSId[0]
        defaultTTSModelId = defaultTTSId[1]
      }
      configsModelsApi
        .submitDefaults({
          defaultLLMProviderId,
          defaultLLMModelId,
          defaultTextEmbeddingProviderId,
          defaultTextEmbeddingModelId,
          defaultRerankProviderId,
          defaultRerankModelId,
          defaultToImageProviderId,
          defaultToImageModelId,
          defaultSpeech2TextProviderId,
          defaultSpeech2TextModelId,
          defaultTTSProviderId,
          defaultTTSModelId,
        })
        .then((res) => {
          if (res) {
            queryClient.invalidateQueries({
              queryKey: ['ss-admin', 'settings', 'configsDefaults'],
            })
            message.success('保存成功')
            if (isTextEmbeddingChanged && res.value) {
              setTaskId(res.value)
            }
            handleClose(true)
          }
        })
    })
  }

  return (
    <Modal
      open={true}
      title='默认模型设置'
      onCancel={() => handleClose(false)}
      footer={null}
      destroyOnHidden
    >
      <Spin spinning={isPending}>
        <Form form={form} layout='vertical' style={{ gap: 20 }}>
          <Form.Item
            name='defaultLLMId'
            label={`${getModelTypeDisplayName(ModelType.LLM)}模型`}
            rules={[{ required: true, message: '请选择模型' }]}
          >
            <Select
              style={{ width: '100%' }}
              options={llmProviders.map((provider) => ({
                label: (
                  <span className='text-base font-bold'>
                    {provider.providerName}
                  </span>
                ),
                title: provider.providerName,
                options: provider.models.map((model) => ({
                  label: (
                    <div className='flex items-center space-x-2'>
                      <img
                        src={`/assets/images/providers/${provider.providerId}/${provider.iconUrl}`}
                        alt={provider.providerName}
                        style={{
                          maxWidth: 'fit-content',
                        }}
                        className='h-4 w-4 rounded object-contain'
                      />
                      <span>{model.modelId}</span>
                    </div>
                  ),
                  value: `${provider.providerId}:${model.modelId}`,
                })),
              }))}
            />
          </Form.Item>
          <Form.Item
            name='defaultTextEmbeddingId'
            label={`${getModelTypeDisplayName(ModelType.TEXT_EMBEDDING)}模型`}
            rules={[{ required: true, message: '请选择模型' }]}
            help={
              <Alert
                message='请注意，更改嵌入模型后，系统将重新生成所有知识库的向量值。'
                type='warning'
                showIcon
                style={{ marginBottom: 10, marginTop: 10 }}
              />
            }
          >
            <Select
              style={{ width: '100%' }}
              options={textEmbeddingProviders.map((provider) => ({
                label: (
                  <span className='text-base font-bold'>
                    {provider.providerName}
                  </span>
                ),
                title: provider.providerName,
                options: provider.models.map((model) => ({
                  label: (
                    <div className='flex items-center space-x-2'>
                      <img
                        src={`/assets/images/providers/${provider.providerId}/${provider.iconUrl}`}
                        alt={provider.providerName}
                        style={{
                          maxWidth: 'fit-content',
                        }}
                        className='h-4 w-4 rounded object-contain'
                      />
                      <span>{model.modelId}</span>
                    </div>
                  ),
                  value: `${provider.providerId}:${model.modelId}`,
                })),
              }))}
            />
          </Form.Item>
          <Form.Item
            name='defaultRerankId'
            label={`${getModelTypeDisplayName(ModelType.RERANK)}模型`}
          >
            <Select
              style={{ width: '100%' }}
              options={rerankProviders.map((provider) => ({
                label: (
                  <span className='text-base font-bold'>
                    {provider.providerName}
                  </span>
                ),
                title: provider.providerName,
                options: provider.models.map((model) => ({
                  label: (
                    <div className='flex items-center space-x-2'>
                      <img
                        src={`/assets/images/providers/${provider.providerId}/${provider.iconUrl}`}
                        alt={provider.providerName}
                        style={{
                          maxWidth: 'fit-content',
                        }}
                        className='h-4 w-4 rounded object-contain'
                      />
                      <span>{model.modelId}</span>
                    </div>
                  ),
                  value: `${provider.providerId}:${model.modelId}`,
                })),
              }))}
            />
          </Form.Item>
          <Form.Item
            name='defaultToImageId'
            label={`${getModelTypeDisplayName(ModelType.TO_IMAGE)}模型`}
          >
            <Select
              style={{ width: '100%' }}
              options={toImageProviders.map((provider) => ({
                label: (
                  <span className='text-base font-bold'>
                    {provider.providerName}
                  </span>
                ),
                title: provider.providerName,
                options: provider.models.map((model) => ({
                  label: (
                    <div className='flex items-center space-x-2'>
                      <img
                        src={`/assets/images/providers/${provider.providerId}/${provider.iconUrl}`}
                        alt={provider.providerName}
                        style={{
                          maxWidth: 'fit-content',
                        }}
                        className='h-4 w-4 rounded object-contain'
                      />
                      <span>{model.modelId}</span>
                    </div>
                  ),
                  value: `${provider.providerId}:${model.modelId}`,
                })),
              }))}
            />
          </Form.Item>
          <Form.Item
            name='defaultSpeech2TextId'
            label={`${getModelTypeDisplayName(ModelType.SPEECH2TEXT)}模型`}
          >
            <Select
              style={{ width: '100%' }}
              options={speech2TextProviders.map((provider) => ({
                label: (
                  <span className='text-base font-bold'>
                    {provider.providerName}
                  </span>
                ),
                title: provider.providerName,
                options: provider.models.map((model) => ({
                  label: (
                    <div className='flex items-center space-x-2'>
                      <img
                        src={`/assets/images/providers/${provider.providerId}/${provider.iconUrl}`}
                        alt={provider.providerName}
                        style={{
                          maxWidth: 'fit-content',
                        }}
                        className='h-4 w-4 rounded object-contain'
                      />
                      <span>{model.modelId}</span>
                    </div>
                  ),
                  value: `${provider.providerId}:${model.modelId}`,
                })),
              }))}
            />
          </Form.Item>
          <Form.Item
            name='defaultTTSId'
            label={`${getModelTypeDisplayName(ModelType.TTS)}模型`}
          >
            <Select
              style={{ width: '100%' }}
              options={ttsProviders.map((provider) => ({
                label: (
                  <span className='text-base font-bold'>
                    {provider.providerName}
                  </span>
                ),
                title: provider.providerName,
                options: provider.models.map((model) => ({
                  label: (
                    <div className='flex items-center space-x-2'>
                      <img
                        src={`/assets/images/providers/${provider.providerId}/${provider.iconUrl}`}
                        alt={provider.providerName}
                        style={{
                          maxWidth: 'fit-content',
                        }}
                        className='h-4 w-4 rounded object-contain'
                      />
                      <span>{model.modelId}</span>
                    </div>
                  ),
                  value: `${provider.providerId}:${model.modelId}`,
                })),
              }))}
            />
          </Form.Item>

          <Form.Item style={{ marginBottom: 0, textAlign: 'right' }}>
            <Space>
              <Button onClick={() => handleClose(false)}>取消</Button>
              <Button
                type='primary'
                onClick={() => {
                  const isTextEmbeddingChanged =
                    data &&
                    data.defaultTextEmbeddingProviderId &&
                    `${data.defaultTextEmbeddingProviderId}:${data.defaultTextEmbeddingModelId}` !==
                      form.getFieldValue('defaultTextEmbeddingId')
                      ? true
                      : false
                  if (isTextEmbeddingChanged) {
                    Modal.confirm({
                      title: '提示',
                      content:
                        '更改嵌入模型后，系统将重新生成所有知识库的向量值。\n\n确定要继续吗？',
                      okText: '确定',
                      cancelText: '取消',
                      onOk: () => {
                        handleSubmit(isTextEmbeddingChanged)
                      },
                    })
                  } else {
                    handleSubmit(isTextEmbeddingChanged)
                  }
                }}
              >
                保存
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Spin>
    </Modal>
  )
}

export default ModalDefaults
