import { createFileRoute } from '@tanstack/react-router'
import Status from '@/features/ss-admin/dataset/status'

export const Route = createFileRoute('/ss-admin/dataset/status')({
  component: Status,
})
