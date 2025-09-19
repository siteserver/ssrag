import { DragOutlined, DeleteOutlined } from '@ant-design/icons'
import { useSortable } from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'
import { Input } from 'antd'

const SortableInput: React.FC<{
  index: number
  text: string
  onBlur: (text: string) => void
  onDelete: (index: number) => void
}> = ({ index, text, onBlur, onDelete }) => {
  const { attributes, listeners, setNodeRef, transform, transition } =
    useSortable({ id: index })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  }

  return (
    <div ref={setNodeRef} style={style} className='mb-2'>
      <Input
        placeholder='请输入提示词'
        defaultValue={text}
        onBlur={(e) => onBlur(e.target.value)}
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
    </div>
  )
}

export default SortableInput
