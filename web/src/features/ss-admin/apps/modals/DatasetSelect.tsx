import React, { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { SiteSummary } from '@/models'
import { message, Skeleton } from 'antd'
import datasetSelectApi from '@/api/admin/apps/modals/datasetSelectApi'
import SiteIcon from '@/components/SiteIcon'

const DatasetSelect: React.FC<{
  siteId: number
  nodeId: string
  onChange: (selectedSites: SiteSummary[]) => void
}> = ({ siteId, nodeId, onChange }) => {
  const [sites, setSites] = useState<SiteSummary[]>([])
  const [selectedSiteIds, setSelectedSiteIds] = useState<number[]>([])
  const [search, setSearch] = useState('')

  const { isPending } = useQuery({
    queryKey: ['ss-admin', 'apps', 'modals', 'datasetSelect', siteId, nodeId],
    queryFn: async () => {
      const res = await datasetSelectApi.get({ siteId, nodeId })
      if (res) {
        setSites(res.sites || [])
        setSelectedSiteIds(res.selectedSiteIds || [])
      }
      return res
    },
  })

  const filteredSites = sites.filter((m) => {
    return (
      m.siteName?.toLowerCase().includes(search.toLowerCase()) ||
      m.description?.toLowerCase().includes(search.toLowerCase())
    )
  })

  const handleSiteClick = async (site: SiteSummary) => {
    const res = await datasetSelectApi.submit({
      siteId,
      nodeId,
      datasetSiteId: site.id,
    })
    if (res) {
      if (selectedSiteIds.includes(site.id)) {
        message.warning('已取消关联')
        const newSelectedSiteIds = selectedSiteIds.filter((m) => m !== site.id)
        setSelectedSiteIds(newSelectedSiteIds)
        const newSelectedSites = sites.filter((m) =>
          newSelectedSiteIds.includes(m.id)
        )
        onChange(newSelectedSites)
      } else {
        message.success('关联成功')
        const newSelectedSiteIds = [...selectedSiteIds, site.id]
        setSelectedSiteIds(newSelectedSiteIds)
        const newSelectedSites = sites.filter((m) =>
          newSelectedSiteIds.includes(m.id)
        )
        onChange(newSelectedSites)
      }
    }
  }

  if (isPending) {
    return <Skeleton style={{ padding: '20px' }} active />
  }

  return (
    <div className='w-full max-w-[800px] rounded-2xl bg-white p-8'>
      <div className='mb-8 flex items-center'>
        <input
          className='w-72 rounded border px-3 py-2 text-sm focus:ring-2 focus:ring-blue-500 focus:outline-none'
          placeholder='Search'
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>
      <div className='grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3'>
        {filteredSites.map((m) => {
          const icon = m.iconUrl ? (
            <img
              src={m.iconUrl}
              className='mr-4 h-10 w-10 rounded object-contain'
              alt={`${m.siteName} icon`}
            />
          ) : (
            <SiteIcon type={m.siteType} className='mr-4 h-10 w-10' />
          )

          return (
            <div
              key={m.id}
              className={`flex cursor-pointer items-center rounded-lg p-4 hover:shadow-lg ${
                selectedSiteIds.includes(m.id)
                  ? 'border-2 border-blue-500 bg-blue-100'
                  : 'border bg-white'
              }`}
              onClick={() => {
                handleSiteClick(m)
              }}
            >
              {icon}
              <div className='flex-1'>
                <div className='mb-1 text-base font-semibold'>{m.siteName}</div>
                <div className='mb-1 text-xs text-gray-500'>
                  <div className='line-clamp-3 wrap-anywhere'>
                    {m.description}
                  </div>
                </div>
              </div>
            </div>
          )
        })}
      </div>
    </div>
  )
}

export default DatasetSelect
