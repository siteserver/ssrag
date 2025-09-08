import { RunVariable } from '@/dto'
import { Typography } from 'antd'
import DataTypeIcon from './DataTypeIcon'
import NodeBodyItem from './NodeBodyItem'

const { Text } = Typography

const NodeBodyVariables: React.FC<{
  title: string
  variables: RunVariable[]
}> = ({ title, variables }) => {
  const list = (variables || []).filter((_, index) => index < 4)

  return (
    <NodeBodyItem title={title}>
      {list.map((v, index) => (
        <Text code key={index} style={{ marginRight: 0 }}>
          {v.name}
          <DataTypeIcon type={v.dataType} />
        </Text>
      ))}
    </NodeBodyItem>
  )
}

export default NodeBodyVariables
