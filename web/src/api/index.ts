import axios, { AxiosError, type AxiosInstance } from 'axios'
import { addQuery, getQueryString } from '@/utils'
import { message } from 'antd'
import _ from 'lodash'

export const TOKEN_KEY = 'ssrag_access_token'

class Api {
  token: string
  private _api: AxiosInstance

  constructor() {
    this.token =
      getQueryString('access_token') ||
      sessionStorage.getItem(TOKEN_KEY) ||
      localStorage.getItem(TOKEN_KEY) ||
      ''

    this._api = axios.create({
      baseURL: '/api',
      headers: this.getHeaders(),
    })
  }

  getErrorMessage(error: Error): string {
    if (error instanceof AxiosError) {
      if (error.response) {
        // 服务器返回了响应，但状态码不在 2xx 范围内
        const data = error.response.data as { message?: string } | string
        const message =
          typeof data === 'object' ? data.message || JSON.stringify(data) : data
        return message
      } else if (error.request) {
        // 请求已发出，但未收到响应
        return `响应错误：${error.request}`
      }
      return error.message
    }
    return String(error)
  }

  async getResponse(url: string, data: Record<string, unknown>) {
    const response = await this._api.get(url, {
      params: data,
      responseType: 'blob',
    })
    return response
  }

  async get<T>(url: string, data?: Record<string, unknown>) {
    try {
      if (data) {
        const response = await this._api.get<T>(url, {
          params: data,
        })
        return response.data
      } else {
        const response = await this._api.get<T>(url)
        return response.data
      }
    } catch (error: unknown) {
      this.error(error as Error)
    }
    return null
  }

  async post<T>(url: string, data?: Record<string, unknown>) {
    try {
      if (data) {
        const response = await this._api.post<T>(url, data)
        return response.data
      } else {
        const response = await this._api.post<T>(url)
        return response.data
      }
    } catch (error: unknown) {
      this.error(error as Error)
    }
    return null
  }

  async upload<T>(url: string, data: FormData) {
    const response = await this._api.post<T>(url, data, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
    return response.data
  }

  getUploadUrl(url: string, params?: Record<string, unknown>) {
    return addQuery(`/api${url}`, params)
  }

  redirectToLogin() {
    const top = window.top || window
    const location = _.trimEnd(top.location.pathname, '/')
    if (location.endsWith('/ss-admin')) {
      top.location.href = addQuery('/ss-admin/login', {
        returnUrl: top.location.href,
      })
    } else {
      top.location.href = addQuery('/home/login', {
        status: 401,
        returnUrl: top.location.href,
      })
    }
  }

  error(error: Error) {
    if (error instanceof AxiosError) {
      if (error.status === 401) {
        this.redirectToLogin()
      } else if (error.status === 400 || error.status === 500) {
        message.open({
          type: 'error',
          content: error.response?.data?.message || '服务器错误',
        })
      }
    } else {
      // 使用防抖确保1秒内只调用一次
      _.debounce(
        () => {
          message.open({
            type: 'error',
            content: this.getErrorMessage(error),
          })
        },
        1000,
        { leading: true, trailing: false }
      )()
    }
  }

  getHeaders() {
    return {
      Authorization: `Bearer ${this.token}`,
    }
  }
}

const api = new Api()

export default api
