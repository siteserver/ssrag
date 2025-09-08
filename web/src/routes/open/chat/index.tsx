import { createFileRoute } from '@tanstack/react-router'
import Chat from '@/features/open/chat'

export const Route = createFileRoute('/open/chat/')({
  component: Chat,
  validateSearch: (search: Record<string, unknown>) => ({
    id: search.id as string,
  }),
})
