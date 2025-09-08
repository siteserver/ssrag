import { useQuery } from '@tanstack/react-query'
import { Form, App, Skeleton, Input, Button } from 'antd'
import datasetSettingsApi from '@/api/admin/dataset/datasetSettingsApi'
import { getQueryInt } from '@/utils/query'
import './app.css'

const siteId = getQueryInt('siteId')

function Writer() {
  const { modal, message } = App.useApp()
  const [form] = Form.useForm()

  const { isPending } = useQuery({
    queryKey: ['dataset', 'settings', siteId],
    queryFn: async () => {
      const res = await datasetSettingsApi.get({ siteId })
      if (res) {
        form.setFieldsValue(res.settings)
      }
      return res
    },
  })

  const handleNext = async () => {
    modal.confirm({
      title: '确定修改分段策略吗？',
      content:
        '修改分段策略后，系统将根据原始文档对当前知识库自动重新分段，手动修改的分段内容不会保留。',
      okText: '确定修改',
      cancelText: '取消',
      onOk: async () => {
        const settings = form.getFieldsValue()
        const res = await datasetSettingsApi.submit({
          siteId,
          ...settings,
        })
        if (res) {
          message.success('分段策略修改成功!')
        }
      },
    })
  }

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <div className='writer-container'>
      <h1>
        知识库 <span className='highlight'>AI撰写</span>
      </h1>
      <div className='subtitle'>开启您的创作之旅</div>
      <div className='form-card'>
        <div className='form-group'>
          <div className='label'>
            <span className='bar' /> 文章标题{' '}
            <span className='required'>(必填)</span>
          </div>
          <Input.TextArea
            placeholder='请输入文章标题'
            maxLength={150}
            showCount
            autoSize={{ minRows: 2, maxRows: 2 }}
          />
        </div>
        <div className='form-group'>
          <div className='label'>
            <span className='bar' /> 您是否需要加入关键词?
          </div>
          <Input
            placeholder='请输入关键词，使用分号分割'
            maxLength={50}
            showCount
          />
        </div>
        <Button type='primary' className='next-btn' onClick={handleNext}>
          下一步
        </Button>
        <div className='step-indicator'>
          <span className='dot active' />
          <span className='dot' />
          <span className='dot' />
        </div>
      </div>
    </div>
  )
}

export default Writer
