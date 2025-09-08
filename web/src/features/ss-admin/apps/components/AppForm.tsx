import { useEffect, useState } from 'react'
import api from '@/api'
import { getSiteTypeName } from '@/enums'
import { SiteType } from '@/enums'
import { SiteSummary } from '@/models'
import { UploadOutlined } from '@ant-design/icons'
import {
  Input,
  Modal,
  Form,
  Upload,
  App as AntdApp,
  Segmented,
  Radio,
} from 'antd'
import { UploadChangeParam, UploadFile } from 'antd/es/upload'
import appsApi from '@/api/admin/apps/appsApi'
import SiteIcon from '@/components/SiteIcon'

const { TextArea } = Input

interface FileList {
  uid: string
  name: string
  status: string
  url: string
}

const AppForm: React.FC<{
  site: SiteSummary | null
  rootSiteId: number
  onClose: () => void
}> = ({ site, rootSiteId, onClose }) => {
  const { message } = AntdApp.useApp()
  const [fileList, setFileList] = useState<FileList[]>([])
  const [root, setRoot] = useState(false)
  const [form] = Form.useForm()
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if (site) {
      setRoot(site.root)
      form.setFieldsValue({
        siteId: site.id,
        siteType: site.siteType,
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
      setFileList(newFileList)
    } else {
      form.setFieldsValue({
        siteId: 0,
        siteType: SiteType.Chat,
        siteName: '',
        description: '',
        siteDir: '',
        iconUrl: '',
      })
      setFileList([])
    }
  }, [site, form])

  const apiSubmit = async (
    siteType: string,
    siteName: string,
    description?: string,
    iconUrl?: string,
    siteDir?: string
  ) => {
    const siteId = site?.id ?? 0
    setLoading(true)
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
      if (siteId === 0) {
        const res2 = await appsApi.init({
          siteId: res.value,
        })
        if (res2) {
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
      } else {
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
    } else {
      setLoading(false)
    }
  }

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
      setFileList(newFileList as FileList[])
    }
  }

  const handleModalSubmit = async () => {
    try {
      const values = await form.validateFields()
      await apiSubmit(
        site?.siteType ?? values.siteType,
        values.siteName,
        values.description,
        values.iconUrl,
        values.siteDir
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
      title={site ? '编辑' + getSiteTypeName(site.siteType) : '新增应用'}
      open={true}
      onOk={handleModalSubmit}
      confirmLoading={loading}
      okButtonProps={{ loading }}
      width={800}
      onCancel={() => onClose()}
    >
      <Form form={form} layout='vertical' style={{ paddingTop: 10 }}>
        {!site && (
          <Form.Item
            label='应用类型'
            name='siteType'
            rules={[{ required: true, message: '请选择应用类型' }]}
          >
            <Segmented
              block
              options={[
                {
                  label: (
                    <div
                      style={{
                        padding: 10,
                      }}
                    >
                      <SiteIcon
                        type={SiteType.Chat}
                        className='node-add-icon'
                      />
                      <span style={{ marginLeft: 10 }}>
                        {getSiteTypeName(SiteType.Chat)}
                      </span>
                      <div style={{ color: '#999' }}>基于知识库的简单应用</div>
                    </div>
                  ),
                  value: SiteType.Chat,
                },
                {
                  label: (
                    <div
                      style={{
                        padding: 10,
                      }}
                    >
                      <SiteIcon
                        type={SiteType.Chatflow}
                        className='node-add-icon'
                      />
                      <span style={{ marginLeft: 10 }}>
                        {getSiteTypeName(SiteType.Chatflow)}
                      </span>
                      <div style={{ color: '#999' }}>
                        基于知识库与AI工作流编排的应用
                      </div>
                    </div>
                  ),
                  value: SiteType.Chatflow,
                },
                // {
                //   label: (
                //     <div
                //       style={{
                //         padding: 10,
                //       }}
                //     >
                //       <SiteIcon
                //         type={SiteType.Agent}
                //         className='node-add-icon'
                //       />
                //       <span style={{ marginLeft: 10 }}>
                //         {getSiteTypeName(SiteType.Agent)}
                //       </span>
                //       <div style={{ color: '#999' }}>
                //         自主处理复杂任务的应用
                //       </div>
                //     </div>
                //   ),
                //   disabled: true,
                //   value: SiteType.Agent,
                // },
              ]}
            />
          </Form.Item>
        )}

        <Form.Item
          label='应用名称'
          name='siteName'
          rules={[{ required: true, message: '请输入应用名称' }]}
        >
          <Input
            showCount
            maxLength={20}
            placeholder='给应用起一个独一无二的名字'
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
                  placeholder='实际在服务器中保存此应用的文件夹名称，此路径必须以英文或拼音命名'
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
          label='应用介绍'
          name='description'
          rules={[{ required: false }]}
        >
          <TextArea
            rows={4}
            placeholder='应用的介绍，将会展示给应用的用户'
            showCount
            maxLength={500}
          />
        </Form.Item>

        <Form.Item label='应用图标' name='iconUrl'>
          <Upload
            accept='.jpg,.jpeg,.gif,.png,.pneg,.bmp,.webp,.svg,.ico,.jfif'
            action={appsApi.getUploadUrl(site?.id ?? 0)}
            name='file'
            headers={api.getHeaders()}
            listType='picture-card'
            fileList={fileList as UploadFile[]}
            maxCount={1}
            onChange={handleUploadChange}
          >
            <UploadOutlined />
          </Upload>
        </Form.Item>
      </Form>
    </Modal>
  )
}

export default AppForm
