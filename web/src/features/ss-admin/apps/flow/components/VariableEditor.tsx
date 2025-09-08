import { useState } from 'react'
import { useEffect } from 'react'
import { RunVariable } from '@/dto'
import { VariableDataTypeOptions, VariableDataType } from '@/enums'
import { MinusCircleOutlined } from '@ant-design/icons'
import { Table, Input, Select, Button } from 'antd'
import { useStore } from '../store'

const VariableEditor: React.FC<{
  nodeId: string
  disabled: boolean
}> = ({ nodeId, disabled }) => {
  const store = useStore()
  const [variables, setVariables] = useState<RunVariable[]>([])

  useEffect(() => {
    const outVariables = store.getNodeOutVariables(nodeId)
    const variables = []
    for (const variable of outVariables) {
      variables.push({ ...variable, key: variable.name })
    }
    setVariables(variables)
  }, [nodeId])

  const handleRemoveVariable = (
    e: React.MouseEvent<HTMLButtonElement>,
    variable: RunVariable
  ) => {
    e.stopPropagation()
    const newVariables = variables.filter((v) => v !== variable)
    store.setNodeOutVariables(nodeId, [...newVariables])
  }

  const columns = [
    {
      title: '变量名',
      dataIndex: 'name',
      width: '60%',
      render: (text: string, record: RunVariable) => (
        <Input
          size='small'
          value={text}
          disabled={disabled}
          placeholder='输入变量名'
          maxLength={20}
          onChange={(e) => {
            const newVariables = variables.map((v) =>
              v === record ? { ...v, name: e.target.value } : v
            )
            store.setNodeOutVariables(nodeId, [...newVariables])
          }}
        />
      ),
    },
    {
      title: '变量类型',
      dataIndex: 'dataType',
      width: '32%',
      render: (text: string, record: RunVariable) => (
        <Select
          size='small'
          disabled={disabled}
          value={text || VariableDataType.String}
          style={{ width: '100%' }}
          options={VariableDataTypeOptions}
          onChange={(value) => {
            const newVariables = variables.map((v) =>
              v === record ? { ...v, dataType: value as VariableDataType } : v
            )
            store.setNodeOutVariables(nodeId, [...newVariables])
          }}
        ></Select>
      ),
    },
    {
      title: '',
      dataIndex: 'delete',
      width: '8%',
      render: (_: unknown, variable: RunVariable) => (
        <Button
          size='small'
          disabled={disabled}
          icon={<MinusCircleOutlined />}
          type='text'
          danger
          onClick={(e: React.MouseEvent<HTMLButtonElement>) =>
            handleRemoveVariable(e, variable)
          }
        />
      ),
    },
  ]

  return (
    <>
      <Table
        bordered={false}
        size='small'
        columns={columns}
        dataSource={variables}
        pagination={false}
      />
    </>
  )
}

export default VariableEditor
