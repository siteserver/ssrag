import { useEffect, useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import api from '@/api'
import { SiteTemplate } from '@/dto'
import { getSiteTypeName } from '@/enums'
import { SiteType } from '@/enums'
import { SiteSummary } from '@/models'
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons'
import {
  Input,
  Modal,
  Form,
  Upload,
  App as AntdApp,
  Segmented,
  Radio,
  Spin,
  Table,
} from 'antd'
import { UploadChangeParam, UploadFile } from 'antd/es/upload'
import appsApi from '@/api/admin/apps/appsApi'
import { uuid } from '@/utils/strings'
import SiteIcon from '@/components/SiteIcon'

const { TextArea } = Input

interface FileList {
  uid: string
  name: string
  status: string
  url: string
}

enum CreateType {
  CreateEmpty = 'CreateEmpty',
  UseTemplate = 'UseTemplate',
  ImportTemplate = 'ImportTemplate',
}

const DatasetForm: React.FC<{
  site: SiteSummary | null
  rootSiteId: number
  onClose: () => void
}> = ({ site, rootSiteId, onClose }) => {
  const { message } = AntdApp.useApp()
  const [fileSiteTemplateList, setFileSiteTemplateList] = useState<FileList[]>(
    []
  )
  const [fileImageUrlList, setFileImageUrlList] = useState<FileList[]>([])
  const [root, setRoot] = useState(false)
  const [form] = Form.useForm()
  const [siteType, setSiteType] = useState(SiteType.Web)
  const [createType, setCreateType] = useState(CreateType.CreateEmpty)
  const [siteTemplates, setSiteTemplates] = useState<SiteTemplate[]>([])
  const [loading, setLoading] = useState(false)

  const { isPending } = useQuery({
    queryKey: ['ss-admin', 'apps', 'templates', siteType],
    queryFn: async () => {
      const res = await appsApi.getTemplates({ siteType })
      if (res) {
        setSiteTemplates(res.siteTemplates)
      }
      return res
    },
  })

  useEffect(() => {
    if (site) {
      setRoot(site.root)
      form.setFieldsValue({
        siteId: site.id,
        siteName: site.siteName,
        description: site.description,
        iconUrl: site.iconUrl,
        siteDir: site.siteDir,
      })
      const newFileList = []
      if (site.iconUrl) {
        newFileList.push({
          uid: '-1',
          name: '图标',
          status: 'done',
          url: site.iconUrl,
        })
      }
      setFileImageUrlList(newFileList)
    } else {
      setSiteType(SiteType.Web)
      form.setFieldsValue({
        siteId: 0,
        siteName: '',
        description: '',
        siteDir: '',
        iconUrl: '',
      })
      setFileImageUrlList([])
    }
  }, [site, form])

  const apiSubmit = async (
    siteName: string,
    description?: string,
    iconUrl?: string,
    siteDir?: string,
    siteTemplate?: string
  ) => {
    const siteId = site?.id ?? 0
    setLoading(true)
    if (siteId === 0) {
      const body: Record<string, string> = {
        uuid: uuid(),
        siteType,
        siteTemplate: siteTemplate ?? '',
        siteName: siteName ?? '',
        description: description ?? '',
        iconUrl: iconUrl ?? '',
        root: root.toString(),
        siteDir: siteDir ?? '',
        createType,
      }
      location.href = `/ss-admin/apps/create/?${new URLSearchParams(body).toString()}`
    } else {
      const res = await appsApi.submitCore({
        siteId,
        siteType,
        siteName,
        description,
        iconUrl,
        siteDir,
        root,
      })
      if (res) {
        message.open({
          type: 'success',
          content: '保存成功',
        })
        onClose()
        setTimeout(() => {
          const top = window.top
          if (top) {
            top.location.reload()
          }
        }, 300)
      }
      setLoading(false)
    }
  }

  const handleUploadSiteTemplateChange = (info: UploadChangeParam) => {
    if (info.file.status === 'removed') {
      form.setFieldsValue({ siteTemplate: '' })
      setFileSiteTemplateList([])
    } else {
      let newFileList = [...info.fileList]
      newFileList = newFileList.slice(-1)
      newFileList = newFileList.map((file) => {
        if (file.response) {
          file.url = file.response.value
          form.setFieldsValue({ siteTemplate: info.file.response.value })
        }
        return file
      })
      setFileSiteTemplateList(newFileList as FileList[])
    }
  }

  const handleUploadImageUrlChange = (info: UploadChangeParam) => {
    if (info.file.status === 'removed') {
      form.setFieldsValue({ iconUrl: '' })
      setFileImageUrlList([])
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
      setFileImageUrlList(newFileList as FileList[])
    }
  }

  const handleModalSubmit = async () => {
    try {
      const values = await form.validateFields()
      await apiSubmit(
        values.siteName,
        values.description,
        values.iconUrl,
        values.siteDir,
        values.siteTemplate
      )
    } catch {
      message.open({
        type: 'error',
        content: '请检查表单输入',
      })
    }
  }

  return (
    <Modal
      title={site ? '编辑' + getSiteTypeName(site.siteType) : '新增知识库'}
      open={true}
      onOk={handleModalSubmit}
      confirmLoading={loading}
      okButtonProps={{ loading }}
      width={800}
      onCancel={() => onClose()}
    >
      <Spin
        spinning={isPending}
        indicator={
          <LoadingOutlined
            style={{
              fontSize: 48,
            }}
            spin
          />
        }
        fullscreen
      />
      <Form form={form} layout='vertical' style={{ paddingTop: 10 }}>
        {!site && (
          <Form.Item label='知识库类型'>
            <Segmented
              block
              value={siteType}
              onChange={(value) => setSiteType(value)}
              options={[
                {
                  label: (
                    <div
                      style={{
                        padding: 10,
                      }}
                    >
                      <SiteIcon type={SiteType.Web} className='node-add-icon' />
                      <span style={{ marginLeft: 10 }}>
                        {getSiteTypeName(SiteType.Web)}
                      </span>
                      <div style={{ color: '#999' }}>基于CMS的网站知识库</div>
                    </div>
                  ),
                  value: SiteType.Web,
                },
                {
                  label: (
                    <div
                      style={{
                        padding: 10,
                      }}
                    >
                      <SiteIcon
                        type={SiteType.Document}
                        className='node-add-icon'
                      />
                      <span style={{ marginLeft: 10 }}>
                        {getSiteTypeName(SiteType.Document)}
                      </span>
                      <div style={{ color: '#999' }}>基于文档的知识库</div>
                    </div>
                  ),
                  value: SiteType.Document,
                },
                // {
                //   label: (
                //     <div
                //       style={{
                //         padding: 10,
                //       }}
                //     >
                //       <SiteIcon
                //         type={SiteType.Markdown}
                //         className='node-add-icon'
                //       />
                //       <span style={{ marginLeft: 10 }}>
                //         {getSiteTypeName(SiteType.Markdown)}
                //       </span>
                //       <div style={{ color: '#999' }}>基于Markdown的知识库</div>
                //     </div>
                //   ),
                //   disabled: true,
                //   value: SiteType.Markdown,
                // },
              ]}
            />
          </Form.Item>
        )}

        {!site && (
          <>
            <Form.Item label='创建方式'>
              <Radio.Group
                buttonStyle='solid'
                value={createType}
                onChange={(e) => setCreateType(e.target.value)}
              >
                <Radio.Button value={CreateType.CreateEmpty}>
                  创建空知识库
                </Radio.Button>
                {siteTemplates.length > 0 && (
                  <Radio.Button value={CreateType.UseTemplate}>
                    使用模板创建知识库
                  </Radio.Button>
                )}
                <Radio.Button value={CreateType.ImportTemplate}>
                  导入模板创建知识库
                </Radio.Button>
              </Radio.Group>
            </Form.Item>

            {createType === CreateType.UseTemplate && (
              <Form.Item
                label='选择模板'
                name='siteTemplate'
                rules={[{ required: true, message: '请选择知识库模板' }]}
              >
                <Table
                  dataSource={siteTemplates}
                  rowKey='directoryName'
                  pagination={false}
                  style={{ width: '100%' }}
                  size='small'
                  rowSelection={{
                    type: 'radio',
                    onChange: (_, selectedRows) => {
                      form.setFieldsValue({
                        siteTemplate: selectedRows[0].directoryName,
                      })
                    },
                  }}
                >
                  <Table.Column
                    title='模板名称'
                    dataIndex='siteTemplateName'
                    key='siteTemplateName'
                  />
                  <Table.Column
                    title='模板文件夹'
                    dataIndex='directoryName'
                    key='directoryName'
                  />
                  <Table.Column
                    title='模板介绍'
                    dataIndex='description'
                    key='description'
                  />
                </Table>
              </Form.Item>
            )}

            {createType === CreateType.ImportTemplate && (
              <>
                <Form.Item
                  label='知识库模板'
                  name='siteTemplate'
                  rules={[{ required: true, message: '请导入知识库模板' }]}
                >
                  <Upload.Dragger
                    accept='.zip'
                    action={appsApi.getUploadTemplateUrl()}
                    name='file'
                    headers={api.getHeaders()}
                    fileList={fileSiteTemplateList as UploadFile[]}
                    maxCount={1}
                    onChange={handleUploadSiteTemplateChange}
                    showUploadList={{ showPreviewIcon: false }}
                    style={{ padding: '12px 0' }}
                  >
                    <p className='ant-upload-drag-icon'>
                      <UploadOutlined />
                    </p>
                    <p className='ant-upload-text'>点击或拖拽上传知识库模板</p>
                    <p className='ant-upload-hint'>
                      仅支持zip格式的知识库模板文件
                    </p>
                  </Upload.Dragger>
                </Form.Item>
              </>
            )}
          </>
        )}

        <Form.Item
          label='知识库名称'
          name='siteName'
          rules={[{ required: true, message: '请输入知识库名称' }]}
        >
          <Input
            showCount
            maxLength={20}
            placeholder='给知识库起一个独一无二的名字'
          />
        </Form.Item>

        {!site && (
          <>
            <Form.Item label='文件夹'>
              <Radio.Group
                buttonStyle='solid'
                value={root}
                onChange={(e) => setRoot(e.target.value)}
              >
                <Radio.Button value={true} disabled={rootSiteId > 0}>
                  根目录
                </Radio.Button>
                <Radio.Button value={false}>子目录</Radio.Button>
              </Radio.Group>
            </Form.Item>

            {root === false && (
              <Form.Item
                label='文件夹名称'
                name='siteDir'
                rules={[{ required: true, message: '请输入文件夹名称' }]}
              >
                <Input
                  showCount
                  maxLength={20}
                  placeholder='实际在服务器中保存此知识库的文件夹名称，此路径必须以英文或拼音命名'
                />
              </Form.Item>
            )}
          </>
        )}
        {site && !site.root && (
          <Form.Item
            label='文件夹名称'
            name='siteDir'
            rules={[{ required: true, message: '请输入文件夹名称' }]}
          >
            <Input
              showCount
              maxLength={20}
              placeholder='实际在服务器中保存此知识库的文件夹名称，此路径必须以英文或拼音命名'
            />
          </Form.Item>
        )}

        <Form.Item
          label='知识库介绍'
          name='description'
          rules={[{ required: false }]}
        >
          <TextArea
            rows={4}
            placeholder='知识库的介绍'
            showCount
            maxLength={500}
          />
        </Form.Item>

        <Form.Item label='知识库图标' name='iconUrl'>
          <Upload
            accept='.jpg,.jpeg,.gif,.png,.pneg,.bmp,.webp,.svg,.ico,.jfif'
            action={appsApi.getUploadUrl(site?.id ?? 0)}
            name='file'
            headers={api.getHeaders()}
            listType='picture-card'
            fileList={fileImageUrlList as UploadFile[]}
            maxCount={1}
            onChange={handleUploadImageUrlChange}
          >
            <UploadOutlined />
          </Upload>
        </Form.Item>
      </Form>
    </Modal>
  )
}

export default DatasetForm
