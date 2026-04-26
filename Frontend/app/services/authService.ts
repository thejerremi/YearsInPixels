import { createApiClient } from '~/services/apiClient'
import type { RefreshTokenRequestDto, TokenResponseDto, UserDto } from '~/types/auth'

type AuthPingResponseDto = {
  message: string
  timestamp: string
}

export const authService = {
  async register(credentials: UserDto) {
    const response = await createApiClient().post<TokenResponseDto>('/api/auth/register', credentials)

    return response.data
  },

  async login(credentials: UserDto) {
    const response = await createApiClient().post<TokenResponseDto>('/api/auth/login', credentials)

    return response.data
  },

  async refreshToken(payload: RefreshTokenRequestDto) {
    const response = await createApiClient().post<TokenResponseDto>('/api/auth/refresh-token', payload)

    return response.data
  },

  async checkAuthenticated(accessToken: string) {
    const response = await createApiClient(accessToken).get<AuthPingResponseDto>('/api/auth')

    return response.data
  }
}
