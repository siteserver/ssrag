import { ChunkConfig } from '@/models'

export const defaultChunkConfig: ChunkConfig = {
  separators: ['double_new_line', 'new_line', ' '],
  chunkSize: 1000,
  chunkOverlap: 200,
  isChunkReplaces: false,
  isChunkDeletes: false,
}
