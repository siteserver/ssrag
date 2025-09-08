import { createFileRoute } from '@tanstack/react-router'
import Copilot from '@/features/open/copilot'

export const Route = createFileRoute('/open/copilot/')({
  component: Copilot,
  validateSearch: (search: Record<string, unknown>) => ({
    id: search.id as string,
  }),
})
