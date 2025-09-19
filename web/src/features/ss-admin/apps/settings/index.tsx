import { useEffect, useState } from 'react'
import { useQuery, useQueryClient } from '@tanstack/react-query'
import { settingsDefaults } from '@/config'
import { DisplayType, getDisplayTypeName, SearchType, SiteType } from '@/enums'
import { Prompt, Site, SiteValues } from '@/models'
import {
  PlusOutlined,
  QuestionCircleOutlined,
  UploadOutlined,
} from '@ant-design/icons'
import { DndContext, DragEndEvent, closestCenter } from '@dnd-kit/core'
import { SortableContext, verticalListSortingStrategy } from '@dnd-kit/sortable'
import {
  Row,
  Col,
  Input,
  Tooltip,
  Button,
  Splitter,
  Typography,
  Space,
  Layout,
  Switch,
  Form,
  Collapse,
  Skeleton,
  Upload,
  UploadFile,
  Segmented,
  Modal,
  Radio,
  notification,
} from 'antd'
import { UploadChangeParam } from 'antd/es/upload'
import { Settings2 } from 'lucide-react'
import { v4 as uuidv4 } from 'uuid'
import settingsApi from '@/api/admin/apps/settingsApi'
import Chat from '@/features/open/chat'
import Copilot from '@/features/open/copilot'
import Home from '@/features/open/home'
import ConfigsModelsApp from '@/features/ss-admin/settings/configsModels/configsModelsApp'
import SettingsDataset from '../components/SettingsDataset'
import SettingsSearching from '../components/SettingsSearching'
import './app.css'
import SettingsAI from './components/settingsAI'
import SettingsPrompts from './components/settingsPrompts'
import SortableInput from './components/sortableInput'
import { useStore } from './store'

const { Header, Content } = Layout
const { Text } = Typography

