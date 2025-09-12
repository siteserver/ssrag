import { createFileRoute } from '@tanstack/react-router'
import Logs from '@/features/ss-admin/apps/logs/index'

export const Route = createFileRoute('/ss-admin/apps/logs')({
  component: Logs,
})
