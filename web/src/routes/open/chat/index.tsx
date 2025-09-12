import { createFileRoute } from '@tanstack/react-router'
import Chat from '@/features/open/chat'

export const Route = createFileRoute('/open/chat/')({
  component: () => <Chat sessionId={null} />,
  validateSearch: (search: Record<string, unknown>) => ({
    sessionId: search.sessionId as string,
    id: search.id as string,
  }),
})
