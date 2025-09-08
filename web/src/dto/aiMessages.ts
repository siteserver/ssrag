export enum AiMessageStatus {
  Local = 'local',
  AI = 'ai',
  Loading = 'loading',
}

// export interface Message {
//   role: string
//   content: string
// }

export interface AiParameters {
  thinking: boolean
  searching: boolean
}

export interface AiMessage {
  id: string
  status: AiMessageStatus
  message: string
}

export interface UnsplashResult {
  cover_photo: UnsplashPhoto
}

export interface UnsplashPhoto {
  urls: {
    regular: string
  }
}