const Settings: React.FC = () => {
  const store = useStore()
  const queryClient = useQueryClient()
  const [form] = Form.useForm()
  const [fileList, setFileList] = useState<UploadFile[]>([])
  const [settings, setSettings] = useState<SiteValues>()
  const [site, setSite] = useState<Site>()
  const [modalConfigsModels, setModalConfigsModels] = useState(false)
  const [displayType, setDisplayType] = useState<DisplayType>(DisplayType.Home)
  const [sessionId, setSessionId] = useState<string | null>(uuidv4())
  const [activeKey, setActiveKey] = useState<string>('styles')
  const [isHotPrompts, setIsHotPrompts] = useState(false)
  const [hotPrompts, setHotPrompts] = useState<Prompt[]>([])
  const [isFunctionPrompts, setIsFunctionPrompts] = useState(false)
  const [functionPrompts, setFunctionPrompts] = useState<Prompt[]>([])
  const [isInputPrompts, setIsInputPrompts] = useState(false)
  const [inputPrompts, setInputPrompts] = useState<Prompt[]>([])

  const { isPending } = useQuery({
    queryKey: ['ss-admin', 'apps', 'settings', store.siteId],
    queryFn: async () => {
      const res = await settingsApi.get({
        siteId: store.siteId,
      })
      if (res) {
        store.init(
          res.site,
          res.values,
          res.models,
          res.defaultModel,
          res.datasetSites,
          res.hotPrompts,
          res.functionPrompts,
          res.inputPrompts
        )
        setSite(res.site)
        setSettings(res.values)
        setDisplayType(res.values.displayType || DisplayType.Home)
        setFileList([
          {
            uid: '-1',
            name: '图标',
            status: 'done',
            url: res.site.iconUrl,
          },
        ])
        setIsHotPrompts(res.values.isHotPrompts)
        setHotPrompts(res.hotPrompts)
        setIsFunctionPrompts(res.values.isFunctionPrompts)
        setFunctionPrompts(res.functionPrompts)
        setIsInputPrompts(res.values.isInputPrompts)
        setInputPrompts(res.inputPrompts)
      }
      return res
    },
  })

  useEffect(() => {
    if (settings && site) {
      // const valuesHotPrompts = hotPrompts.map((prompt: Prompt) => ({
      //   title: prompt.title,
      //   iconUrl: prompt.iconUrl,
      //   text: prompt.text,
      // }))
      // const valuesInputPrompts = inputPrompts.map((prompt: Prompt) => ({
      //   title: prompt.title,
      //   iconUrl: prompt.iconUrl,
      //   text: prompt.text,
      // }))

      form.setFieldsValue({
        providerModelId: settings.providerModelId || '',
        llmSystemPrompt: settings.llmSystemPrompt || '',
        datasetSearchType: settings.datasetSearchType || SearchType.Semantic,
        datasetMaxCount: settings.datasetMaxCount || 5,
        datasetMinScore: settings.datasetMinScore || 0.5,

        displayType: settings.displayType || DisplayType.Home,
        headerText: settings.headerText || settingsDefaults.headerText,
        footerText: settings.footerText || settingsDefaults.footerText,

        welcomeTitle: settings.welcomeTitle || settingsDefaults.welcomeTitle,
        welcomeVariant:
          settings.welcomeVariant || settingsDefaults.welcomeVariant,
        welcomePosition:
          settings.welcomePosition || settingsDefaults.welcomePosition,
        description: site.description || settingsDefaults.description,
        iconUrl: site.iconUrl,

        hotPromptsTitle: settings.hotPromptsTitle || '',
        // hotPrompts: valuesHotPrompts,
        functionPromptsTitle: settings.functionPromptsTitle || '',
        // inputPrompts: valuesInputPrompts,

        senderPlaceholder:
          settings.senderPlaceholder || settingsDefaults.senderPlaceholder,
        senderAllowSpeech: settings.senderAllowSpeech,
      })
    }
  }, [settings, site, form])

  const handleUploadChange = (info: UploadChangeParam) => {
    if (info.file.status === 'removed') {
      form.setFieldsValue({ iconUrl: '' })
      setFileList([])
    } else {
      let newFileList = [...info.fileList]
      newFileList = newFileList.slice(-1)
      newFileList = newFileList.map((file) => {
        if (file.response) {
          file.url = file.response.value
          form.setFieldsValue({ iconUrl: info.file.response.value })
        }
        return file
      })
      setFileList(newFileList as UploadFile[])
    }
  }

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields()

      let res = null
      if (activeKey === 'styles') {
        // let hotPrompts = []
        // if (values.hotPrompts) {
        //   hotPrompts = values.hotPrompts.map((prompt: Prompt) => ({
        //     title: prompt.title,
        //     iconUrl: prompt.iconUrl,
        //     text: prompt.text,
        //   }))
        // }

        // let inputPrompts = []
        // if (values.inputPrompts) {
        //   inputPrompts = values.inputPrompts.map((prompt: Prompt) => ({
        //     title: prompt.title,
        //     iconUrl: prompt.iconUrl,
        //     text: prompt.text,
        //   }))
        // }

        res = await settingsApi.styles({
          siteId: store.siteId,

          displayType: values.displayType,
          headerText: values.headerText,
          footerText: values.footerText,
          welcomeTitle: values.welcomeTitle,
          welcomeVariant: values.welcomeVariant,
          welcomePosition: values.welcomePosition,
          iconUrl: values.iconUrl,
          description: values.description,

          isHotPrompts: isHotPrompts,
          hotPromptsTitle: values.hotPromptsTitle,
          hotPrompts: hotPrompts,
          isFunctionPrompts: isFunctionPrompts,
          functionPromptsTitle: values.functionPromptsTitle,
          functionPrompts: functionPrompts,
          isInputPrompts: isInputPrompts,
          inputPrompts: inputPrompts,

          senderPlaceholder: values.senderPlaceholder,
          senderAllowSpeech: values.senderAllowSpeech,
        })
      } else if (activeKey === 'ai') {
        res = await settingsApi.ai({
          siteId: store.siteId,
          providerModelId: values.providerModelId,
          llmSystemPrompt: values.llmSystemPrompt,
          datasetSearchType: values.datasetSearchType,
          datasetMaxCount: values.datasetMaxCount,
          datasetMinScore: values.datasetMinScore,
        })
      }

      if (res) {
        notification.success({
          message: '保存成功',
        })
        queryClient.invalidateQueries({ queryKey: ['chat', site?.uuid] })
        setSessionId(uuidv4())
      }
    } catch {
      notification.error({
        message: '保存失败',
        description: '请检查表单输入',
      })
    }
  }

  const handleHotPromptsDragEnd = (event: DragEndEvent) => {
    const { active, over } = event
    if (!over || active.id === over.id) return
    const oldIndex = Number(active.id)
    const newIndex = Number(over.id)
    if (
      isNaN(oldIndex) ||
      isNaN(newIndex) ||
      oldIndex < 0 ||
      newIndex < 0 ||
      oldIndex >= hotPrompts.length ||
      newIndex >= hotPrompts.length
    ) {
      return
    }
    // 使用 uuid 作为唯一标识，避免 React 还原顺序
    const newPrompts = [...hotPrompts]
    const [movedItem] = newPrompts.splice(oldIndex, 1)
    newPrompts.splice(newIndex, 0, movedItem)
    setHotPrompts([...newPrompts])
  }

  const handleInputPromptsDragEnd = (event: DragEndEvent) => {
    const { active, over } = event
    if (!over || active.id === over.id) return
    const oldIndex = Number(active.id)
    const newIndex = Number(over.id)
    if (
      isNaN(oldIndex) ||
      isNaN(newIndex) ||
      oldIndex < 0 ||
      newIndex < 0 ||
      oldIndex >= inputPrompts.length ||
      newIndex >= inputPrompts.length
    ) {
      return
    }
    // 使用 uuid 作为唯一标识，避免 React 还原顺序
    const newPrompts = [...inputPrompts]
    const [movedItem] = newPrompts.splice(oldIndex, 1)
    newPrompts.splice(newIndex, 0, movedItem)
    setInputPrompts([...newPrompts])
  }

  const stylesItems = [
    {
      key: '1',
      forceRender: true,
      label: (
        <>
          <Text type='secondary' strong>
            显示设置
          </Text>
          <Tooltip title='设置应用的显示样式'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </>
      ),
      children: (
        <Row>
          <Col span={24}>
            <Form.Item
              name='displayType'
              label='显示方式'
              tooltip='设置应用的显示方式'
            >
              <Segmented
                options={[
                  {
                    label: getDisplayTypeName(DisplayType.Home),
                    value: DisplayType.Home,
                  },
                  {
                    label: getDisplayTypeName(DisplayType.Chat),
                    value: DisplayType.Chat,
                  },
                  {
                    label: getDisplayTypeName(DisplayType.Copilot),
                    value: DisplayType.Copilot,
                  },
                ]}
                onChange={(value) => {
                  setDisplayType(value)
                  form.setFieldsValue({ displayType: value })
                }}
              />
            </Form.Item>
            {displayType !== DisplayType.Chat && (
              <Form.Item
                name='headerText'
                label='头部文字'
                tooltip='显示在助手窗口头部的文字'
                rules={[{ required: true, message: '请输入头部文字' }]}
              >
                <Input placeholder='请输入头部文字' />
              </Form.Item>
            )}
            <Form.Item
              name='footerText'
              label='底部文字'
              tooltip='显示在列表底部的文字'
            >
              <Input placeholder='请输入底部文字' />
            </Form.Item>
          </Col>
        </Row>
      ),
    },
    {
      key: '2',
      forceRender: true,
      label: (
        <>
          <Text type='secondary' strong>
            欢迎语设置
          </Text>
          <Tooltip title='设置应用的欢迎语'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </>
      ),
      children: (
        <Row>
          <Col span={24}>
            <Form.Item
              name='iconUrl'
              label='欢迎语图标'
              tooltip='显示在提示列表前侧的图标'
            >
              <Upload
                accept='.jpg,.jpeg,.gif,.png,.pneg,.bmp,.webp,.svg,.ico,.jfif'
                action={settingsApi.getUploadUrl(store.siteId)}
                name='file'
                headers={settingsApi.getUploadHeaders()}
                listType='picture-card'
                fileList={fileList}
                maxCount={1}
                onChange={handleUploadChange}
              >
                <UploadOutlined />
              </Upload>
            </Form.Item>
            <Form.Item
              name='welcomeTitle'
              label='欢迎语标题'
              tooltip='显示在欢迎语位置的标题'
              rules={[{ required: true, message: '请输入欢迎语标题' }]}
            >
              <Input placeholder='请输入欢迎语标题' />
            </Form.Item>

            <Form.Item
              name='description'
              label='欢迎语描述'
              tooltip='显示在欢迎语位置的描述'
              rules={[{ required: true, message: '请输入欢迎语描述' }]}
            >
              <Input placeholder='请输入欢迎语描述' />
            </Form.Item>
            <Form.Item
              name='welcomeVariant'
              label='欢迎语样式'
              tooltip='设置欢迎语样式变体'
            >
              <Segmented
                options={[
                  {
                    label: '无边框',
                    value: 'borderless',
                  },
                  {
                    label: '填充',
                    value: 'filled',
                  },
                ]}
                onChange={(value) => {
                  form.setFieldsValue({ welcomeVariant: value })
                }}
              />
            </Form.Item>
            <Form.Item
              name='welcomePosition'
              label='欢迎语显示位置'
              tooltip='设置欢迎语的显示位置'
            >
              <Segmented
                options={[
                  {
                    label: '顶部',
                    value: 'top',
                  },
                  {
                    label: '居中',
                    value: 'center',
                  },
                ]}
                onChange={(value) => {
                  form.setFieldsValue({ welcomePosition: value })
                }}
              />
            </Form.Item>
          </Col>
        </Row>
      ),
    },
    {
      key: '3',
      forceRender: true,
      label: (
        <>
          <Text type='secondary' strong>
            热点话题设置
          </Text>
          <Tooltip title='用户可选择的热点话题'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </>
      ),
      children: (
        <Row>
          <Col span={24}>
            <Form.Item label='是否显示热点话题' tooltip='设置是否显示热点话题'>
              <Segmented
                value={isHotPrompts}
                options={[
                  {
                    label: '显示',
                    value: true,
                  },
                  {
                    label: '不显示',
                    value: false,
                  },
                ]}
                onChange={(value) => {
                  setIsHotPrompts(value)
                }}
              />
            </Form.Item>
            {isHotPrompts && (
              <>
                <Form.Item
                  name='hotPromptsTitle'
                  label='热点话题标题'
                  tooltip='显示在热点话题位置的标题'
                >
                  <Input placeholder='请输入热点话题标题' />
                </Form.Item>
                <Form.Item label='热点话题' tooltip='设置热点话题'>
                  <DndContext
                    collisionDetection={closestCenter}
                    onDragEnd={handleHotPromptsDragEnd}
                  >
                    <SortableContext
                      items={hotPrompts.map((_, i) => i)}
                      strategy={verticalListSortingStrategy}
                    >
                      {hotPrompts.map((_, index) => (
                        <SortableInput
                          key={hotPrompts[index].uuid}
                          index={index}
                          text={hotPrompts[index].text}
                          onBlur={(text) => {
                            if (text === hotPrompts[index].text) return
                            const newPrompts = hotPrompts.map((item, i) =>
                              i === index ? { ...item, text } : item
                            )
                            setHotPrompts(newPrompts)
                          }}
                          onDelete={(i) => {
                            const newPrompts = hotPrompts.filter(
                              (_, j) => j !== i
                            )
                            setHotPrompts(newPrompts)
                          }}
                        />
                      ))}
                    </SortableContext>
                  </DndContext>
                  <Button
                    block
                    color='primary'
                    variant='dashed'
                    icon={
                      <PlusOutlined style={{ verticalAlign: '-0.125em' }} />
                    }
                    onClick={() => {
                      // const values = form.getFieldValue('hotPrompts') || []
                      // const hotPrompts = values.map((prompt: Prompt) => ({
                      //   title: prompt.title,
                      //   iconUrl: prompt.iconUrl,
                      //   text: prompt.text,
                      // }))
                      // hotPrompts.push({
                      //   uuid: '',
                      //   title: '',
                      //   iconUrl: '',
                      //   text: '',
                      // })
                      setHotPrompts([
                        ...hotPrompts,
                        {
                          uuid: uuidv4(),
                          title: '',
                          iconUrl: '',
                          text: '',
                        },
                      ])
                    }}
                  >
                    添加热点话题
                  </Button>
                </Form.Item>
              </>
            )}
          </Col>
        </Row>
      ),
    },
    {
      key: '4',
      forceRender: true,
      label: (
        <>
          <Text type='secondary' strong>
            功能提示设置
          </Text>
          <Tooltip title='用户可选择的功能提示'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </>
      ),
      children: (
        <Row>
          <Col span={24}>
            <Form.Item label='是否显示功能提示' tooltip='设置是否显示功能提示'>
              <Segmented
                value={isFunctionPrompts}
                options={[
                  {
                    label: '显示',
                    value: true,
                  },
                  {
                    label: '不显示',
                    value: false,
                  },
                ]}
                onChange={(value) => {
                  setIsFunctionPrompts(value)
                }}
              />
            </Form.Item>
            {isFunctionPrompts && (
              <>
                <Form.Item
                  name='functionPromptsTitle'
                  label='功能提示标题'
                  tooltip='显示在功能提示位置的标题'
                >
                  <Input placeholder='请输入功能提示标题' />
                </Form.Item>
                <Form.Item
                  name='functionPrompts'
                  label='功能提示'
                  tooltip='设置功能提示'
                >
                  <SettingsPrompts
                    siteId={store.siteId}
                    prompts={functionPrompts}
                    onChange={(prompts) => {
                      setFunctionPrompts(prompts)
                    }}
                  />
                </Form.Item>
              </>
            )}
          </Col>
        </Row>
      ),
    },
    {
      key: '5',
      forceRender: true,
      label: (
        <>
          <Text type='secondary' strong>
            输入框提示设置
          </Text>
          <Tooltip title='用户可选择的输入框提示'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </>
      ),
      children: (
        <Row>
          <Col span={24}>
            <Form.Item
              label='是否显示输入框提示'
              tooltip='设置是否显示输入框提示'
            >
              <Segmented
                value={isInputPrompts}
                options={[
                  {
                    label: '显示',
                    value: true,
                  },
                  {
                    label: '不显示',
                    value: false,
                  },
                ]}
                onChange={(value) => {
                  setIsInputPrompts(value)
                }}
              />
            </Form.Item>
            {isInputPrompts && (
              <>
                <Form.Item label='输入框提示' tooltip='设置输入框提示'>
                  <DndContext
                    collisionDetection={closestCenter}
                    onDragEnd={handleInputPromptsDragEnd}
                  >
                    <SortableContext
                      items={inputPrompts.map((_, i) => i)}
                      strategy={verticalListSortingStrategy}
                    >
                      {inputPrompts.map((_, index) => (
                        <SortableInput
                          key={inputPrompts[index].uuid}
                          index={index}
                          text={inputPrompts[index].text}
                          onBlur={(text) => {
                            if (text === inputPrompts[index].text) return
                            const newPrompts = inputPrompts.map((item, i) =>
                              i === index ? { ...item, text } : item
                            )
                            setInputPrompts(newPrompts)
                          }}
                          onDelete={(i) => {
                            const newPrompts = inputPrompts.filter(
                              (_, j) => j !== i
                            )
                            setInputPrompts(newPrompts)
                          }}
                        />
                      ))}
                    </SortableContext>
                  </DndContext>
                  <Button
                    block
                    color='primary'
                    variant='dashed'
                    icon={
                      <PlusOutlined style={{ verticalAlign: '-0.125em' }} />
                    }
                    onClick={() => {
                      // const values = form.getFieldValue('inputPrompts') || []
                      // const inputPrompts = values.map((prompt: Prompt) => ({
                      //   title: prompt.title,
                      //   iconUrl: prompt.iconUrl,
                      //   text: prompt.text,
                      // }))
                      // inputPrompts.push({
                      //   uuid: '',
                      //   title: '',
                      //   iconUrl: '',
                      //   text: '',
                      // })
                      setInputPrompts([
                        ...inputPrompts,
                        {
                          uuid: uuidv4(),
                          title: '',
                          iconUrl: '',
                          text: '',
                        },
                      ])
                    }}
                  >
                    添加提示
                  </Button>
                </Form.Item>
              </>
            )}
          </Col>
        </Row>
      ),
    },
    {
      key: '6',
      forceRender: true,
      label: (
        <>
          <Text type='secondary' strong>
            输入框设置
          </Text>
          <Tooltip title='用于聊天的输入框组件'>
            <QuestionCircleOutlined
              style={{ marginLeft: '8px', color: '#999', cursor: 'pointer' }}
            />
          </Tooltip>
        </>
      ),
      children: (
        <Row>
          <Col span={24}>
            <Form.Item
              name='senderPlaceholder'
              label='占位符'
              tooltip='显示在输入框中的占位符'
              rules={[{ required: true, message: '请输入占位符' }]}
            >
              <Input placeholder='请输入占位符' />
            </Form.Item>
            <Form.Item
              name='senderAllowSpeech'
              label='语音输入'
              tooltip='开启后，用户可以语音输入，需要用户同意麦克风权限'
            >
              <Switch size='small' />
            </Form.Item>
          </Col>
        </Row>
      ),
    },
  ]

  const aiItems = [
    {
      key: '1',
      forceRender: true,
      label: (
        <Row align='middle' justify='space-between'>
          <Col>
            <Row align='middle'>
              <Text type='secondary' strong>
                AI 设置
              </Text>
              <Tooltip title='用户可选择的提示集'>
                <QuestionCircleOutlined
                  style={{
                    marginLeft: '8px',
                    color: '#999',
                    cursor: 'pointer',
                  }}
                />
              </Tooltip>
            </Row>
          </Col>
          <Col>
            <Row
              align='middle'
              onClick={(e) => {
                e.stopPropagation()
              }}
            >
              <Button
                type='link'
                style={{ height: 'auto' }}
                icon={<Settings2 className='h-4 w-4' />}
                onClick={() => {
                  setModalConfigsModels(true)
                }}
              >
                模型设置
              </Button>
            </Row>
          </Col>
        </Row>
      ),
      children: <SettingsAI form={form} />,
    },
    {
      key: '2',
      forceRender: true,
      label: (
        <>
          <Text type='secondary' strong>
            知识库
          </Text>
          <Tooltip title='关联知识库，当用户提问时，会从知识库中检索相关信息'>
            <QuestionCircleOutlined
              style={{
                marginLeft: '8px',
                color: '#999',
                cursor: 'pointer',
              }}
            />
          </Tooltip>
        </>
      ),
      children: (
        <SettingsDataset
          siteId={store.siteId}
          nodeId={site?.uuid || ''}
          datasetSites={store.datasetSites}
          onChange={(datasetSites) => {
            store.updateDatasetSites(datasetSites)
          }}
        />
      ),
    },
    {
      key: '3',
      forceRender: true,
      label: (
        <>
          <Text type='secondary' strong>
            检索设置
          </Text>
          <Tooltip title='知识库检索设置'>
            <QuestionCircleOutlined
              style={{
                marginLeft: '8px',
                color: '#999',
                cursor: 'pointer',
              }}
            />
          </Tooltip>
        </>
      ),
      children: <SettingsSearching />,
    },
  ]

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
          defaultSize='30%'
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
                应用配置
              </Text>
            </div>

            <Space>
              <Button type='primary' onClick={handleSubmit}>
                保 存
              </Button>
            </Space>
          </Header>
          <Content>
            {site?.siteType === SiteType.Chat && (
              <div className='m-2'>
                <Radio.Group
                  block
                  value={activeKey}
                  buttonStyle='solid'
                  onChange={(e) => setActiveKey(e.target.value)}
                >
                  <Radio.Button value='styles'>样式设置</Radio.Button>
                  <Radio.Button value='ai'>AI 设置</Radio.Button>
                </Radio.Group>
              </div>
            )}
            <Collapse
              items={activeKey === 'styles' ? stylesItems : aiItems}
              bordered={false}
              defaultActiveKey={['1']}
              destroyOnHidden={false}
            />
          </Content>
        </Splitter.Panel>
        <Splitter.Panel>
          {displayType === DisplayType.Home && <Home sessionId={sessionId} />}
          {displayType === DisplayType.Chat && <Chat sessionId={sessionId} />}
          {displayType === DisplayType.Copilot && (
            <Copilot sessionId={sessionId} />
          )}
        </Splitter.Panel>
      </Splitter>

      {modalConfigsModels && (
        <Modal
          open={modalConfigsModels}
          onCancel={() => setModalConfigsModels(false)}
          footer={null}
          width='850px'
          style={{ top: 20, bottom: 20 }}
          destroyOnHidden={true}
        >
          <div className='h-full overflow-y-auto'>
            <ConfigsModelsApp />
          </div>
        </Modal>
      )}
    </Form>
  )
}

export default Settings
