import { createFileRoute } from '@tanstack/react-router'
import Documents from '@/features/ss-admin/dataset/documents'

export const Route = createFileRoute('/ss-admin/dataset/documents')({
  component: Documents,
})
