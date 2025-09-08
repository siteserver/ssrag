export enum ModelSkill {
  REASONING = 'reasoning',
  VLM = 'vlm',
  TOOLS = 'tools',
  FIM = 'fim',
  MATH = 'math',
  CODER = 'coder',
}

export const ModelSkillDisplayNames: Record<ModelSkill, string> = {
  [ModelSkill.REASONING]: '推理',
  [ModelSkill.VLM]: '视觉',
  [ModelSkill.TOOLS]: '工具',
  [ModelSkill.FIM]: '补全',
  [ModelSkill.MATH]: '数学',
  [ModelSkill.CODER]: '代码',
}

export function getModelSkillDisplayName(
  modelSkill: ModelSkill | string
): string {
  return ModelSkillDisplayNames[modelSkill as ModelSkill] || modelSkill
}
