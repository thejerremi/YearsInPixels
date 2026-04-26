export type UserDto = {
  username: string
  password: string
}

export type AuthUser = {
  username: string
}

export type TokenResponseDto = {
  accessToken: string
  refreshToken: string
  userId: string
  username: string
}

export type RefreshTokenRequestDto = {
  userId: string
  refreshToken: string
}
