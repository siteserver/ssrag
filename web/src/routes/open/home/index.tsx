import { createFileRoute } from '@tanstack/react-router'
import Home from '@/features/open/home'

export const Route = createFileRoute('/open/home/')({
  component: Home,
})
