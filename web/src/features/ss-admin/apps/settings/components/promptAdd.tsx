import React, { useEffect, useState } from 'react'
import { Prompt } from '@/models'
import { UploadOutlined } from '@ant-design/icons'
import { Form, message, Modal, Upload, Input, UploadFile } from 'antd'
import { UploadChangeParam } from 'antd/es/upload'
import { v4 as uuidv4 } from 'uuid'
import settingsApi from '@/api/admin/apps/settingsApi'

const PromptAdd: React.FC<{
  siteId: number
  prompt: Prompt
  onAdd: (prompt: Prompt) => void
  onEdit: (prompt: Prompt) => void
  onClose: () => void
}> = ({ siteId, prompt, onAdd, onEdit, onClose }) => {
  const [fileList, setFileList] = useState<UploadFile[]>([])
  const [form] = Form.useForm()

  useEffect(() => {
    form.setFieldsValue({ ...prompt })
    if (prompt.iconUrl) {
      setFileList([{ url: prompt.iconUrl, uid: '-1', name: '图标' }])
    } else {
      setFileList([])
    }
  }, [prompt, form])

  const handleUploadChange = (info: UploadChangeParam) => {
    setFileList(info.fileList)
  }

  const handleSubmit = async () => {
    const values = await form.validateFields()
    if (
      fileList.length > 0 &&
      fileList[0].response &&
      fileList[0].response.value
    ) {
      values.iconUrl = fileList[0].response.value
      form.setFieldsValue({ iconUrl: values.iconUrl })
    }

    if (prompt.uuid) {
      message.success('提示编辑成功')
      onEdit({
        ...values,
        uuid: prompt.uuid,
      })
    } else {
      message.success('提示添加成功')
      onAdd({
        ...values,
        uuid: uuidv4(),
      })
    }
  }

  return (
    <Modal
      title={prompt.uuid ? '编辑提示' : '添加提示'}
      open={true}
      onCancel={() => onClose()}
      onOk={handleSubmit}
      width='850px'
      style={{ top: 20, bottom: 20 }}
      destroyOnHidden={true}
    >
      <Form layout='vertical' form={form}>
        <Form.Item
          name='iconUrl'
          label='提示图标'
          tooltip='显示在提示列表前侧的图标'
          rules={[{ required: true, message: '请上传提示图标' }]}
        >
          <Upload
            accept='.jpg,.jpeg,.gif,.png,.pneg,.bmp,.webp,.svg,.ico,.jfif'
            action={settingsApi.getUploadUrl(siteId)}
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
          name='title'
          label='提示标题'
          tooltip='显示在提示位置的标题'
          rules={[{ required: true, message: '请输入提示标题' }]}
        >
          <Input placeholder='请输入提示标题' />
        </Form.Item>

        <Form.Item
          name='text'
          label='提示文本'
          tooltip='显示在提示位置的文本'
          rules={[{ required: true, message: '请输入提示文本' }]}
        >
          <Input placeholder='请输入提示文本' />
        </Form.Item>
      </Form>
    </Modal>
  )
}

export default PromptAdd
