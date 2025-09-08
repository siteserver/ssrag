import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Cascade, Result } from '@/dto'
import { SearchType } from '@/enums'
import {
  DownloadOutlined,
  LoadingOutlined,
  QuestionCircleOutlined,
} from '@ant-design/icons'
import { MenuUnfoldOutlined } from '@ant-design/icons'
import {
  Spin,
  Modal,
  Skeleton,
  message,
  Button,
  InputNumber,
  Row,
  Col,
  Slider,
  Input,
  Tooltip,
  Radio,
  Cascader,
  Splitter,
  Space,
  Empty,
  Card,
  Divider,
  Flex,
  Typography,
  InputNumberProps,
} from 'antd'
import datasetDocumentsApi from '@/api/admin/dataset/datasetDocumentsApi'
import datasetTestingApi from '@/api/admin/dataset/datasetTestingApi'
import { mdToHtml } from '@/utils/markdown'
import AdaptiveHeightEditor from '@/components/adaptive-height-editor'
import FileIcon from '@/components/file-icon'
import './app.css'
import { useTestingStore } from './store/testingStore'

const { Text } = Typography

const Testing: React.FC = () => {
  const store = useTestingStore()
  const [markdown, setMarkdown] = useState('')
  const [isPreview, setIsPreview] = useState(false)
  const [showLeftPanel, setShowLeftPanel] = useState(true)
  const [searchType, setSearchType] = useState(SearchType.Semantic)
  const [documentChannelIds, setDocumentChannelIds] = useState<number[]>([
    store.siteId,
  ])
  const [query, setQuery] = useState('')
  const [cuts, setCuts] = useState<string[]>([])
  const [results, setResults] = useState<Result[] | null>(null)
  const [maxCount, setMaxCount] = useState(10)
  const [minScore, setMinScore] = useState(0.4)

  const { isPending } = useQuery({
    queryKey: ['ss-admin', 'testing', store.siteId],
    queryFn: async () => {
      const res = await datasetTestingApi.get({
        siteId: store.siteId,
      })
      if (res) {
        store.init(res.siteUrl, res.channels)
      }
      return res
    },
  })

  const getChannelName = (
    channelId: number,
    cascades: Cascade<number>[]
  ): string => {
    for (const cascade of cascades) {
      if (cascade.value === channelId) {
        return cascade.label || ''
      }
      if (cascade.children) {
        const childName = getChannelName(channelId, cascade.children)
        if (childName) {
          return childName
        }
      }
    }
    return ''
  }

  const handleMaxCountChange: InputNumberProps['onChange'] = (newValue) => {
    setMaxCount(newValue as number)
  }

  const handleMinScoreChange: InputNumberProps['onChange'] = (newValue) => {
    setMinScore(newValue as number)
  }

  const handleTestingClick = async () => {
    if (!query) {
      message.error('请输入搜索文本')
      return
    }

    store.setLoading(true)
    const res = await datasetTestingApi.submit({
      siteId: store.siteId,
      channelId: store.channelId,
      contentId: store.contentId,
      searchType: searchType,
      maxCount,
      minScore,
      query,
    })
    if (res) {
      setCuts(res.cuts)
      setResults([...res.results])
      setDocumentChannelIds(res.channelIds)
    }
    store.setLoading(false)
  }

  const handleOpenDocument = async (documentId: number, isPreview: boolean) => {
    store.setLoading(true)
    if (isPreview) {
      const res = await datasetDocumentsApi.getMarkdown({
        documentId: documentId,
      })
      if (res) {
        setMarkdown(res.value)
        setIsPreview(true)
      }
    } else {
      const res = await datasetDocumentsApi.download({
        documentId: documentId,
      })
      if (res) {
        window.open(res.value, '_blank')
      }
    }
    store.setLoading(false)
  }

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <div className='dataset-container'>
      <Spin
        spinning={store.loading}
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
      <Splitter
        style={{
          height: '100hv',
          minHeight: '550px',
          boxShadow: '0 0 10px rgba(0, 0, 0, 0.1)',
        }}
      >
        {showLeftPanel && (
          <Splitter.Panel
            defaultSize='20%'
            min='0'
            max='70%'
            style={{ backgroundColor: '#fff', overflowX: 'hidden' }}
          >
            <div className='testing-content'>
              <Flex vertical gap='small'>
                <Text type='secondary'>搜索策略</Text>
                <Radio.Group
                  block
                  onChange={(e) => {
                    setSearchType(e.target.value)
                  }}
                  options={[
                    {
                      label: (
                        <span>
                          语义检索
                          <Tooltip title='基于向量的文本相关性查询，推荐在需要理解语义关联度和跨语言查询的场景使用。'>
                            <QuestionCircleOutlined style={{ marginLeft: 4 }} />
                          </Tooltip>
                        </span>
                      ),
                      value: SearchType.Semantic,
                    },
                    {
                      label: (
                        <span>
                          全文检索
                          <Tooltip title='依赖于关键词的全文搜索，推荐在搜索具有特定名称、缩写词、短语或ID的场景使用。'>
                            <QuestionCircleOutlined style={{ marginLeft: 4 }} />
                          </Tooltip>
                        </span>
                      ),
                      value: SearchType.FullText,
                    },
                    {
                      label: (
                        <span>
                          混合检索
                          <Tooltip title='结合全文检索与语义检索的优势，并对结果进行综合排序'>
                            <QuestionCircleOutlined style={{ marginLeft: 4 }} />
                          </Tooltip>
                        </span>
                      ),
                      value: SearchType.Hybrid,
                    },
                  ]}
                  value={searchType}
                  optionType='button'
                  buttonStyle='solid'
                />
                <Text type='secondary'>
                  搜索范围
                  <Tooltip title='设置搜索范围，系统将搜索该栏目及其子栏目下的所有内容'>
                    <QuestionCircleOutlined style={{ marginLeft: 4 }} />
                  </Tooltip>
                </Text>
                <Cascader
                  placeholder='选择栏目'
                  variant='outlined'
                  expandTrigger='hover'
                  changeOnSelect
                  style={{ marginBottom: '6px', width: '100%' }}
                  options={store.channels}
                  value={documentChannelIds}
                  onChange={(value) => {
                    store.setChannelId(value[value.length - 1] as number)
                    setDocumentChannelIds(value)
                  }}
                />
                <Text type='secondary'>
                  最大召回数量
                  <Tooltip title='设置最大召回数量，超过该数量后，将不再召回更多内容'>
                    <QuestionCircleOutlined style={{ marginLeft: 4 }} />
                  </Tooltip>
                </Text>
                <Row>
                  <Col span={16}>
                    <Slider
                      min={1}
                      max={20}
                      onChange={handleMaxCountChange}
                      value={typeof maxCount === 'number' ? maxCount : 0}
                    />
                  </Col>
                  <Col span={8}>
                    <InputNumber
                      min={1}
                      max={20}
                      style={{ margin: '0 16px' }}
                      value={maxCount}
                      onChange={handleMaxCountChange}
                    />
                  </Col>
                </Row>
                <Text type='secondary'>
                  最小匹配度
                  <Tooltip title='根据设置的匹配度选取段落返回给大模型，低于设定匹配度的内容不会被召回'>
                    <QuestionCircleOutlined style={{ marginLeft: 4 }} />
                  </Tooltip>
                </Text>
                <Row>
                  <Col span={16}>
                    <Slider
                      min={0.01}
                      max={0.99}
                      step={0.01}
                      onChange={handleMinScoreChange}
                      value={typeof minScore === 'number' ? minScore : 0}
                    />
                  </Col>
                  <Col span={8}>
                    <InputNumber
                      min={0.01}
                      max={0.99}
                      style={{ margin: '0 16px' }}
                      value={minScore}
                      onChange={handleMinScoreChange}
                    />
                  </Col>
                </Row>
                <Text type='secondary'>搜索文本</Text>
                <Input.TextArea
                  value={query}
                  onChange={(e) => {
                    setQuery(e.target.value)
                  }}
                  autoFocus
                  placeholder='请输入文本'
                  autoSize={{ minRows: 8, maxRows: 16 }}
                />
                <Button block type='primary' onClick={handleTestingClick}>
                  搜索
                </Button>
              </Flex>
            </div>
          </Splitter.Panel>
        )}

        <Splitter.Panel
          defaultSize={showLeftPanel ? '80%' : '100%'}
          min='20%'
          max='100%'
          style={{ backgroundColor: '#fff' }}
        >
          {!results && (
            <Row
              align={'middle'}
              justify={'center'}
              style={{ width: '100%', height: '100%' }}
            >
              <Empty
                image={Empty.PRESENTED_IMAGE_SIMPLE}
                description='测试结果将在这里展示'
              />
            </Row>
          )}
          {results && (
            <div className='testing-results'>
              <div className='testing-results-header'>
                <Space>
                  {!showLeftPanel && (
                    <Button
                      type='text'
                      icon={<MenuUnfoldOutlined />}
                      onClick={() => setShowLeftPanel(true)}
                    />
                  )}

                  <Text strong>{`获取到 ${results.length} 个分块`}</Text>

                  {/* <FileIcon type={document.ext_name} />
                <Text strong>{`${document.file_name}${document.ext_name}`}</Text>
                <Tag color="blue">{getDocumentName(document.type)}</Tag>
                <Tag color="green">{getFileSize(document.file_size)}</Tag> */}
                </Space>
              </div>

              <div className='testing-results-content'>
                <div className='testing-results-content-left'>
                  {results.map((result) => {
                    const highlightedHtml = mdToHtml(result.text)
                    // 解析highlightedHtml中的标签内文本，并对每一个cuts高亮显示
                    let highlightedContent = highlightedHtml

                    if (cuts && cuts.length > 0) {
                      // 构造正则，避免在标签内高亮，只高亮文本内容
                      // 方案：用DOMParser解析html，递归遍历节点，对文本节点做高亮
                      try {
                        const parser = new DOMParser()
                        const doc = parser.parseFromString(
                          highlightedHtml,
                          'text/html'
                        )
                        function highlightNode(node: Node) {
                          if (node.nodeType === Node.TEXT_NODE) {
                            let text = node.textContent || ''
                            cuts.forEach((cut) => {
                              if (!cut) return
                              // 对cut做转义
                              const safeCut = cut.replace(
                                /[.*+?^${}()|[\]\\]/g,
                                '\\$&'
                              )
                              const regex = new RegExp(safeCut, 'gi')
                              text = text.replace(
                                regex,
                                (match) =>
                                  `<span style="background-color: #ffeb3b; font-weight: bold;">${match}</span>`
                              )
                            })
                            // 用span包裹，防止直接插入html
                            const temp = document.createElement('span')
                            temp.innerHTML = text
                            // 替换原文本节点
                            if (node.parentNode) {
                              Array.from(temp.childNodes).forEach((n) => {
                                node.parentNode!.insertBefore(n, node)
                              })
                              node.parentNode.removeChild(node)
                            }
                          } else if (node.nodeType === Node.ELEMENT_NODE) {
                            Array.from(node.childNodes).forEach(highlightNode)
                          }
                        }
                        Array.from(doc.body.childNodes).forEach(highlightNode)
                        highlightedContent = doc.body.innerHTML
                      } catch {
                        // fallback: 不做高亮
                        highlightedContent = highlightedHtml
                      }
                    }

                    return (
                      <Card
                        key={result.id}
                        hoverable={true}
                        size='small'
                        style={{ marginBottom: 8 }}
                      >
                        <Text style={{ textAlign: 'justify' }}>
                          <div
                            className='markdown-body'
                            style={{ backgroundColor: 'transparent' }}
                            dangerouslySetInnerHTML={{
                              __html: highlightedContent,
                            }}
                          />
                        </Text>
                        <Divider style={{ margin: '12px 0' }} />
                        <Flex
                          style={{
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'space-between',
                          }}
                        >
                          <Space
                            style={{
                              justifyContent: 'space-between',
                              alignItems: 'center',
                            }}
                          >
                            <Button
                              color='primary'
                              style={{ fontSize: '12px' }}
                              onClick={() =>
                                handleOpenDocument(result.documentId, false)
                              }
                            >
                              <a>
                                <FileIcon type={result.extName} />
                                <span className='testing-results-content-left-title'>
                                  {result.docName}
                                </span>
                                <DownloadOutlined />
                              </a>
                            </Button>
                          </Space>
                          <Space>
                            <span className='chunk-tag'>
                              所属栏目：
                              <Text strong>
                                {getChannelName(
                                  result.channelId,
                                  store.channels
                                )}
                              </Text>
                            </span>
                            <span className='chunk-tag'>
                              内容长度：
                              <Text strong>{result.text.length}</Text>
                            </span>
                            {(searchType === SearchType.Semantic ||
                              searchType === SearchType.FullText) && (
                              <span className='chunk-tag'>
                                SCORE：
                                <Text strong>{result.score.toFixed(2)}</Text>
                              </span>
                            )}
                          </Space>
                        </Flex>
                      </Card>
                    )
                  })}
                </div>
              </div>
            </div>
          )}
        </Splitter.Panel>
      </Splitter>

      <Modal
        centered
        closable={false}
        open={isPreview}
        onCancel={() => setIsPreview(false)}
        width={'100%'}
        styles={{
          body: {
            height: window.innerHeight - 140 + 'px',
            overflow: 'auto',
          },
        }}
        footer={
          <Button type='primary' onClick={() => setIsPreview(false)}>
            关 闭
          </Button>
        }
      >
        <AdaptiveHeightEditor
          height='100%'
          language='markdown'
          value={markdown}
          onChange={() => {}}
        />
      </Modal>
    </div>
  )
}

export default Testing
