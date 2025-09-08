import { DragOutlined, DeleteOutlined } from '@ant-design/icons'
import { useSortable } from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'
import { Form, Input } from 'antd'

const SortableInput: React.FC<{
  name: string
  index: number
  placeholder: string
  onDelete: (index: number) => void
}> = ({ name, index, placeholder, onDelete }) => {
  const { attributes, listeners, setNodeRef, transform, transition } =
    useSortable({ id: index })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  }

  return (
    <div key={index} ref={setNodeRef} style={style}>
      <Form.Item name={[name, index]}>
        <Input
          placeholder={placeholder}
          addonBefore={
            <DragOutlined
              {...attributes}
              {...listeners}
              style={{ cursor: 'move', color: '#999' }}
            />
          }
          addonAfter={
            <DeleteOutlined
              style={{ cursor: 'pointer', color: '#999' }}
              onClick={() => onDelete(index)}
            />
          }
        />
      </Form.Item>
    </div>
  )
}

export default SortableInput
