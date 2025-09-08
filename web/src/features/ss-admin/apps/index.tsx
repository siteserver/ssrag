import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { getSiteTypeName, isAppSite, SiteType } from '@/enums'
import { SiteSummary } from '@/models'
import {
  LoadingOutlined,
  PlusOutlined,
  EditOutlined,
  MenuOutlined,
} from '@ant-design/icons'
import {
  Card,
  Input,
  Spin,
  Row,
  Col,
  Button,
  Space,
  Dropdown,
  Tag,
  type MenuProps,
  App as AntdApp,
  Skeleton,
  Typography,
} from 'antd'
import appsApi from '@/api/admin/apps/appsApi'
import SiteIcon from '@/components/SiteIcon'
import './app.css'
import AppForm from './components/AppForm'
import DatasetForm from './components/DatasetForm'
import SiteDelete from './components/SiteDelete'
import SiteView from './components/SiteView'

const { Meta } = Card
const { Text } = Typography

interface State {
  allSites: SiteSummary[]
}

const Apps: React.FC = () => {
  const { message, modal } = AntdApp.useApp()
  const [loading, setLoading] = useState(false)
  const [isSiteView, setIsSiteView] = useState(false)
  const [isAppForm, setIsAppForm] = useState(false)
  const [isDatasetForm, setIsDatasetForm] = useState(false)
  const [isSiteDelete, setIsSiteDelete] = useState(false)
  const [site, setSite] = useState<SiteSummary | null>(null)
  const [state, setState] = useState<State>({ allSites: [] })
  const [sites, setSites] = useState<SiteSummary[]>([])
  const [rootSiteId, setRootSiteId] = useState<number>(0)

  const { isPending, refetch } = useQuery({
    queryKey: ['ss-admin', 'apps'],
    queryFn: async () => {
      const res = await appsApi.get()
      if (res) {
        setRootSiteId(res.rootSiteId)
        setState({
          allSites: [...res.sites],
        })
        setSites([...res.sites])
      }
      return res
    },
  })

  const apiDisable = async (site: SiteSummary) => {
    setLoading(true)
    const res = await appsApi.disable({
      siteId: site.id,
    })
    if (res) {
      message.open({
        type: 'success',
        content: site.disabled ? '应用启用成功' : '应用禁用成功',
      })
      await refetch()
    }
    setLoading(false)
  }

  const apiOrder = async (siteId: number, isUp: boolean, rows: number) => {
    setLoading(true)
    const res = await appsApi.order({
      siteId,
      isUp,
      rows,
    })
    if (res) {
      message.open({
        type: 'success',
        content: '应用排序成功',
      })
      refetch()
    }
    setLoading(false)
  }

  const handleView = (site: SiteSummary) => {
    setSite(site)
    setIsSiteView(true)
  }

  const handleAdd = (isApp: boolean) => {
    setSite(null)
    if (isApp) {
      setIsAppForm(true)
    } else {
      setIsDatasetForm(true)
    }
  }

  const handleAppEdit = (site: SiteSummary) => {
    setSite(site)
    if (isAppSite(site.siteType)) {
      setIsAppForm(true)
    } else {
      setIsDatasetForm(true)
    }
  }

  const getMoreItems = (site: SiteSummary) => {
    const menus: MenuProps['items'] = []
    menus.push({
      key: menus.length + 1 + '',
      label: '访问',
      onClick: (info) => {
        info.domEvent.stopPropagation()
        window.open(site.root ? '/' : '/' + site.siteDir, '_blank')
      },
    })
    if (isAppSite(site.siteType)) {
      menus.push({
        key: menus.length + 1 + '',
        label: site.disabled ? '启用' : '禁用',
        onClick: (info) => {
          info.domEvent.stopPropagation()
          modal.confirm({
            title: site.disabled ? '确认启用' : '确认禁用',
            content: `确定要${site.disabled ? '启用' : '禁用'} ${site.siteName} 吗？`,
            okText: '确认',
            cancelText: '取消',
            onOk: () => {
              apiDisable(site)
            },
          })
        },
      })
    }
    if (site.siteType === SiteType.Web) {
      menus.push({
        key: menus.length + 1 + '',
        label: '整站保存',
        onClick: (info) => {
          info.domEvent.stopPropagation()
          window.parent.utils.addTab(
            '保存知识库模板',
            window.parent.utils.getSettingsUrl('sitesSave', { siteId: site.id })
          )
        },
      })
      menus.push({
        key: menus.length + 1 + '',
        label: '修改',
        onClick: (info) => {
          info.domEvent.stopPropagation()
          handleAppEdit(site)
        },
      })
    }

    if (site.root) {
      const url = window.parent.utils.getSettingsUrl('sitesChangeRoot', {
        siteId: site.id,
      })
      menus.push({
        key: menus.length + 1 + '',
        label: '转移到子目录',
        onClick: (info) => {
          info.domEvent.stopPropagation()
          window.parent.utils.addTab('转移到子目录', url)
        },
      })
    } else if (rootSiteId === 0 || site.id === rootSiteId) {
      const title = '转移到根目录'
      const url = window.parent.utils.getSettingsUrl('sitesChangeRoot', {
        siteId: site.id,
      })
      menus.push({
        key: menus.length + 1 + '',
        label: title,
        onClick: (info) => {
          info.domEvent.stopPropagation()
          window.parent.utils.addTab(title, url)
        },
      })
    }

    menus.push({
      key: menus.length + 1,
      danger: true,
      label: '删除',
      disabled: site.root,
      title: site.root ? '根目录站点需要转移到子目录才能删除' : '',
      onClick: (info) => {
        info.domEvent.stopPropagation()
        setIsSiteDelete(true)
        setSite(site)
      },
    })

    return menus
  }

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <div className='apps'>
      <Spin
        spinning={loading}
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
      <Row>
        <Col span={12}>
          <Space>
            <Input.Search
              placeholder='搜索'
              style={{ width: 200 }}
              onChange={(e) => {
                const searchText = e.target.value.trim().toLowerCase()
                if (!searchText) {
                  setSites(state.allSites)
                } else {
                  const filteredSites = state.allSites.filter(
                    (site: SiteSummary) =>
                      site.siteName.toLowerCase().includes(searchText) ||
                      (site.description &&
                        site.description.toLowerCase().includes(searchText))
                  )
                  setSites(filteredSites)
                }
              }}
            />
          </Space>
        </Col>
        <Col span={12}>
          <div className='apps-drag-tip'>可拖拽卡片对项目进行排序</div>
        </Col>
      </Row>

      <div className='apps-add-button-container'>
        <Row gutter={[16, 16]}>
          <Col span={6}>
            <Button
              type='dashed'
              size='large'
              className='apps-add-button'
              icon={<PlusOutlined />}
              onClick={() => handleAdd(true)}
            >
              新增应用
            </Button>
          </Col>
          <Col span={6}>
            <Button
              type='dashed'
              size='large'
              className='apps-add-button'
              icon={<PlusOutlined />}
              onClick={() => handleAdd(false)}
            >
              新增知识库
            </Button>
          </Col>
          {sites.map((site) => {
            const icon = site.iconUrl ? (
              <img
                src={site.iconUrl}
                className='node-add-icon'
                alt={`${site.siteName} icon`}
              />
            ) : (
              <SiteIcon type={site.siteType} className='node-add-icon' />
            )
            return (
              <Col
                key={site.id}
                className={`${site.isMoving ? 'dragging' : ''} ${
                  site.isDragOver ? 'dropping' : ''
                }`}
                draggable
                onDragStart={(e) => {
                  e.dataTransfer.setData('siteId', site.id + '')
                  setSites(
                    sites.map((s) => ({ ...s, isMoving: s.id === site.id }))
                  )
                }}
                onDragEnd={() => {
                  setSites(
                    sites.map((s) => ({
                      ...s,
                      isMoving: false,
                      isDragOver: false,
                    }))
                  )
                }}
                onDragOver={(e) => {
                  e.preventDefault()
                  setSites(
                    sites.map((s) => ({
                      ...s,
                      isDragOver: s.id === site.id,
                    }))
                  )
                }}
                onDragLeave={(e) => {
                  e.preventDefault()
                  setSites(sites.map((s) => ({ ...s, isDragOver: false })))
                }}
                onDrop={(e) => {
                  e.preventDefault()
                  const draggedId = parseInt(e.dataTransfer.getData('siteId'))
                  const draggedIndex = sites.findIndex(
                    (s) => s.id === draggedId
                  )
                  const dropIndex = sites.findIndex((s) => s.id === site.id)
                  if (draggedIndex !== dropIndex) {
                    const newSites = [...sites]
                    const [removed] = newSites.splice(draggedIndex, 1)
                    newSites.splice(dropIndex, 0, removed)
                    setSites(newSites.map((s) => ({ ...s, isDragOver: false })))

                    apiOrder(
                      draggedId,
                      draggedIndex > dropIndex,
                      Math.abs(draggedIndex - dropIndex)
                    )
                  }
                }}
                span={6}
              >
                <Card
                  hoverable
                  className='apps-card'
                  variant='outlined'
                  onClick={() => handleView(site)}
                  actions={[
                    <EditOutlined
                      key='edit'
                      onClick={(e: React.MouseEvent<HTMLAnchorElement>) => {
                        e.stopPropagation()
                        handleAppEdit(site)
                      }}
                    />,
                    <Dropdown
                      key='more'
                      menu={{
                        items: getMoreItems(site),
                      }}
                      trigger={['hover']}
                    >
                      <MenuOutlined
                        key='ellipsis'
                        onClick={(e: React.MouseEvent<HTMLAnchorElement>) => {
                          e.stopPropagation()
                        }}
                      />
                    </Dropdown>,
                  ]}
                >
                  <Meta
                    avatar={icon}
                    title={
                      <Space size={1}>
                        <Text strong className='mr-2'>
                          {site.siteName}
                        </Text>
                        <Tag
                          style={{ fontWeight: 400 }}
                          color={isAppSite(site.siteType) ? 'blue' : 'green'}
                        >
                          {getSiteTypeName(site.siteType)}
                        </Tag>
                        {site.root && (
                          <Tag style={{ fontWeight: 400 }} color='red'>
                            根目录
                          </Tag>
                        )}
                        {!site.root && (
                          <Button
                            type='link'
                            size='small'
                            onClick={(e) => {
                              e.stopPropagation()
                              window.open(`/${site.siteDir}`, '_blank')
                            }}
                          >
                            /{site.siteDir}
                          </Button>
                        )}

                        {site.disabled && (
                          <Tag style={{ fontWeight: 400 }} color='error'>
                            已禁用
                          </Tag>
                        )}
                      </Space>
                    }
                    description={site.description || '暂无介绍'}
                  />
                </Card>
              </Col>
            )
          })}
        </Row>
      </div>

      {isSiteView && site && (
        <SiteView site={site} onClose={() => setIsSiteView(false)} />
      )}

      {isAppForm && (
        <AppForm
          site={site}
          rootSiteId={rootSiteId}
          onClose={() => {
            setIsAppForm(false)
            setSite(null)
          }}
        />
      )}

      {isDatasetForm && (
        <DatasetForm
          site={site}
          rootSiteId={rootSiteId}
          onClose={() => setIsDatasetForm(false)}
        />
      )}

      {isSiteDelete && site && (
        <SiteDelete site={site} onClose={() => setIsSiteDelete(false)} />
      )}
    </div>
  )
}

export default Apps
