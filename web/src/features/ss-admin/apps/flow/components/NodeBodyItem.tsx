import { Flex, Typography } from 'antd'

const { Text } = Typography

const NodeBodyItem: React.FC<{
  title: string
  children: React.ReactNode
}> = ({ title, children }) => {
  return (
    <Flex gap='small' style={{ marginBottom: '8px' }}>
      <Text type='secondary' strong>
        {title}
      </Text>
      {children}
    </Flex>
  )
}

export default NodeBodyItem
