declare global {
  interface Window {
    wx?: {
      config: unknown
    }
  }
}

export function isMobile() {
  const isMobile = /Mobi|Android/i.test(navigator.userAgent)
  const isWeChat = /micromessenger/i.test(navigator.userAgent)
  const isMiniProgram = typeof window.wx !== 'undefined' && window.wx.config
  if (isMobile || isWeChat || isMiniProgram) {
    return true
  }
  return window.innerWidth < 800
}
