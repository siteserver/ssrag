import { useState } from 'react'
import { NodeType } from '@/enums'
import { getNodeTypeName } from '@/enums'
import { Segmented, Divider } from 'antd'
import NodeIcon from './NodeIcon'

const AddNodes: React.FC<{
  onNodeClick: (nodeType: NodeType) => void
}> = ({ onNodeClick }) => {
  const [segment, setSegment] = useState('Basic')

  const handleDragStart = (
    event: React.DragEvent<HTMLDivElement>,
    nodeType: NodeType
  ) => {
    event.dataTransfer.setData('application/reactflow', nodeType)
    event.dataTransfer.effectAllowed = 'move'
  }

  const basicNodes = (
    <div className='node-add-container'>
      <div className='node-add-divider'>
        <Divider orientation='left'>AI</Divider>
      </div>
      <div className='node-add-elements'>
        <div
          draggable
          onClick={() => onNodeClick(NodeType.LLM)}
          onDragStart={(event) => handleDragStart(event, NodeType.LLM)}
          className='node-add-element'
        >
          <div className='node-add-icon-container'>
            <NodeIcon type={NodeType.LLM} className='node-add-icon' />
          </div>
          <div className='node-add-text'>{getNodeTypeName(NodeType.LLM)}</div>
        </div>
        <div
          draggable
          onClick={() => onNodeClick(NodeType.Dataset)}
          onDragStart={(event) => handleDragStart(event, NodeType.Dataset)}
          className='node-add-element'
        >
          <div className='node-add-icon-container'>
            <NodeIcon type={NodeType.Dataset} className='node-add-icon' />
          </div>
          <div className='node-add-text'>
            {getNodeTypeName(NodeType.Dataset)}
          </div>
        </div>
        <div
          draggable
          onClick={() => onNodeClick(NodeType.Intent)}
          onDragStart={(event) => handleDragStart(event, NodeType.Intent)}
          className='node-add-element'
        >
          <div className='node-add-icon-container'>
            <NodeIcon type={NodeType.Intent} className='node-add-icon' />
          </div>
          <div className='node-add-text'>
            {getNodeTypeName(NodeType.Intent)}
          </div>
        </div>
        <div
          draggable
          onClick={() => onNodeClick(NodeType.Ask)}
          onDragStart={(event) => handleDragStart(event, NodeType.Ask)}
          className='node-add-element'
        >
          <div className='node-add-icon-container'>
            <NodeIcon type={NodeType.Ask} className='node-add-icon' />
          </div>
          <div className='node-add-text'>{getNodeTypeName(NodeType.Ask)}</div>
        </div>
      </div>
      <div className='node-add-divider'>
        <Divider orientation='left'>搜索</Divider>
      </div>
      <div className='node-add-elements'>
        <div
          draggable
          onClick={() => onNodeClick(NodeType.WebSearch)}
          onDragStart={(event) => handleDragStart(event, NodeType.WebSearch)}
          className='node-add-element'
        >
          <div className='node-add-icon-container'>
            <NodeIcon type={NodeType.WebSearch} className='node-add-icon' />
          </div>
          <div className='node-add-text'>
            {getNodeTypeName(NodeType.WebSearch)}
          </div>
        </div>
        <div
          draggable
          onClick={() => onNodeClick(NodeType.Sql)}
          onDragStart={(event) => handleDragStart(event, NodeType.Sql)}
          className='node-add-element'
        >
          <div className='node-add-icon-container'>
            <NodeIcon type={NodeType.Sql} className='node-add-icon' />
          </div>
          <div className='node-add-text'>{getNodeTypeName(NodeType.Sql)}</div>
        </div>
      </div>
      <div className='node-add-divider'>
        <Divider orientation='left'>输入输出</Divider>
      </div>
      <div className='node-add-elements'>
        <div
          draggable
          onClick={() => onNodeClick(NodeType.Input)}
          onDragStart={(event) => handleDragStart(event, NodeType.Input)}
          className='node-add-element'
        >
          <div className='node-add-icon-container'>
            <NodeIcon type={NodeType.Input} className='node-add-icon' />
          </div>
          <div className='node-add-text'>{getNodeTypeName(NodeType.Input)}</div>
        </div>
        <div
          draggable
          onClick={() => onNodeClick(NodeType.Output)}
          onDragStart={(event) => handleDragStart(event, NodeType.Output)}
          className='node-add-element'
        >
          <div className='node-add-icon-container'>
            <NodeIcon type={NodeType.Output} className='node-add-icon' />
          </div>
          <div className='node-add-text'>
            {getNodeTypeName(NodeType.Output)}
          </div>
        </div>
        <div
          draggable
          onClick={() => onNodeClick(NodeType.Text)}
          onDragStart={(event) => handleDragStart(event, NodeType.Text)}
          className='node-add-element'
        >
          <div className='node-add-icon-container'>
            <NodeIcon type={NodeType.Text} className='node-add-icon' />
          </div>
          <div className='node-add-text'>{getNodeTypeName(NodeType.Text)}</div>
        </div>
      </div>
    </div>
  )

  const toolNodes = (
    <div className='node-add-container'>
      <div className='node-add-divider'>
        <Divider orientation='left'>接口</Divider>
      </div>
      <div className='node-add-elements'>
        <div
          draggable
          onClick={() => onNodeClick(NodeType.Http)}
          onDragStart={(event) => handleDragStart(event, NodeType.Http)}
          className='node-add-element'
        >
          <div className='node-add-icon-container'>
            <NodeIcon type={NodeType.Http} className='node-add-icon' />
          </div>
          <div className='node-add-text'>{getNodeTypeName(NodeType.Http)}</div>
        </div>
      </div>
    </div>
  )

  return (
    <>
      <Segmented
        options={[
          {
            label: '基础功能',
            value: 'Basic',
          },
          {
            label: '工具箱',
            value: 'Tools',
          },
        ]}
        size='large'
        block
        onChange={(value) => {
          setSegment(value)
        }}
      />
      {segment === 'Basic' && basicNodes}
      {segment === 'Tools' && toolNodes}
    </>
  )
}

export default AddNodes
