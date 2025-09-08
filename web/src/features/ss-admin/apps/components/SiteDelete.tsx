import { useEffect, useState } from 'react'
import { getSiteTypeName } from '@/enums'
import { isAppSite } from '@/enums'
import { SiteSummary } from '@/models'
import { Input, Modal, Form, App as AntdApp, Alert, Typography } from 'antd'
import appsApi from '@/api/admin/apps/appsApi'

const { Text } = Typography

const SiteDelete: React.FC<{
  site: SiteSummary
  onClose: (refresh?: boolean) => void
}> = ({ site, onClose }) => {
  const [loading, setLoading] = useState(false)
  const { message } = AntdApp.useApp()
  const [typeName, setTypeName] = useState('')
  const [form] = Form.useForm()

  useEffect(() => {
    form.setFieldsValue({
      siteDir: '',
    })
    if (isAppSite(site.siteType)) {
      setTypeName('应用')
    } else {
      setTypeName('知识库')
    }
  }, [site, form])

  const apiDelete = async (site: SiteSummary) => {
    setLoading(true)
    const res = await appsApi.deleteAI({
      siteId: site.id,
      siteDir: site.siteDir,
    })
    if (res) {
      const res2 = await appsApi.deleteCore({
        siteId: site.id,
        siteDir: site.siteDir,
      })
      if (res2) {
        message.open({
          type: 'success',
          content: `${getSiteTypeName(site.siteType)}删除成功`,
        })
        setTimeout(() => {
          const top = window.top
          if (top) {
            top.location.reload()
          }
        }, 300)
      }
    }
    setLoading(false)
  }

  const handleModalSubmit = async () => {
    try {
      const values = await form.validateFields()
      if (values.siteDir === site.siteDir) {
        await apiDelete(site)
      } else {
        message.open({
          type: 'error',
          content: '文件夹名称不正确，请重新输入',
        })
        return
      }
    } catch {
      message.open({
        type: 'error',
        content: '请输入需要删除的文件夹名称',
      })
    }
  }

  return (
    <Modal
      title={`删除${getSiteTypeName(site.siteType)}`}
      open={true}
      onOk={handleModalSubmit}
      width={800}
      onCancel={() => onClose(false)}
      confirmLoading={loading}
    >
      <Alert
        message={`此操作将会删除${getSiteTypeName(site.siteType)}，且数据无法恢复，请谨慎操作！`}
        type='error'
        showIcon
        style={{ marginBottom: 10 }}
      />
      <Form form={form} layout='vertical' style={{ paddingTop: 10 }}>
        <Form.Item label={`${typeName}名称`}>
          <Text strong>{site.siteName}</Text>
        </Form.Item>

        <Form.Item label={`${typeName}文件夹`}>
          <Text strong copyable={{ text: site.siteDir }}>
            {site.siteDir}
          </Text>
        </Form.Item>

        <Form.Item
          label='文件夹名称'
          name='siteDir'
          rules={[{ required: true, message: '请输入文件夹名称' }]}
        >
          <Input
            showCount
            maxLength={20}
            placeholder={`请输入需要删除的${typeName}文件夹名称`}
          />
        </Form.Item>
      </Form>
    </Modal>
  )
}

export default SiteDelete
