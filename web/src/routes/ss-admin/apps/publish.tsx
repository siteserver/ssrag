import { createFileRoute } from '@tanstack/react-router'
import Publish from '@/features/ss-admin/apps/publish'

export const Route = createFileRoute('/ss-admin/apps/publish')({
  component: Publish,
})
