import axios, { type AxiosError, type AxiosInstance } from 'axios'

type ErrorResponse = {
  statusCode?: number
  message?: string
  errors?: string[]
}

export const createApiClient = (accessToken?: string | null): AxiosInstance => {
  const config = useRuntimeConfig()

  const client = axios.create({
    baseURL: config.public.apiBaseUrl
  })

  if (accessToken) {
    client.defaults.headers.common.Authorization = `Bearer ${accessToken}`
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
