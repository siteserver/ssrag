import { useState } from 'react'
import api from '@/api'
import { Button } from 'antd'
import '@/assets/magic-button.css'
import IconMagic from '@/assets/others/magic.svg?react'

const MagicButton: React.FC<{
  style?: React.CSSProperties
  onClick: () => void
  children: React.ReactNode
}> = ({ style, onClick, children }) => {
  const [loading, setLoading] = useState(false)

  const handleClick = async () => {
    setLoading(true)
    try {
      if (onClick) {
        await onClick()
      }
    } catch (error) {
      api.error(error as Error)
    } finally {
      setLoading(false)
    }
  }

  return (
    <Button
      className='gradient-button'
      icon={<IconMagic />}
      type='primary'
      onClick={handleClick}
      loading={loading}
      style={style}
    >
      {children}
    </Button>
  )
}

export default MagicButton
