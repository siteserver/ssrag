import React, { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { getModelSkillDisplayName, getModelTypeDisplayName } from '@/enums'
import { Model, ModelProvider, ProviderManifest } from '@/models'
import {
  Modal,
  Button,
  Form,
  Skeleton,
  App,
  Tooltip,
  Divider,
  Tag,
  Badge,
  Alert,
  Flex,
} from 'antd'
import { KeySquare, Plus, Trash2, CircleX, Settings2 } from 'lucide-react'
import configsModelsApi from '@/api/admin/settings/configsModelsApi'
import ModalDefaults from './components/modalDefaults'
import ModalModel from './components/modalModel'
import ModalProvider from './components/modalProvider'
import TaskStatus from './components/taskStatus'

const ConfigsModelsApp: React.FC = () => {
  const { message } = App.useApp()
  const [providers, setProviders] = useState<ModelProvider[]>([])
  const [manifests, setManifests] = useState<ProviderManifest[]>([])
  const [search, setSearch] = useState('')
  const [modalProviderOpen, setModalProviderOpen] = useState(false)
  const [modalModelOpen, setModalModelOpen] = useState(false)
  const [selectedManifest, setSelectedManifest] = useState<
    ProviderManifest | undefined
  >(undefined)
  const [selectedProvider, setSelectedProvider] =
    useState<ModelProvider | null>(null)
  const [selectedModel, setSelectedModel] = useState<Model | null>(null)
  const [form] = Form.useForm<Record<string, unknown>>()
  const [defaultModelModalOpen, setDefaultModelModalOpen] = useState(false)
  const [isDefaultsReady, setIsDefaultsReady] = useState(false)
  const [taskId, setTaskId] = useState<string | null>(null)

  const { isPending, refetch, isRefetching } = useQuery({
    queryKey: ['ss-admin', 'settings', 'configsModels', 'manifests'],
    queryFn: async () => {
      const res = await configsModelsApi.get()
      if (res) {
        setProviders(res.providers || [])
        if (manifests.length === 0) {
          const resManifests = await configsModelsApi.getManifests()
          if (resManifests) {
            setManifests(resManifests.manifests || [])
          }
        }
        if (
          res.configValues &&
          res.configValues.defaultLLMProviderId &&
          res.configValues.defaultLLMModelId &&
          res.configValues.defaultTextEmbeddingProviderId &&
          res.configValues.defaultTextEmbeddingModelId
        ) {
          setIsDefaultsReady(true)
        } else {
          setIsDefaultsReady(false)
        }
      }
      return res
    },
  })

  const filteredManifests = manifests.filter(
    (m) =>
      !providers.find((p) => p.providerId === m.name) &&
      (m.label?.zh_Hans?.toLowerCase().includes(search.toLowerCase()) ||
        m.label?.en_US?.toLowerCase().includes(search.toLowerCase()))
  )

  const handleManifestClick = (
    manifest: ProviderManifest,
    provider: ModelProvider | null
  ) => {
    setSelectedProvider(provider)
    setSelectedManifest(manifest)

    if (manifest.model.provider_credential_schema) {
      manifest.model.provider_credential_schema.credential_form_schemas.forEach(
        (schema) => {
          if (provider?.credentials?.[schema.variable]) {
            form.setFieldsValue({
              [schema.variable]: provider.credentials?.[schema.variable] || '',
            })
          } else {
            form.setFieldsValue({
              [schema.variable]: schema.default || '',
            })
          }
        }
      )
    }
    setModalProviderOpen(true)
  }

  const handleProviderSubmit = async (values: Record<string, unknown>) => {
    try {
      await configsModelsApi.submitProvider({
        providerId: selectedManifest?.name || '',
        providerName:
          selectedManifest?.label?.zh_Hans ||
          selectedManifest?.label?.en_US ||
          '',
        iconUrl: selectedManifest?.model.icon_small?.en_US || '',
        description:
          selectedManifest?.description?.zh_Hans ||
          selectedManifest?.description?.en_US ||
          '',
        credentials: values,
      })
      refetch()
      setModalProviderOpen(false)
      form.resetFields()
      message.success(
        selectedProvider ? '模型供应商密钥设置成功' : '模型供应商添加成功'
      )
    } catch (error) {
      message.error(error as string)
    }
  }

  const handleModelSubmit = async (values: Record<string, unknown>) => {
    if (!selectedManifest) return
    try {
      if (selectedModel) {
        await configsModelsApi.submitModel({
          id: selectedModel.id,
          providerId: selectedManifest.name,
          modelType: selectedModel.modelType as string,
          modelId: values.modelId as string,
          skills: values.skills as string[],
          extendValues: values,
        })
        refetch()
        setModalModelOpen(false)
        message.success('模型更新成功')
      } else {
        const modelId = values.modelId as string
        const provider = providers.find(
          (p) => p.providerId === selectedManifest?.name
        )
        const model = provider?.models.find(
          (model) => model.modelId === modelId
        )
        if (model) {
          message.error('添加失败，该模型已存在！')
          return
        }
        await configsModelsApi.submitModel({
          id: 0,
          providerId: selectedManifest.name,
          modelType: values.modelType as string,
          modelId: modelId,
          skills: values.skills as string[],
          extendValues: values,
        })
        refetch()
        setModalModelOpen(false)
        message.success('模型添加成功')
      }
    } catch (error) {
      message.error(error as string)
    }
  }

  const handleCancel = () => {
    setModalProviderOpen(false)
    setModalModelOpen(false)
    form.resetFields()
  }

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <div className='w-full max-w-[800px] rounded-2xl bg-white p-8'>
      <div className='mb-6 flex items-center justify-between'>
        <h1 className='text-2xl font-semibold'>模型设置</h1>
        {isDefaultsReady && (
          <Button
            color='primary'
            variant='outlined'
            onClick={() => setDefaultModelModalOpen(true)}
          >
            <Settings2 className='h-4 w-4' /> 默认模型设置
          </Button>
        )}
      </div>

      {!isDefaultsReady && !isRefetching && (
        <Alert
          message='默认模型未设置'
          showIcon
          description='请启用并设置默认模型，否则系统无法正常工作'
          type='error'
          action={
            <Button danger onClick={() => setDefaultModelModalOpen(true)}>
              <Settings2 className='h-4 w-4' /> 默认模型设置
            </Button>
          }
          style={{ marginBottom: '20px' }}
        />
      )}

      {taskId && (
        <div className='flex items-center space-x-4'>
          <TaskStatus taskId={taskId} setTaskId={setTaskId} />
        </div>
      )}

      {/* To be configured */}
      {providers.length > 0 && (
        <div className='mb-8'>
          <div className='mb-2 text-lg font-medium'>已启用</div>
          <div className='space-y-4'>
            {providers.map((item) => {
              const manifest = manifests.find((m) => m.name === item.providerId)
              const supported_model_types =
                manifest?.model.supported_model_types || []
              return (
                <div
                  key={item.providerId}
                  className='rounded-lg border bg-blue-50 px-6 py-4'
                >
                  <div className='clear-both flex items-center justify-between'>
                    <div className='flex items-center space-x-4'>
                      <div className='flex h-10 w-10 items-center justify-center rounded text-xl font-bold'>
                        <img
                          src={`/assets/images/providers/${item.providerId}/${item.iconUrl}`}
                          alt={item.providerName}
                          style={{
                            maxWidth: 'fit-content',
                          }}
                          className='h-10 w-10 rounded object-contain'
                        />
                      </div>
                      <div>
                        <div className='text-base font-semibold'>
                          {item.providerName}
                        </div>
                        <div className='mt-1 flex flex-wrap gap-1'>
                          {item.description}
                        </div>
                        <div className='mt-1 flex flex-wrap gap-1'>
                          {supported_model_types.map((model_type) => (
                            <span
                              key={model_type}
                              className='mr-1 rounded bg-blue-200 px-2 py-0.5 text-xs text-blue-700'
                            >
                              {getModelTypeDisplayName(model_type)}
                            </span>
                          ))}
                        </div>
                      </div>
                    </div>
                    <div className='flex items-end space-x-4'>
                      <Flex vertical={true}>
                        <Button
                          type='link'
                          className='px-0'
                          icon={<KeySquare className='h-4 w-4' />}
                          onClick={() => {
                            const manifest = manifests.find(
                              (m) => m.name === item.providerId
                            )
                            if (manifest) {
                              handleManifestClick(manifest, item)
                            }
                          }}
                        >
                          密钥设置
                        </Button>
                        <Tooltip title='删除模型供应商'>
                          <Button
                            type='link'
                            danger
                            className='px-0'
                            icon={<Trash2 className='h-4 w-4' />}
                            onClick={() => {
                              Modal.confirm({
                                title: '删除模型供应商',
                                content: '确定删除该模型供应商吗？',
                                onOk: async () => {
                                  await configsModelsApi.deleteProvider({
                                    providerId: item.providerId,
                                  })
                                  refetch()
                                  message.success('模型供应商删除成功')
                                },
                                onCancel: () => {},
                              })
                            }}
                          >
                            删除供应商
                          </Button>
                        </Tooltip>
                        <Button
                          color='blue'
                          variant='solid'
                          className='mt-3 rounded-full px-4 py-2'
                          icon={<Plus className='h-4 w-4' />}
                          onClick={() => {
                            setSelectedManifest(manifest)
                            setSelectedModel(null)
                            form.resetFields()
                            setModalModelOpen(true)
                          }}
                        >
                          添加模型
                        </Button>
                      </Flex>
                    </div>
                  </div>
                  {supported_model_types.map((model_type) => {
                    const models = item.models.filter(
                      (x) => x.modelType === model_type
                    )
                    if (models.length === 0) {
                      return null
                    }
                    return (
                      <>
                        <Divider className='m-0'>
                          {getModelTypeDisplayName(model_type)}模型
                        </Divider>
                        <div className='flex flex-wrap gap-2'>
                          {models.map((model) => (
                            <div key={model.modelId} className='relative'>
                              <Badge
                                count={
                                  <Tooltip title='删除模型'>
                                    <CircleX
                                      onClick={(e) => {
                                        e.stopPropagation()
                                        Modal.confirm({
                                          title: '删除模型',
                                          content: '确定删除该模型吗？',
                                          onOk: async () => {
                                            await configsModelsApi.deleteModel({
                                              providerId: item.providerId,
                                              modelId: model.modelId,
                                            })
                                            refetch()
                                            message.success('模型删除成功')
                                          },
                                          onCancel: () => {},
                                        })
                                      }}
                                      className='h-4 w-4 cursor-pointer rounded-full bg-white text-red-500'
                                    />
                                  </Tooltip>
                                }
                                offset={[-4, 4]}
                              >
                                <Button
                                  color='blue'
                                  variant='outlined'
                                  className='rounded-full px-4 py-2'
                                  onClick={() => {
                                    setSelectedManifest(manifest)
                                    setSelectedModel(model)
                                    form.setFieldsValue({
                                      modelId: model.modelId,
                                      modelType: model.modelType,
                                      ...model.extendValues,
                                    })
                                    setModalModelOpen(true)
                                  }}
                                >
                                  {model.modelId}
                                  {model.skills &&
                                    model.skills.map((skill) => (
                                      <Tag
                                        key={skill}
                                        style={{
                                          marginRight: 0,
                                          marginLeft: 0,
                                        }}
                                      >
                                        {getModelSkillDisplayName(skill)}
                                      </Tag>
                                    ))}
                                </Button>
                              </Badge>
                            </div>
                          ))}
                        </div>
                      </>
                    )
                  })}
                </div>
              )
            })}
          </div>
        </div>
      )}
      {/* Install model providers */}
      <div>
        <div className='mb-2 flex items-center justify-between'>
          <div className='text-lg font-medium'>未启用</div>
          {/* <a href='#' className='text-xs text-blue-600 hover:underline'>
              Discover more in Marketplace →
            </a> */}
        </div>
        <div className='mb-8 flex items-center'>
          <input
            className='w-72 rounded border px-3 py-2 text-sm focus:ring-2 focus:ring-blue-500 focus:outline-none'
            placeholder='Search'
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
        <div className='grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3'>
          {filteredManifests.map((m) => (
            <div
              key={m.name}
              className='flex cursor-pointer items-center rounded-lg border bg-white p-4 hover:shadow-lg'
              onClick={() => {
                handleManifestClick(m, null)
              }}
            >
              <img
                src={`/assets/images/providers/${m.name}/${m.model.icon_small?.en_US}`}
                alt={m.label?.zh_Hans || m.label?.en_US}
                className='mr-4 h-10 w-10 rounded object-contain'
              />
              <div className='flex-1'>
                <div className='mb-1 text-base font-semibold'>
                  {m.label?.zh_Hans || m.label?.en_US}
                </div>
                <div className='mb-1 text-xs text-gray-500'>
                  <div className='line-clamp-3 wrap-anywhere'>
                    {m.description?.zh_Hans || m.description?.en_US}
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
      {modalProviderOpen && (
        <ModalProvider
          selectedManifest={selectedManifest}
          selectedProvider={selectedProvider}
          handleCancel={handleCancel}
          handleSubmit={handleProviderSubmit}
          form={form}
        />
      )}
      {modalModelOpen && (
        <ModalModel
          selectedManifest={selectedManifest}
          selectedModel={selectedModel}
          handleCancel={handleCancel}
          handleSubmit={handleModelSubmit}
          form={form}
        />
      )}
      {defaultModelModalOpen && (
        <ModalDefaults
          handleClose={(refresh) => {
            setDefaultModelModalOpen(false)
            if (refresh) {
              refetch()
            }
          }}
          setTaskId={setTaskId}
        />
      )}
    </div>
  )
}

export default ConfigsModelsApp
