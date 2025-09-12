import { createFileRoute } from '@tanstack/react-router'
import Copilot from '@/features/open/copilot'

export const Route = createFileRoute('/open/copilot/')({
  component: () => <Copilot sessionId={null} />,
  validateSearch: (search: Record<string, unknown>) => ({
    sessionId: search.sessionId as string,
    id: search.id as string,
  }),
})
