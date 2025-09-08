import { useState } from 'react'
import { getSiteTypeName, isAppSite } from '@/enums'
import { SiteSummary } from '@/models'
import { EditOutlined, EllipsisOutlined, PlusOutlined } from '@ant-design/icons'
import {
  Row,
  Col,
  Button,
  Card,
  Space,
  Tag,
  Dropdown,
  Modal,
  message,
} from 'antd'
import { MenuProps } from 'antd/es/menu'
import datasetSelectApi from '@/api/admin/apps/modals/datasetSelectApi'
import SiteIcon from '@/components/SiteIcon'
import DatasetConfig from '../modals/DatasetConfig'
import DatasetSelect from '../modals/DatasetSelect'

const { Meta } = Card

const SettingsDataset: React.FC<{
  siteId: number
  nodeId: string
  datasetSites: SiteSummary[]
  onChange: (datasetSites: SiteSummary[]) => void
}> = ({ siteId, nodeId, datasetSites, onChange }) => {
  const [modalDatasetSelect, setModalDatasetSelect] = useState(false)
  const [modalDatasetConfig, setModalDatasetConfig] = useState(false)
  const [datasetConfigSite, setDatasetConfigSite] =
    useState<SiteSummary | null>(null)

  const handleAdd = () => {
    setModalDatasetSelect(true)
  }

  const handleAppEdit = (site: SiteSummary) => {
    setDatasetConfigSite(site)
    setModalDatasetConfig(true)
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

    menus.push({
      key: menus.length + 1,
      danger: true,
      label: '取消关联',
      onClick: async (info) => {
        info.domEvent.stopPropagation()
        const res = await datasetSelectApi.delete({
          siteId: siteId,
          nodeId: '',
          datasetSiteId: site.id,
        })
        if (res) {
          message.success('取消关联成功')
          onChange(datasetSites.filter((s) => s.id !== site.id))
        }
      },
    })

    return menus
  }

  return (
    <Row gutter={[16, 16]}>
      <Col span={12}>
        <Button
          type='dashed'
          size='large'
          className='apps-add-button'
          icon={<PlusOutlined />}
          onClick={() => handleAdd()}
        >
          关联知识库
        </Button>
      </Col>
      {datasetSites.map((site) => {
        const icon = site.iconUrl ? (
          <img
            src={site.iconUrl}
            className='h-10 w-10 rounded object-contain'
            alt={`${site.siteName} icon`}
          />
        ) : (
          <SiteIcon type={site.siteType} className='h-10 w-10' />
        )
        return (
          <Col key={site.id} span={12}>
            <Card
              hoverable
              className='apps-card'
              variant='outlined'
              onClick={() => handleAppEdit(site)}
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
                  <EllipsisOutlined
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
                  <Space>
                    <a>{site.siteName}</a>
                    <Tag
                      style={{ fontWeight: 400 }}
                      color={isAppSite(site.siteType) ? 'blue' : 'green'}
                    >
                      {getSiteTypeName(site.siteType)}
                    </Tag>
                  </Space>
                }
                description={site.description || '暂无介绍'}
              />
            </Card>
          </Col>
        )
      })}

      {modalDatasetSelect && (
        <Modal
          open={true}
          onCancel={() => setModalDatasetSelect(false)}
          title='关联知识库'
          footer={null}
          width='850px'
          style={{ top: 20, bottom: 20 }}
          destroyOnHidden={true}
        >
          <div className='h-full overflow-y-auto'>
            <DatasetSelect
              siteId={siteId}
              nodeId={nodeId}
              onChange={(selectedSites) => {
                onChange(selectedSites)
                setModalDatasetSelect(false)
              }}
            />
          </div>
        </Modal>
      )}

      {modalDatasetConfig && datasetConfigSite && (
        <DatasetConfig
          siteId={siteId}
          nodeId={nodeId}
          site={datasetConfigSite}
          onClose={() => setModalDatasetConfig(false)}
        />
      )}
    </Row>
  )
}

export default SettingsDataset
