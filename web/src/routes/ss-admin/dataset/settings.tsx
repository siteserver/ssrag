import { createFileRoute } from '@tanstack/react-router'
import Settings from '@/features/ss-admin/dataset/settings'

export const Route = createFileRoute('/ss-admin/dataset/settings')({
  component: Settings,
})
