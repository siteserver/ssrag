import { createFileRoute } from '@tanstack/react-router'
import Flow from '@/features/ss-admin/apps/flow'

export const Route = createFileRoute('/ss-admin/apps/flow')({
  component: Flow,
})
