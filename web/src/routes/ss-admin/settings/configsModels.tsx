import { createFileRoute } from '@tanstack/react-router'
import ConfigsModels from '@/features/ss-admin/settings/configsModels'

export const Route = createFileRoute('/ss-admin/settings/configsModels')({
  component: ConfigsModels,
})
