import Icon3D from '@/assets/files/3d.svg?react'
import IconAudio from '@/assets/files/audio.svg?react'
import IconCode from '@/assets/files/code.svg?react'
import IconDatabase from '@/assets/files/database.svg?react'
import IconDoc from '@/assets/files/doc.svg?react'
import IconFolder from '@/assets/files/folder.svg?react'
import IconHtml from '@/assets/files/html.svg?react'
import IconImage from '@/assets/files/image.svg?react'
import IconPdf from '@/assets/files/pdf.svg?react'
import IconPpt from '@/assets/files/ppt.svg?react'
import IconTxt from '@/assets/files/txt.svg?react'
import IconUnknown from '@/assets/files/unknown.svg?react'
import IconVideo from '@/assets/files/video.svg?react'
import IconXls from '@/assets/files/xls.svg?react'
import IconZip from '@/assets/files/zip.svg?react'

const FileIcon: React.FC<{
  type: string
  className?: string
  style?: React.CSSProperties
}> = ({ type, className, style }) => {
  if (type === '.3d') {
    return <Icon3D className={className} style={style} />
  } else if (type === '.mp3') {
    return <IconAudio className={className} style={style} />
  } else if (type === '.js') {
    return <IconCode className={className} style={style} />
  } else if (type === '.db') {
    return <IconDatabase className={className} style={style} />
  } else if (type === '.doc' || type === '.docx') {
    return <IconDoc className={className} style={style} />
  } else if (type === 'folder') {
    return <IconFolder className={className} style={style} />
  } else if (type === '.htm' || type === '.html') {
    return <IconHtml className={className} />
  } else if (type === '.png') {
    return <IconImage className={className} style={style} />
  } else if (type === '.pdf') {
    return <IconPdf className={className} style={style} />
  } else if (type === '.ppt' || type === '.pptx') {
    return <IconPpt className={className} style={style} />
  } else if (type === '.txt') {
    return <IconTxt className={className} style={style} />
  } else if (type === '.mp4') {
    return <IconVideo className={className} style={style} />
  } else if (type === '.xls') {
    return <IconXls className={className} style={style} />
  } else if (type === '.zip') {
    return <IconZip className={className} style={style} />
  }
  return <IconUnknown className={className} style={style} />
}

export default FileIcon
