import { createFileRoute } from '@tanstack/react-router'
import Apps from '@/features/ss-admin/apps'

export const Route = createFileRoute('/ss-admin/apps/')({
  component: Apps,
})
