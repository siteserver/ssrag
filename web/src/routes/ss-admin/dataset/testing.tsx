import { createFileRoute } from '@tanstack/react-router'
import Testing from '@/features/ss-admin/dataset/testing'

export const Route = createFileRoute('/ss-admin/dataset/testing')({
  component: Testing,
})
