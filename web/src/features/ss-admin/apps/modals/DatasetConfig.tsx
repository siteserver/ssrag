import React, { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Cascade } from '@/dto'
import { SiteSummary } from '@/models'
import { Cascader, Form, message, Modal, Radio, Skeleton } from 'antd'
import datasetConfigApi from '@/api/admin/apps/modals/datasetConfigApi'
import SiteIcon from '@/components/SiteIcon'

const DatasetConfig: React.FC<{
  siteId: number
  nodeId: string
  site: SiteSummary
  onClose: () => void
}> = ({ siteId, nodeId, site, onClose }) => {
  const [channels, setChannels] = useState<Cascade<number>[]>([])
  const [datasetAllChannels, setDatasetAllChannels] = useState<boolean>(true)
  const [datasetChannelIds, setDatasetChannelIds] = useState<Array<number[]>>(
    []
  )

  const { isPending } = useQuery({
    queryKey: [
      'ss-admin',
      'apps',
      'modals',
      'datasetConfig',
      siteId,
      nodeId,
      site.id,
    ],
    queryFn: async () => {
      const res = await datasetConfigApi.get({
        siteId,
        nodeId,
        datasetSiteId: site.id,
      })
      if (res) {
        setChannels(res.channels)
        setDatasetAllChannels(res.dataset.datasetAllChannels ? true : false)
        setDatasetChannelIds(res.datasetChannelIds || [])
      }
      return res
    },
  })

  const handleSubmit = async () => {
    const res = await datasetConfigApi.submit({
      siteId,
      nodeId,
      datasetSiteId: site.id,
      datasetAllChannels,
      datasetChannelIds,
    })
    if (res) {
      message.success('知识库配置成功')
      onClose()
    }
  }

  const icon = site.iconUrl ? (
    <img
      src={site.iconUrl}
      className='mr-4 h-10 w-10 rounded object-contain'
      alt={`${site.siteName} icon`}
    />
  ) : (
    <SiteIcon type={site.siteType} className='mr-4 h-10 w-10' />
  )

  return (
    <Modal
      title='配置知识库'
      open={true}
      onCancel={onClose}
      onOk={handleSubmit}
      width='850px'
      style={{ top: 20, bottom: 20 }}
      destroyOnHidden={true}
    >
      {isPending && <Skeleton style={{ padding: '20px' }} active />}
      {!isPending && (
        <Form layout='vertical'>
          <div
            key={site.id}
            className={`mt-4 mb-4 flex items-center rounded-lg border-2 p-4`}
          >
            {icon}
            <div className='flex-1'>
              <div className='mb-1 text-base font-semibold'>
                {site.siteName}
              </div>
              <div className='mb-1 text-xs text-gray-500'>
                <div className='line-clamp-3 wrap-anywhere'>
                  {site.description}
                </div>
              </div>
            </div>
          </div>

          <Form.Item label='检索范围' tooltip='设置检索范围'>
            <Radio.Group
              options={[
                { label: '检索整个知识库', value: true },
                { label: '仅检索指定栏目', value: false },
              ]}
              onChange={(e) => setDatasetAllChannels(e.target.value === true)}
              value={datasetAllChannels}
              optionType='button'
              buttonStyle='solid'
            />
          </Form.Item>

          {datasetAllChannels === false && (
            <Form.Item
              name='datasetChannelIds'
              label='指定栏目'
              tooltip='设置指定栏目'
            >
              <Cascader.Panel
                multiple
                options={channels}
                className='w-full'
                defaultValue={datasetChannelIds}
                onChange={(value) => {
                  setDatasetChannelIds(value as Array<number[]>)
                }}
              />
              <div className='mt-2 text-xs text-gray-500'>
                指定栏目后，系统将仅检索指定栏目以及子栏目下的内容。
              </div>
            </Form.Item>
          )}
        </Form>
      )}
    </Modal>
  )
}

export default DatasetConfig
