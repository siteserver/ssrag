export enum RuleType {
  Email = 'email',
  Mobile = 'mobile',
  Url = 'url',
  Alpha = 'alpha',
  AlphaDash = 'alphaDash',
  AlphaNum = 'alphaNum',
  AlphaSpaces = 'alphaSpaces',
  Decimal = 'decimal',
  Digits = 'digits',
  Max = 'max',
  MaxValue = 'maxValue',
  Min = 'min',
  MinValue = 'minValue',
  Chinese = 'chinese',
  Zip = 'zip',
  IdCard = 'idCard',
  Regex = 'regex',
}

export const getRuleTypeName = (ruleType?: RuleType) => {
  if (!ruleType) return ''
  switch (ruleType) {
    case RuleType.Email:
      return '邮箱'
    case RuleType.Mobile:
      return '手机号码'
    case RuleType.Url:
      return '网址'
    case RuleType.Alpha:
      return '英文字母'
    case RuleType.AlphaDash:
      return '英文字母、数字、破折号或下划线'
    case RuleType.AlphaNum:
      return '英文字母或数字'
    case RuleType.AlphaSpaces:
      return '英文字母或空格'
    case RuleType.Decimal:
      return '数字'
    case RuleType.Digits:
      return '整数'
    case RuleType.Max:
      return '最大长度'
    case RuleType.MaxValue:
      return '最大数值'
    case RuleType.Min:
      return '最小长度'
    case RuleType.MinValue:
      return '最小数值'
    case RuleType.Chinese:
      return '中文'
    case RuleType.Zip:
      return '邮政编码'
    case RuleType.IdCard:
      return '身份证号码'
    case RuleType.Regex:
      return '自定义'
    default:
      return ''
  }
}

export const RuleTypeOptions = [
  {
    type: RuleType.Email,
    message: '字段必须是有效的电子邮件',
  },
  {
    type: RuleType.Mobile,
    message: '字段必须是有效的手机号码',
  },
  {
    type: RuleType.Url,
    message: '字段必须是有效的Url网址',
  },
  {
    type: RuleType.Alpha,
    message: '字段只能包含英文字母',
  },
  {
    type: RuleType.AlphaDash,
    message: '字段只能包含英文字母、数字、破折号或下划线',
  },
  {
    type: RuleType.AlphaNum,
    message: '字段只能包含英文字母或数字',
  },
  {
    type: RuleType.AlphaSpaces,
    message: '字段只能包含英文字母或空格',
  },
  {
    type: RuleType.Decimal,
    message: '字段必须是数字',
  },
  {
    type: RuleType.Digits,
    message: '字段必须是整数',
  },
  {
    type: RuleType.Max,
    message: '字段不能超过指定的长度',
  },
  {
    type: RuleType.MaxValue,
    message: '字段必须是数值，并且不能大于指定的值',
  },
  {
    type: RuleType.Min,
    message: '字段不能低于指定的长度',
  },
  {
    type: RuleType.MinValue,
    message: '字段必须是数值，并且不能小于指定的值',
  },
  {
    type: RuleType.Chinese,
    message: '字段必须是中文',
  },
  {
    type: RuleType.Zip,
    message: '字段必须是邮政编码',
  },
  {
    type: RuleType.IdCard,
    message: '字段必须是身份证号码',
  },
  {
    type: RuleType.Regex,
    message: '字段必须匹配指定的规则',
  },
]
