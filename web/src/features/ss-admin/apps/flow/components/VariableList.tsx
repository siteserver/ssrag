import { RunVariable } from '@/dto'
import { Typography } from 'antd'
import AdaptiveHeightEditor from '@/components/adaptive-height-editor'

const VariableList: React.FC<{ variables: RunVariable[] }> = ({
  variables,
}) => {
  return (
    <>
      {variables &&
        variables.map((v, index) => {
          let value: string = ''
          let language: string = 'json'
          if (v.value != null && typeof v.value === 'object') {
            value = JSON.stringify(v.value, null, 2)
            language = 'json'
          } else {
            value = v.value?.toString() || ''
            language = 'html'
          }

          return (
            <div key={`${v.name}-${index}`}>
              <Typography.Title copyable={{ text: value }} level={5}>
                {v.name}
              </Typography.Title>
              <AdaptiveHeightEditor
                language={language}
                value={value}
                onChange={() => {}}
              />
            </div>
          )
        })}
    </>
  )
}

export default VariableList
