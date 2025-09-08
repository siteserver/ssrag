import { useState } from 'react'
import { TaskDocumentProcess } from '@/dto'
import { InboxOutlined } from '@ant-design/icons'
import { Row, Upload, Button, UploadFile, App } from 'antd'
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
        // accept=".pdf,.txt,.doc,.docx,.md"
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
          支持
          PDF、TXT、DOC、DOCX、PPT、PPTX、XLS、XLSX、MD、HTML、CSV、JSON、XML、RTF、JPG、PNG
          等格式文件，可批量上传文件，支持 ZIP 压缩包上传
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
