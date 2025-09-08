import { useEffect, useState, useMemo, useCallback } from 'react'
import { ReactFlowNode } from '@/dto'
import { VariableDataType } from '@/enums'
import { FlowVariable } from '@/models'
import {
  MinusCircleOutlined,
  CloseOutlined,
  SettingOutlined,
} from '@ant-design/icons'
import { useReactFlow } from '@xyflow/react'
import {
  Form,
  Input,
  Button,
  Menu,
  Space,
  Tag,
  Typography,
  Flex,
  FormInstance,
} from 'antd'
import { useStore } from '../store'
import NodeIcon from './NodeIcon'

const VariableReference: React.FC<{
  nodeId: string
  form: FormInstance
  disabled?: boolean
}> = ({ nodeId, form, disabled }) => {
  const store = useStore()
  const { getEdges, getNodes } = useReactFlow()
  const [modelIndexVisible, setModelIndexVisible] = useState(-1)

  const edges = useMemo(() => getEdges(), [getEdges])
  const nodes = useMemo(() => getNodes(), [getNodes])

  const getLeftConnectedNodes = useCallback(
    (currentNodeId: string) => {
      const leftNodes: ReactFlowNode[] = []
      const getConnectedNodes = (currentId: string) => {
        const connectedEdges = edges.filter((edge) => edge.target === currentId)
        const connectedNodeIds = connectedEdges.map((edge) => edge.source)
        const connectedNodes = nodes.filter((node) =>
          connectedNodeIds.includes(node.id)
        )
        for (const connectedNode of connectedNodes) {
          if (!leftNodes.find((node) => node.id === connectedNode.id)) {
            leftNodes.push(connectedNode as ReactFlowNode)
            getConnectedNodes(connectedNode.id)
          }
        }
      }
      getConnectedNodes(currentNodeId)
      return [...leftNodes].reverse()
    },
    [edges, nodes]
  )

  const connectedNodes = useMemo(
    () => getLeftConnectedNodes(nodeId),
    [getLeftConnectedNodes, nodeId]
  )

  useEffect(() => {
    const inVariables = store.getNodeInVariables(nodeId)
    form.setFieldsValue({
      variables: inVariables.map((variable) => ({
        ...variable,
      })),
    })
  }, [nodeId, store, form])

  const getMenuItems = useCallback(
    (variable: FlowVariable) => {
      return connectedNodes.map((node) => {
        const nodeSettings = store.getNodeSettings(node.id)
        const outVariables = store.getNodeOutVariables(node.id)
        return {
          key: node.id,
          icon: <NodeIcon type={node.type} isFixed={nodeSettings.isFixed} />,
          label: nodeSettings.title,
          children: outVariables?.map((refVariable, index) => ({
            key: `${node.id}-${index}-${variable.name}`,
            label: (
              <>
                <span>{refVariable.name}</span>
                <Tag style={{ marginLeft: 5 }}>{refVariable.dataType}</Tag>
              </>
            ),
            onClick: () => {
              const variables = form.getFieldValue('variables')
              const updatedVariables = variables.map((v: FlowVariable) =>
                v === variable
                  ? {
                      ...v,
                      isReference: true,
                      referenceNodeId: node.id,
                      referenceName: refVariable.name,
                      dataType: refVariable.dataType,
                    }
                  : v
              )
              form.setFieldsValue({ variables: updatedVariables })
              store.setNodeInVariables(nodeId, updatedVariables)
              setModelIndexVisible(-1)
            },
          })),
        }
      })
    },
    [connectedNodes, store, form, nodeId]
  )

  const handleRemoveVariable = useCallback(
    (e: React.MouseEvent<HTMLButtonElement>, variable: FlowVariable) => {
      e.stopPropagation()
      const variables = form.getFieldValue('variables')
      const updatedVariables = variables.map((v: FlowVariable) =>
        v === variable
          ? {
              ...v,
              isReference: false,
              referenceNodeId: '',
              referenceName: '',
              dataType: VariableDataType.String,
            }
          : v
      )
      setModelIndexVisible(-1)
      form.setFieldsValue({ variables: updatedVariables })
      store.setNodeInVariables(nodeId, updatedVariables)
    },
    [form, nodeId, store]
  )

  const getElement = useCallback(
    (variable: FlowVariable, index: number) => {
      const variableNode = connectedNodes.find(
        (node) =>
          variable.isReference && variable.referenceNodeId + '' == node.id
      )
      if (variableNode && variable.referenceName) {
        const nodeSettings = store.getNodeSettings(variableNode.id)
        return (
          <Button
            style={{
              width: '100%',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'space-between',
              borderRadius: 4,
              overflow: 'hidden',
              cursor: 'pointer',
              border: '1px solid #d9d9d9',
            }}
            type='text'
            onClick={() =>
              modelIndexVisible === index
                ? setModelIndexVisible(-1)
                : setModelIndexVisible(index)
            }
          >
            <Space align='center'>
              <NodeIcon
                type={variableNode.type}
                isFixed={nodeSettings.isFixed}
              />
              <Typography.Text style={{ fontSize: 14, lineHeight: 16 }}>
                {nodeSettings.title} - {variable.referenceName}
                <Tag style={{ marginLeft: 5 }}>{variable.dataType}</Tag>
              </Typography.Text>
            </Space>
            <CloseOutlined
              onClick={(e) =>
                handleRemoveVariable(
                  e as React.MouseEvent<HTMLButtonElement>,
                  variable
                )
              }
            />
          </Button>
        )
      }
      return (
        <Input
          style={{ width: '100%' }}
          placeholder='输入或引用参数值'
          disabled={disabled}
          value={variable.value as string}
          onChange={(e) => {
            const variables = form.getFieldValue('variables')
            const updatedVariables = variables.map((v: FlowVariable) =>
              v === variable ? { ...v, value: e.target.value } : v
            )
            form.setFieldsValue({ variables: updatedVariables })
            store.setNodeInVariables(nodeId, updatedVariables)
          }}
          addonAfter={
            <SettingOutlined
              onClick={() =>
                modelIndexVisible === index
                  ? setModelIndexVisible(-1)
                  : setModelIndexVisible(index)
              }
            />
          }
        />
      )
    },
    [
      connectedNodes,
      disabled,
      handleRemoveVariable,
      modelIndexVisible,
      nodeId,
      store,
      form,
    ]
  )

  return (
    <Form.List name='variables'>
      {(fields, { remove }) => (
        <>
          {fields.map((field, index) => {
            const variable = form.getFieldValue(['variables', index])
            return (
              <div key={field.key} style={{ display: 'flex', marginBottom: 8 }}>
                <Form.Item
                  key={field.key}
                  name={[field.name, 'name']}
                  style={{ flex: 1, marginRight: 8, marginBottom: 0 }}
                  rules={[{ required: true, message: '请输入参数名' }]}
                >
                  <Input
                    disabled={disabled || variable?.isDisabled}
                    placeholder='输入参数名'
                    maxLength={20}
                  />
                </Form.Item>
                <Form.Item
                  key={`${field.key}-value`}
                  name={[field.name, 'value']}
                  style={{ flex: 2, marginRight: 8, marginBottom: 0 }}
                >
                  {getElement(variable, index)}
                </Form.Item>
                <Form.Item style={{ marginBottom: 0 }}>
                  <Button
                    disabled={disabled || variable?.isDisabled}
                    icon={<MinusCircleOutlined />}
                    type='text'
                    danger
                    onClick={() => {
                      remove(field.name)
                      const variables = form.getFieldValue('variables')
                      store.setNodeInVariables(nodeId, variables)
                    }}
                  />
                </Form.Item>
                {modelIndexVisible === index && (
                  <div
                    style={{
                      marginTop: 30,
                      padding: 0,
                      maxHeight: 300,
                      overflowY: 'auto',
                      border: '1px solid rgb(217, 217, 217)',
                      borderRadius: '8px',
                      position: 'absolute',
                      zIndex: 100,
                      backgroundColor: 'white',
                      width: 200,
                      right: 70,
                    }}
                  >
                    {connectedNodes.length === 0 && (
                      <Flex
                        style={{ width: '99%', height: 120, borderRadius: 6 }}
                        justify='center'
                        align='center'
                      >
                        <Typography.Text type='secondary' strong>
                          暂无数据
                        </Typography.Text>
                      </Flex>
                    )}
                    <Menu
                      style={{
                        width: '100%',
                        borderInlineEnd: 'none',
                      }}
                      mode='vertical'
                      items={getMenuItems(variable)}
                    />
                  </div>
                )}
              </div>
            )
          })}
        </>
      )}
    </Form.List>
  )
}

export default VariableReference
