import { useRef, useState, useEffect } from 'react'
import { useQuery } from '@tanstack/react-query'
import { DisplayType, getDisplayTypeName } from '@/enums'
import { getQueryInt } from '@/utils'
import { UploadOutlined } from '@ant-design/icons'
import {
  App as AntdApp,
  Modal,
  Button,
  Switch,
  Form,
  FormInstance,
  Skeleton,
  Divider,
  Segmented,
  Upload,
  Input,
  UploadFile,
} from 'antd'
import { UploadChangeParam } from 'antd/es/upload'
import { Copy } from 'lucide-react'
import publishApi from '@/api/admin/apps/publishApi'
import './app.css'

const siteId = getQueryInt('siteId')

interface FormIconSettings {
  displayType: DisplayType
  isButtonId: boolean
  openIconUrl: string
  closeIconUrl: string
  btnId: string
  isDefaultOpen: boolean
  isBtnDraggable: boolean
  winStyle: string
  btnStyle: string
}

interface FormEmbedSettings {
  displayType: DisplayType
}

const Publish: React.FC = () => {
  const { message } = AntdApp.useApp()
  const [rootUrl, setRootUrl] = useState('')
  const [id, setId] = useState('')
  const [isDialogLink, setIsDialogLink] = useState(false)
  const [isDialogEmbedSettings, setIsDialogEmbedSettings] = useState(false)
  const [isDialogEmbed, setIsDialogEmbed] = useState(false)
  const [isDialogIcon, setIsDialogIcon] = useState(false)
  const [isDialogIconSettings, setIsDialogIconSettings] = useState(false)
  const [formIconSettings, setFormIconSettings] = useState<FormIconSettings>({
    displayType: DisplayType.Copilot,
    isButtonId: false,
    openIconUrl: '',
    closeIconUrl: '',
    btnId: '',
    isDefaultOpen: false,
    isBtnDraggable: false,
    winStyle: '',
    btnStyle: '',
  })
  const formIconSettingsRef = useRef<FormInstance>(null)
  const [formEmbedSettings, setFormEmbedSettings] = useState<FormEmbedSettings>(
    {
      displayType: DisplayType.Chat,
    }
  )
  const formEmbedSettingsRef = useRef<FormInstance>(null)
  const [openFileList, setOpenFileList] = useState<UploadFile[]>([])
  const [closeFileList, setCloseFileList] = useState<UploadFile[]>([])
  const handleOpenUploadChange = (info: UploadChangeParam) => {
    setOpenFileList(info.fileList)
  }
  const handleCloseUploadChange = (info: UploadChangeParam) => {
    setCloseFileList(info.fileList)
  }

  useEffect(() => {}, [formIconSettings, rootUrl])

  const { isPending } = useQuery({
    queryKey: ['publish', siteId],
    queryFn: async () => {
      const res = await publishApi.get({ siteId })
      if (res) {
        const rootUrl = location.protocol + '//' + location.host
        const formIconSettings = {
          displayType: DisplayType.Copilot,
          isButtonId: false,
          openIconUrl: `${rootUrl}/sitefiles/assets/images/apps/chat-open.png`,
          closeIconUrl: `${rootUrl}/sitefiles/assets/images/apps/chat-close.png`,
          btnId: '',
          isDefaultOpen: false,
          isBtnDraggable: false,
          winStyle:
            'bottom: 80px; right: 60px; width: 550px; height: 650px; max-width: 90vw; max-height: 85vh;',
          btnStyle: 'bottom: 30px; right: 60px; width: 40px; height: 40px;',
        }
        setFormIconSettings(formIconSettings)
        setRootUrl(rootUrl)
        setId(res.site.uuid)
        setOpenFileList([
          { url: formIconSettings.openIconUrl, uid: '-1', name: '图标' },
        ])
        setCloseFileList([
          { url: formIconSettings.closeIconUrl, uid: '-1', name: '图标' },
        ])
      }
      return res
    },
  })

  const getExternalUrl = (displayType: DisplayType) => {
    return `${rootUrl}/open/${displayType.toLowerCase()}/?id=${id}`
  }

  const getIframeCode = (displayType: DisplayType) => {
    return `<iframe id="app-${id}" frameborder="0" scrolling="no" src="${getExternalUrl(displayType)}&isIframe=true" style="width: 1px;min-width: 100%;min-height: 200px;"></iframe>
<script type="text/javascript" src="${rootUrl}/sitefiles/assets/lib/iframe-resizer-4.3.6/iframeResizer.min.js"></script>
<script type="text/javascript">iFrameResize({log: false}, '#app-${id}')</script>`
  }

  const getIconCode = () => {
    return `<script
  id="chat-iframe"
  src="${rootUrl}/sitefiles/assets/js/apps/chat-iframe.js"
  defer
  data-default-open="${formIconSettings.isDefaultOpen}"
  data-btn-id="${formIconSettings.btnId || ''}"
  data-btn-open="${formIconSettings.openIconUrl}"
  data-btn-close="${formIconSettings.closeIconUrl}"
  data-btn-draggable="${formIconSettings.isBtnDraggable}"
  data-btn-style="${formIconSettings.btnStyle || ''}"
  data-win-src="${getExternalUrl(formIconSettings.displayType)}"
  data-win-style="${formIconSettings.winStyle}"
></script>`
  }

  const handleCopy = (text: string) => {
    navigator.clipboard.writeText(text)
    message.success('复制成功!')
  }

  const handleIconSettingsSubmit = () => {
    return formIconSettingsRef.current
      ?.validateFields()
      .then(() => {
        const values = formIconSettingsRef.current?.getFieldsValue()
        values.openIconUrl = openFileList[0].url
        values.closeIconUrl = closeFileList[0].url
        setFormIconSettings(values)
        setIsDialogIconSettings(false)
        setIsDialogIcon(true)
      })
      .catch(() => {
        return
      })
  }

  const handleEmbedSettingsSubmit = () => {
    setIsDialogEmbedSettings(false)
    setIsDialogEmbed(true)
  }

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <div className='bot-publish-comp'>
      <div className='bot-publish'>
        <div className='publish-item'>
          <div className='content'>
            <svg
              className='icon'
              viewBox='0 0 1024 1024'
              version='1.1'
              xmlns='http://www.w3.org/2000/svg'
              p-id='11112'
              width='200'
              height='200'
            >
              <path
                d='M507.787 0C231.351 0 7.314 229.127 7.314 511.854c0 282.711 224.037 511.853 500.473 511.853 276.436 0 500.473-229.142 500.473-511.853C1008.26 229.127 784.238 0 507.787 0z'
                fill='#5E7FFF'
                p-id='11113'
              ></path>
              <path
                d='M668.672 500.838a20.114 20.114 0 0 1-28.965-27.75l34.992-36.615a79.053 79.053 0 0 0-7.636-107.608c-30.384-31.583-75.235-34.392-100.572-7.841L468.73 423.6a79.053 79.053 0 0 0 7.65 107.607 20.114 20.114 0 1 1-28.57 28.38 119.077 119.077 0 0 1-7.635-163.153l97.748-101.975c42.042-43.857 113.444-40.23 158.91 8.045a119.077 119.077 0 0 1 7.022 161.31l-35.182 37.01z'
                fill='#FFFFFF'
                p-id='11114'
              ></path>
              <path
                d='M333.97 507.275a20.114 20.114 0 0 1 28.965 27.765l-35.006 36.6a79.053 79.053 0 0 0 7.65 107.608c30.37 31.598 75.22 34.407 100.572 7.856l97.748-102.59a79.053 79.053 0 0 0-7.636-107.608 20.114 20.114 0 1 1 28.555-28.365 119.077 119.077 0 0 1 7.65 163.123l-97.762 101.99c-42.043 43.843-113.445 40.23-158.896-8.045a119.077 119.077 0 0 1-7.05-161.324l35.21-37.01z'
                fill='#FFFFFF'
                p-id='11115'
              ></path>
            </svg>
            <div className='intro'>
              <p className='title'>网页</p>
              <p className='text'>用户在此链接可以直接和AI知识库对话</p>
            </div>
          </div>
          <div className='bottom'>
            <div className='bottom-item'>
              <Button
                color='primary'
                variant='filled'
                onClick={() => setIsDialogLink(true)}
                icon={<Copy />}
              >
                复制链接
              </Button>
            </div>
          </div>
        </div>
        <div className='publish-item'>
          <div className='content'>
            <svg
              className='icon'
              viewBox='0 0 1024 1024'
              version='1.1'
              xmlns='http://www.w3.org/2000/svg'
              p-id='86239'
              width='200'
              height='200'
            >
              <path
                d='M512 1024A512 512 0 1 0 512 0a512 512 0 0 0 0 1024z'
                fill='#35D09C'
                p-id='86240'
              ></path>
              <path
                d='M365.738667 681.415111a24.405333 24.405333 0 0 1-15.928889-6.371555L234.666667 556.828444c-9.557333-9.614222-9.557333-22.357333 0-31.971555l118.215111-118.215111c9.614222-9.557333 22.357333-9.557333 31.971555 0 9.557333 9.614222 9.557333 22.414222 0 31.971555L279.495111 540.842667l102.229333 102.229333c9.557333 9.614222 9.557333 22.357333 0 31.971556a24.405333 24.405333 0 0 1-15.985777 6.371555z m293.944889 0a24.405333 24.405333 0 0 1-15.985778-6.371555c-9.614222-9.614222-9.614222-22.357333 0-31.971556l102.229333-102.229333-102.229333-102.229334c-9.614222-9.557333-9.614222-22.357333 0-31.971555 9.557333-9.557333 22.357333-9.557333 31.971555 0l118.158223 118.215111a24.462222 24.462222 0 0 1 6.428444 15.985778 24.462222 24.462222 0 0 1-6.428444 15.985777l-118.215112 118.215112c-6.371556 3.185778-9.557333 6.371556-15.928888 6.371555zM455.111111 796.444444H448.853333c-12.8-3.185778-19.171556-15.928889-15.928889-25.6l118.158223-469.617777c3.185778-12.8 12.8-19.171556 25.6-15.928889 12.743111 3.128889 19.171556 15.928889 15.928889 25.543111l-118.215112 469.617778c0 9.614222-9.557333 15.985778-19.171555 15.985777z'
                fill='#FFFFFF'
                p-id='86241'
              ></path>
            </svg>
            <div className='intro'>
              <p className='title'>弹窗嵌入</p>
              <p className='text'>
                将对话图标嵌入到网站页面中，点击后显示对话窗口
              </p>
            </div>
          </div>
          <div className='bottom'>
            <div className='bottom-item'>
              <Button
                color='primary'
                variant='filled'
                onClick={() => setIsDialogIconSettings(true)}
                icon={<Copy />}
              >
                设置并复制代码
              </Button>
            </div>
          </div>
        </div>
        <div className='publish-item'>
          <div className='content'>
            <svg
              className='icon'
              viewBox='0 0 1024 1024'
              version='1.1'
              xmlns='http://www.w3.org/2000/svg'
              p-id='70964'
              width='200'
              height='200'
            >
              <path
                d='M64.060207 508.387582c0 247.450611 200.609595 447.939793 447.939793 447.939793 247.450611 0 447.939793-200.609595 447.939793-447.939793 0-247.450611-200.609595-447.939793-447.939793-447.939793-247.330198-0.120414-447.939793 200.489182-447.939793 447.939793z m0 0'
                fill='#08D19F'
                p-id='70965'
              ></path>
              <path
                d='M691.296331 284.417686H332.824083c-24.684854 0-44.793979 20.109125-44.79398 44.793979l0.481656 149.433678c0 16.496707 13.365945 29.742239 29.862653 29.742239h119.450612c41.181562 0 74.536218 33.475071 74.536218 74.656632v119.450612c0 16.496707 13.365945 29.862653 29.862653 29.862653h149.19285c24.684854 0 44.793979-20.109125 44.793979-44.793979V329.211665c-0.120414-24.805268-20.229539-44.793979-44.914393-44.793979z m-31.668862 206.028222c-9.633114-0.963311-16.857949-9.633114-16.857949-19.386642v-48.888052c0-6.622766-8.067733-9.994356-12.763876-5.298213L554.626529 492.492944c-6.863594 6.863594-18.062088 8.067733-25.527752 1.806209-8.669802-7.104421-9.031044-19.868297-1.324553-27.574788l76.101599-76.101599c4.696143-4.696143 1.324553-12.763876-5.298212-12.763876h-48.888053c-10.837253 0-19.507056-9.271872-18.543744-20.349953 0.842897-9.753528 9.753528-16.978363 19.507055-16.978363H665.286924c8.188147 0 14.931326 6.74318 14.931326 14.931326V472.022578c-0.120414 10.716839-9.392286 19.507056-20.590781 18.42333z'
                fill='#FFFFFF'
                p-id='70966'
              ></path>
              <path
                d='M445.170273 545.715898H318.494826c-16.496707 0-29.862653 13.365945-29.862653 29.862653v126.916275c0 16.496707 13.365945 29.862653 29.862653 29.862653h126.795861c16.496707 0 29.862653-13.365945 29.862653-29.862653V575.578551c-0.120414-16.496707-13.486359-29.862653-29.983067-29.862653z m-14.810913 149.313265h-96.933208c-4.094073 0-7.465663-3.37159-7.465663-7.465663v-97.053622c0-4.094073 3.37159-7.465663 7.465663-7.465664h96.933208c4.094073 0 7.465663 3.37159 7.465664 7.465664V687.5635c-0.120414 4.094073-3.37159 7.465663-7.465664 7.465663z'
                fill='#FFFFFF'
                p-id='70967'
              ></path>
            </svg>
            <div className='intro'>
              <p className='title'>页面嵌入</p>
              <p className='text'>将对话窗口直接嵌入到网站页面中</p>
            </div>
          </div>
          <div className='bottom'>
            <div className='bottom-item'>
              <Button
                color='primary'
                variant='filled'
                onClick={() => setIsDialogEmbedSettings(true)}
                icon={<Copy />}
              >
                设置并复制代码
              </Button>
            </div>
          </div>
        </div>
      </div>

      <Modal
        title='复制链接'
        open={isDialogLink}
        onCancel={() => setIsDialogLink(false)}
        width='50%'
        centered
        footer={null}
      >
        <div>
          <h3>独立式</h3>
          <div className='code'>{getExternalUrl(DisplayType.Home)}</div>
          <Button
            size='middle'
            type='primary'
            onClick={() => handleCopy(getExternalUrl(DisplayType.Home))}
          >
            复 制
          </Button>
          <Button
            size='middle'
            type='primary'
            style={{ marginLeft: 10 }}
            onClick={() =>
              window.open(getExternalUrl(DisplayType.Home), '_blank')
            }
          >
            访 问
          </Button>
          <Divider />
          <h3>嵌入式</h3>
          <div className='code'>{getExternalUrl(DisplayType.Chat)}</div>
          <Button
            size='middle'
            type='primary'
            onClick={() => handleCopy(getExternalUrl(DisplayType.Chat))}
          >
            复 制
          </Button>
          <Button
            size='middle'
            type='primary'
            style={{ marginLeft: 10 }}
            onClick={() =>
              window.open(getExternalUrl(DisplayType.Chat), '_blank')
            }
          >
            访 问
          </Button>
          <Divider />
          <h3>助手式</h3>
          <div className='code'>{getExternalUrl(DisplayType.Copilot)}</div>
          <Button
            size='middle'
            type='primary'
            onClick={() => handleCopy(getExternalUrl(DisplayType.Copilot))}
          >
            复 制
          </Button>
          <Button
            size='middle'
            type='primary'
            style={{ marginLeft: 10 }}
            onClick={() =>
              window.open(getExternalUrl(DisplayType.Copilot), '_blank')
            }
          >
            访 问
          </Button>
        </div>
      </Modal>

      <Modal
        title='复制代码'
        open={isDialogEmbed}
        onCancel={() => setIsDialogEmbed(false)}
        width='50%'
        footer={null}
      >
        <h3>网页嵌入代码</h3>
        <div className='tips'>将以下代码放到HTML代码中</div>
        <div className='code'>
          {getIframeCode(formEmbedSettings.displayType)}
        </div>
        <Button
          size='middle'
          type='primary'
          onClick={() =>
            handleCopy(getIframeCode(formEmbedSettings.displayType))
          }
        >
          复 制
        </Button>
      </Modal>

      <Modal
        title='复制代码'
        open={isDialogIcon}
        onCancel={() => setIsDialogIcon(false)}
        width='50%'
        footer={null}
      >
        <h3>网页嵌入代码</h3>
        <div className='tips'>将以下代码放到HTML代码中</div>
        <div className='code'>{getIconCode()}</div>
        <Button
          size='middle'
          type='primary'
          onClick={() => handleCopy(getIconCode())}
        >
          复 制
        </Button>
      </Modal>

      <Modal
        title='弹窗嵌入设置'
        open={isDialogIconSettings}
        onCancel={() => setIsDialogIconSettings(false)}
        width={700}
        footer={[
          <Button key='cancel' onClick={() => setIsDialogIconSettings(false)}>
            取 消
          </Button>,
          <Button
            key='submit'
            type='primary'
            onClick={handleIconSettingsSubmit}
          >
            确 定
          </Button>,
        ]}
      >
        <Form
          style={{ paddingTop: 10 }}
          ref={formIconSettingsRef}
          labelCol={{ span: 4 }}
          wrapperCol={{ span: 16 }}
          initialValues={formIconSettings}
        >
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
                setFormIconSettings({
                  ...formIconSettings,
                  displayType: value,
                })
              }}
            />
          </Form.Item>
          <Form.Item name='isButtonId' label='弹窗按钮' tooltip='设置弹窗按钮'>
            <Segmented
              options={[
                {
                  label: '自定义图标',
                  value: false,
                },
                {
                  label: '指定页面元素Id',
                  value: true,
                },
              ]}
              onChange={(value) => {
                setFormIconSettings({
                  ...formIconSettings,
                  isButtonId: value,
                })
              }}
            />
          </Form.Item>
          {formIconSettings.isButtonId && (
            <Form.Item
              name='buttonId'
              label='页面元素Id'
              tooltip='设置页面元素Id'
              rules={[{ required: true, message: '请输入页面元素Id' }]}
            >
              <Input />
            </Form.Item>
          )}
          {!formIconSettings.isButtonId && (
            <>
              <Form.Item
                name='openIconUrl'
                label='打开图标'
                tooltip='显示在弹窗打开的图标'
                rules={[{ required: true, message: '请上传打开图标' }]}
              >
                <Upload
                  accept='.jpg,.jpeg,.gif,.png,.pneg,.bmp,.webp,.svg,.ico,.jfif'
                  action={publishApi.getUploadUrl(siteId)}
                  name='file'
                  headers={publishApi.getUploadHeaders()}
                  listType='picture-card'
                  fileList={openFileList}
                  maxCount={1}
                  onChange={handleOpenUploadChange}
                >
                  <UploadOutlined />
                </Upload>
              </Form.Item>
              <Form.Item
                name='closeIconUrl'
                label='关闭图标'
                tooltip='显示在弹窗关闭的图标'
                rules={[{ required: true, message: '请上传关闭图标' }]}
              >
                <Upload
                  accept='.jpg,.jpeg,.gif,.png,.pneg,.bmp,.webp,.svg,.ico,.jfif'
                  action={publishApi.getUploadUrl(siteId)}
                  name='file'
                  headers={publishApi.getUploadHeaders()}
                  listType='picture-card'
                  fileList={closeFileList}
                  maxCount={1}
                  onChange={handleCloseUploadChange}
                >
                  <UploadOutlined />
                </Upload>
              </Form.Item>
              <Form.Item
                name='btnStyle'
                label='按钮样式'
                tooltip='设置按钮样式'
              >
                <Input.TextArea rows={4} />
              </Form.Item>
            </>
          )}
          <Form.Item name='winStyle' label='弹窗样式' tooltip='设置弹窗样式'>
            <Input.TextArea rows={4} />
          </Form.Item>
          <Form.Item label='弹窗打开' name='isDefaultOpen' help='弹窗默认打开'>
            <Switch
              checked={formIconSettings.isDefaultOpen}
              onChange={(checked) =>
                setFormIconSettings({
                  ...formIconSettings,
                  isDefaultOpen: checked,
                })
              }
            />
          </Form.Item>
          <Form.Item label='图标拖拽' name='isBtnDraggable' help='图标可拖拽'>
            <Switch
              checked={formIconSettings.isBtnDraggable}
              onChange={(checked) =>
                setFormIconSettings({
                  ...formIconSettings,
                  isBtnDraggable: checked,
                })
              }
            />
          </Form.Item>
        </Form>
      </Modal>

      <Modal
        title='页面嵌入设置'
        open={isDialogEmbedSettings}
        onCancel={() => setIsDialogEmbedSettings(false)}
        width={500}
        footer={[
          <Button key='cancel' onClick={() => setIsDialogEmbedSettings(false)}>
            取 消
          </Button>,
          <Button
            key='submit'
            type='primary'
            onClick={() => handleEmbedSettingsSubmit()}
          >
            确 定
          </Button>,
        ]}
      >
        <Form
          style={{ paddingTop: 10 }}
          ref={formEmbedSettingsRef}
          layout='vertical'
          initialValues={formEmbedSettings}
          onFinish={handleEmbedSettingsSubmit}
        >
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
                setFormEmbedSettings({
                  ...formEmbedSettings,
                  displayType: value,
                })
              }}
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  )
}

export default Publish
