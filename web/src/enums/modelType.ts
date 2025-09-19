export enum ModelType {
  LLM = 'llm',
  TEXT_EMBEDDING = 'text-embedding',
  RERANK = 'rerank',
  TO_IMAGE = 'to-image',
  SPEECH2TEXT = 'speech2text',
  TTS = 'tts',
  MODERATION = 'moderation',
}

export const ModelTypeDisplayNames: Record<ModelType, string> = {
  [ModelType.LLM]: '对话',
  [ModelType.TEXT_EMBEDDING]: '文本嵌入',
  [ModelType.RERANK]: '重排序',
  [ModelType.TO_IMAGE]: '图片生成',
  [ModelType.SPEECH2TEXT]: '语音转文本',
  [ModelType.TTS]: '文本转语音',
  [ModelType.MODERATION]: '内容审核',
}

export function getModelTypeDisplayName(modelType: ModelType | string): string {
  return ModelTypeDisplayNames[modelType as ModelType] || modelType
}
