import { getSiteTypeName, isAppSite } from '@/enums'
import { SiteSummary } from '@/models'
import { Modal, Descriptions } from 'antd'
import SiteIcon from '@/components/SiteIcon'

const SiteView: React.FC<{
  site: SiteSummary
  onClose: () => void
}> = ({ site, onClose }) => {
  const isApp = isAppSite(site.siteType)
  const siteUrl = '/' + (site.root ? '' : site.siteDir)

  return (
    <Modal
      title={isApp ? '应用信息' : '知识库信息'}
      open={true}
      width={800}
      footer={null}
      onCancel={() => onClose()}
    >
      <Descriptions
        title=''
        bordered
        column={2}
        styles={{
          root: {
            paddingTop: 10,
          },
          label: { width: 140 },
        }}
      >
        <Descriptions.Item label={isApp ? '应用类型' : '知识库类型'}>
          {getSiteTypeName(site.siteType)}
        </Descriptions.Item>
        <Descriptions.Item label={isApp ? '应用名称' : '知识库名称'}>
          <span>
            {site.iconUrl ? (
              <img
                src={site.iconUrl}
                className='node-add-icon'
                alt={`${site.siteName} icon`}
              />
            ) : (
              <SiteIcon type={site.siteType} className='node-add-icon' />
            )}
            <span style={{ marginLeft: 10 }}>{site.siteName}</span>
          </span>
        </Descriptions.Item>
        <Descriptions.Item label={isApp ? '应用Id' : '知识库Id'}>
          {site.id}
        </Descriptions.Item>
        {!isApp && (
          <Descriptions.Item label='内容表'>{site.tableName}</Descriptions.Item>
        )}
        <Descriptions.Item label={isApp ? '应用文件夹' : '知识库文件夹'}>
          {site.siteDir}
        </Descriptions.Item>

        <Descriptions.Item label='访问地址'>
          <a href={siteUrl} target='_blank'>
            {siteUrl}
          </a>
        </Descriptions.Item>
        <Descriptions.Item label={isApp ? '应用介绍' : '知识库介绍'} span={2}>
          {site.description || '暂无介绍'}
        </Descriptions.Item>
      </Descriptions>
    </Modal>
  )
}

export default SiteView
