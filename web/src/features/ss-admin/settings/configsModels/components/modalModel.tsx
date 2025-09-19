import { useState, useEffect } from 'react'
import {
  getModelSkillDisplayName,
  getModelTypeDisplayName,
  ModelType,
  ModelSkill,
} from '@/enums'
import {
  getModelLabel,
  ManifestModelJson,
  Model,
  ProviderManifest,
} from '@/models'
import {
  FormInstance,
  Modal,
  Form,
  Input,
  Select,
  Radio,
  Button,
  RadioChangeEvent,
  Checkbox,
} from 'antd'

interface ModalModelProps {
  selectedManifest: ProviderManifest | undefined
  selectedModel: Model | null
  handleCancel: () => void
  handleSubmit: (values: Record<string, unknown>) => void
  form: FormInstance
}
const ModalModel: React.FC<ModalModelProps> = ({
  selectedManifest,
  selectedModel,
  handleCancel,
  handleSubmit,
  form,
}) => {
  const [predefined, setPredefined] = useState(true)
  const [predefinedModels, setPredefinedModels] = useState<ManifestModelJson[]>(
    []
  )

  useEffect(() => {
    if (selectedModel) {
      form.setFieldsValue({
        predefined: false,
      })
      setPredefined(false)
    } else {
      const modelTypes = selectedManifest?.model.supported_model_types
      if (modelTypes) {
        const modelType = modelTypes[0]
        form.setFieldsValue({
          modelType: modelType,
        })
        if (selectedManifest?.model?.models) {
          const models = selectedManifest.model.models.filter(
            (model) => model.model_type === modelType
          )
          setPredefinedModels(models || [])
          const predefined = models.length > 0
          form.setFieldsValue({
            predefined: predefined,
          })
          setPredefined(predefined)
        }
      }
    }
  }, []) // eslint-disable-line react-hooks/exhaustive-deps

  const handleModelTypeChange = (e: RadioChangeEvent) => {
    const modelType = e.target.value
    const models = selectedManifest?.model?.models.filter(
      (model) => model.model_type === modelType
    )
    setPredefinedModels(models || [])
  }

  const handleFormSubmit = (values: Record<string, unknown>) => {
    let valuesToSubmit = values
    if (values.predefined) {
      const modelId = values.modelId as string
      const model = predefinedModels.find((model) => model.model === modelId)
      if (model && model.model_properties) {
        valuesToSubmit = {
          ...values,
          ...model.model_properties,
          skills: model.skills,
        }
      }
    }
    handleSubmit(valuesToSubmit)
  }

  return (
    <Modal
      open={true}
      width={700}
      title={`${selectedModel ? '设置' : '添加'}模型（${selectedManifest?.label?.zh_Hans || selectedManifest?.label?.en_US || ''}）`}
      onCancel={handleCancel}
      footer={null}
      destroyOnHidden
    >
      <Form form={form} layout='vertical' onFinish={handleFormSubmit}>
        {!selectedModel && (
          <>
            <Form.Item
              name='modelType'
              label='模型类型'
              rules={[{ required: true, message: '请选择模型类型' }]}
            >
              <Radio.Group
                block
                onChange={handleModelTypeChange}
                optionType='button'
                buttonStyle='solid'
              >
                {selectedManifest?.model.supported_model_types?.map((type) => (
                  <Radio key={type} value={type}>
                    {getModelTypeDisplayName(type)}
                  </Radio>
                ))}
              </Radio.Group>
            </Form.Item>
            {predefinedModels.length > 0 && (
              <Form.Item name='predefined' label='添加方式'>
                <Radio.Group
                  block
                  options={[
                    { label: '系统预置', value: true },
                    { label: '自定义', value: false },
                  ]}
                  optionType='button'
                  buttonStyle='solid'
                  onChange={(e) => {
                    setPredefined(e.target.value)
                  }}
                />
              </Form.Item>
            )}
          </>
        )}

        {predefined && predefinedModels.length > 0 ? (
          <Form.Item
            name='modelId'
            label='模型名称'
            rules={[{ required: true, message: '请输入模型名称' }]}
          >
            <Radio.Group
              block
              optionType='button'
              buttonStyle='solid'
              style={{
                display: 'grid',
                gridTemplateColumns: '1fr 1fr',
                gap: 8,
                borderRadius: 0,
              }}
              options={predefinedModels
                .filter((x) => !x.deprecated)
                .sort((a, b) => {
                  return (a.order || 999) - (b.order || 999)
                })
                .map((model) => ({
                  label: getModelLabel(model),
                  value: model.model,
                }))}
            />
          </Form.Item>
        ) : (
          <>
            <Form.Item
              name='modelId'
              label='模型名称'
              rules={[{ required: true, message: '请输入模型名称' }]}
            >
              <Input placeholder='请输入模型名称' />
            </Form.Item>
            {form.getFieldValue('modelType') == ModelType.LLM && (
              <Form.Item name='skills' label='模型技能'>
                <Checkbox.Group
                  options={Object.values(ModelSkill).map((skill) => ({
                    label: getModelSkillDisplayName(skill),
                    value: skill,
                  }))}
                />
              </Form.Item>
            )}

            {selectedManifest?.model.model_credential_schema?.credential_form_schemas
              .filter(
                (schema) =>
                  schema.model_types &&
                  schema.model_types.includes(form.getFieldValue('modelType'))
              )
              .map((schema) => (
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
              ))}
          </>
        )}

        <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8 }}>
          <Button onClick={handleCancel}>取 消</Button>
          <Button type='primary' htmlType='submit'>
            确 定
          </Button>
        </div>
      </Form>
    </Modal>
  )
}

export default ModalModel
