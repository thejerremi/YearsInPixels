import axios, { type AxiosError, type AxiosInstance, type InternalAxiosRequestConfig } from 'axios'

type ErrorResponse = {
  statusCode?: number
  message?: string
  errors?: string[]
}

type AccessTokenProvider = () => string | null | undefined

export const createApiClient = (getAccessToken?: AccessTokenProvider): AxiosInstance => {
  const config = useRuntimeConfig()

  const client = axios.create({
    baseURL: config.public.apiBaseUrl
  })

  if (getAccessToken) {
    client.interceptors.request.use((requestConfig: InternalAxiosRequestConfig) => {
      const token = getAccessToken()

      if (token) {
        requestConfig.headers.Authorization = `Bearer ${token}`
      }

      return requestConfig
    })
  }

  return client
}

export const getApiErrorMessage = (error: unknown, fallbackMessage: string) => {
  return getApiErrorMessages(error, fallbackMessage).join('\n')
}

export const getApiErrorMessages = (error: unknown, fallbackMessage: string) => {
  if (!axios.isAxiosError(error)) {
    return [error instanceof Error ? error.message : fallbackMessage]
  }

  const axiosError = error as AxiosError<ErrorResponse | string>

  if (typeof axiosError.response?.data === 'string') {
    return [axiosError.response.data || fallbackMessage]
  }

  const responseData = axiosError.response?.data

  if (responseData?.errors?.length) {
    return responseData.errors
  }

  if (responseData?.message) {
    return [responseData.message]
  }

  return [axiosError.message || fallbackMessage]
}

export const isUnauthorizedError = (error: unknown) => axios.isAxiosError(error) && error.response?.status === 401
