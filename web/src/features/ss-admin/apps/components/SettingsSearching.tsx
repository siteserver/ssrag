import { SearchType } from '@/enums'
import { SearchTypeDisplayName } from '@/enums'
import { Form, Radio, Slider } from 'antd'

const SettingsSearching: React.FC = () => {
  return (
    <div style={{ position: 'relative' }}>
      <Form.Item
        name='datasetSearchType'
        label='搜索策略'
        rules={[
          {
            required: true,
            message: '请选择搜索策略',
          },
        ]}
        help='从知识库中获取知识的检索方式，不同的检索策略可以更有效地找到正确的信息，提高其生成的答案的准确性和可用性。'
      >
        <Radio.Group
          options={[
            {
              value: SearchType.Semantic,
              label: SearchTypeDisplayName[SearchType.Semantic],
            },
            {
              value: SearchType.FullText,
              label: SearchTypeDisplayName[SearchType.FullText],
            },
            {
              value: SearchType.Hybrid,
              label: SearchTypeDisplayName[SearchType.Hybrid],
            },
          ]}
        />
      </Form.Item>
      <Form.Item
        name='datasetMaxCount'
        label='最大召回数量'
        rules={[
          {
            required: true,
            message: '请输入最大召回数量',
          },
        ]}
        help='从知识库中返回给大模型的最大段落数，数值越大返回的内容越多'
      >
        <Slider
          marks={{
            1: '1',
            5: '5',
            10: '10',
            15: '15',
            20: '20',
          }}
          min={1}
          max={20}
        />
      </Form.Item>
      <Form.Item
        name='datasetMinScore'
        label='最小匹配度'
        rules={[
          {
            required: true,
            message: '请输入最小匹配度',
          },
        ]}
        help='根据设置的匹配度选取段落返回给大模型，低于设定匹配度的内容不会被召回'
      >
        <Slider
          marks={{
            0.01: '0.01',
            0.5: '0.5',
            0.99: '0.99',
          }}
          min={0.01}
          max={0.99}
          step={0.01}
        />
      </Form.Item>
    </div>
  )
}

export default SettingsSearching
