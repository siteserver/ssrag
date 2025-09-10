import { useState } from 'react'
import { TaskDocumentProcess } from '@/dto'
import { InboxOutlined } from '@ant-design/icons'
import { Row, Upload, Button, UploadFile, App, Tag } from 'antd'
import datasetDocumentsApi from '@/api/admin/dataset/datasetDocumentsApi'
import { useDocumentsStore } from '../store/documentsStore'

const { Dragger } = Upload

interface Importing1Props {
  onSubmit: (files: TaskDocumentProcess[]) => void
}

const Importing1: React.FC<Importing1Props> = ({ onSubmit }) => {
  const { message } = App.useApp()
  const store = useDocumentsStore()
  const [fileList, setFileList] = useState<UploadFile[] | undefined>([])
  const [responseList, setResponseList] = useState<TaskDocumentProcess[]>([])

  // useEffect(() => {
  //   setFileList(
  //     files
  //       ? files
  //           .filter((file) => file && file.id)
  //           .map((file) => ({
  //             uid: file.id,
  //             name: file.file_name + file.ext_name,
  //             status: 'done',
  //             type: 'text/plain',
  //             response: file,
  //           }))
  //       : []
  //   )
  //   setResponseList(files)
  // }, [files])

  const apiRemove = (task: TaskDocumentProcess) => {
    datasetDocumentsApi.uploadRemove({ task })
    message.success('文档已删除')
  }

  const handleChange = (info: { fileList: UploadFile[]; file: UploadFile }) => {
    setFileList(info.fileList)
    const { status } = info.file
    if (status === 'done') {
      setResponseList([...responseList, info.file.response.task])
    } else if (status === 'error') {
      message.error(`${info.file.name} 上传失败！`)
    } else if (status === 'removed') {
      const task = responseList.find(
        (response) => response.uuid === info.file.response.task.uuid
      )
      if (task) {
        apiRemove(task)
        setResponseList(
          responseList.filter(
            (response) => response.uuid !== info.file.response.task.uuid
          )
        )
      }
    }
  }

  const handleNext = () => {
    onSubmit(responseList)
  }

  return (
    <Row justify='center'>
      <Dragger
        accept='.pdf,.docx,.pptx,.xlsx,.xls,.csv,.txt,.text,.md,.markdown,.html,.htm,.xml,.epub,.ipynb,.msg,.json,.jsonl,.rss,.atom,.zip'
        action={datasetDocumentsApi.getUploadUrl({
          siteId: store.siteId,
          channelId: store.channelId,
          contentId: store.contentId,
        })}
        name='file'
        headers={datasetDocumentsApi.getUploadHeaders()}
        multiple={true}
        listType='picture'
        fileList={fileList}
        onChange={handleChange}
        style={{ width: '100%' }}
      >
        <p className='ant-upload-drag-icon'>
          <InboxOutlined />
        </p>
        <p className='ant-upload-text'>点击上传或拖拽文档到这里</p>
        <p className='ant-upload-hint'>
          支持以下后缀格式的文件（可批量上传文件，支持 ZIP 压缩包上传）
          <br />
          <Tag color='blue'>.pdf</Tag>
          <Tag color='blue'>.docx</Tag>
          <Tag color='blue'>.pptx</Tag>
          <Tag color='blue'>.xlsx</Tag>
          <Tag color='blue'>.xls</Tag>
          <Tag color='blue'>.csv</Tag>
          <Tag color='blue'>.txt</Tag>
          <Tag color='blue'>.text</Tag>
          <Tag color='blue'>.md</Tag>
          <Tag color='blue'>.markdown</Tag>
          <Tag color='blue'>.html</Tag>
          <Tag color='blue'>.htm</Tag>
          <Tag color='blue'>.xml</Tag>
          <Tag color='blue'>.epub</Tag>
          <Tag color='blue'>.ipynb</Tag>
          <Tag color='blue'>.msg</Tag>
          <Tag color='blue'>.json</Tag>
          <Tag color='blue'>.jsonl</Tag>
          <Tag color='blue'>.rss</Tag>
          <Tag color='blue'>.atom</Tag>
          <Tag color='blue'>.zip</Tag>
        </p>
      </Dragger>
      <Row align='middle' style={{ marginTop: 20 }}>
        <Button disabled={true} style={{ marginRight: 10 }}>
          上一步
        </Button>
        <Button
          onClick={handleNext}
          disabled={responseList.length === 0}
          type='primary'
        >
          下一步
        </Button>
      </Row>
    </Row>
  )
}

export default Importing1
