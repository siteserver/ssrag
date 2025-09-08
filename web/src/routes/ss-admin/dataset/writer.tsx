import { createFileRoute } from '@tanstack/react-router'
import Writer from '@/features/ss-admin/dataset/writer'

export const Route = createFileRoute('/ss-admin/dataset/writer')({
  component: Writer,
})
