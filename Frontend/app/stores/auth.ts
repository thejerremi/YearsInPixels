import { defineStore } from 'pinia'
import { authService } from '~/services/authService'
import { getApiErrorMessages, isUnauthorizedError } from '~/api/apiClient'
import { useToastStore } from '~/stores/toast'
import type { AuthUser, TokenResponseDto, UserDto } from '~/types/auth'
import { translateApiMessages } from '~/utils/apiErrorTranslations'
import { translate } from '~/utils/translate'

export const useAuthStore = defineStore('auth', () => {
  const toastStore = useToastStore()
  const accessTokenCookie = useCookie<string | null>('years_access_token', { sameSite: 'lax' })
  const refreshTokenCookie = useCookie<string | null>('years_refresh_token', { sameSite: 'lax' })
  const userIdCookie = useCookie<string | null>('years_user_id', { sameSite: 'lax' })
  const usernameCookie = useCookie<string | null>('years_username', { sameSite: 'lax' })

  const user = ref<AuthUser | null>(null)
  const accessToken = ref<string | null>(accessTokenCookie.value)
  const refreshToken = ref<string | null>(refreshTokenCookie.value)
  const isLoading = ref(false)
  const errorMessage = ref('')

  const isAuthenticated = computed(() => Boolean(accessToken.value))

  const setApiError = (error: unknown, fallbackKey: string, showToast = false) => {
    const translatedMessages = translateApiMessages(getApiErrorMessages(error, translate(fallbackKey)), translate)

    errorMessage.value = translatedMessages.join('\n')

    if (showToast) {
      toastStore.showError(errorMessage.value)
    }
  }

  const persistSession = (response: TokenResponseDto) => {
    accessToken.value = response.accessToken
    refreshToken.value = response.refreshToken
    user.value = { username: response.username }

    accessTokenCookie.value = response.accessToken
    refreshTokenCookie.value = response.refreshToken
    userIdCookie.value = response.userId
    usernameCookie.value = response.username
  }

  const loadSessionFromCookies = () => {
    accessToken.value = accessTokenCookie.value
    refreshToken.value = refreshTokenCookie.value

    if (!accessToken.value) {
      user.value = null
      return
    }

    user.value = {
      username: usernameCookie.value ?? ''
    }
  }

  const clearSession = () => {
    user.value = null
    accessToken.value = null
    refreshToken.value = null
    errorMessage.value = ''

    accessTokenCookie.value = null
    refreshTokenCookie.value = null
    userIdCookie.value = null
    usernameCookie.value = null
  }

  const login = async (credentials: UserDto) => {
    isLoading.value = true
    errorMessage.value = ''

    try {
      const response = await authService.login(credentials)

      persistSession(response)
    } catch (error) {
      setApiError(error, 'auth.errors.loginFailed')
      throw error
    } finally {
      isLoading.value = false
    }
  }

  const register = async (credentials: UserDto) => {
    isLoading.value = true
    errorMessage.value = ''

    try {
      const response = await authService.register(credentials)

      persistSession(response)
    } catch (error) {
      setApiError(error, 'auth.errors.registerFailed')
      throw error
    } finally {
      isLoading.value = false
    }
  }

  const refreshTokens = async () => {
    const currentUserId = userIdCookie.value

    if (!currentUserId || !refreshToken.value) {
      clearSession()
      return false
    }

    try {
      const response = await authService.refreshToken({
        userId: currentUserId,
        refreshToken: refreshToken.value
      })

      persistSession(response)
      return true
    } catch {
      clearSession()
      return false
    }
  }

  const checkAuthenticated = async () => {
    if (!accessToken.value) {
      return false
    }

    try {
      await authService.checkAuthenticated(accessToken.value)
      return true
    } catch (error) {
      if (!isUnauthorizedError(error)) {
        clearSession()
        setApiError(error, 'auth.errors.sessionExpired', true)
        return false
      }

      const isSessionRefreshed = await refreshTokens()

      if (!isSessionRefreshed) {
        setApiError(error, 'auth.errors.sessionExpired', true)
      }

      return isSessionRefreshed
    }
  }

  const logout = () => {
    clearSession()
    return navigateTo('/login')
  }

  return {
    user,
    accessToken,
    refreshToken,
    isAuthenticated,
    isLoading,
    errorMessage,
    loadSessionFromCookies,
    login,
    register,
    refreshTokens,
    checkAuthenticated,
    logout
  }
})
