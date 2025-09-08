import { createFileRoute } from '@tanstack/react-router'
import Settings from '@/features/ss-admin/apps/settings'

export const Route = createFileRoute('/ss-admin/apps/settings')({
  component: Settings,
})
