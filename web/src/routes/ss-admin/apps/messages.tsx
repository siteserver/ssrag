import { createFileRoute } from '@tanstack/react-router'
import Messages from '@/features/ss-admin/apps/messages'

export const Route = createFileRoute('/ss-admin/apps/messages')({
  component: Messages,
})
