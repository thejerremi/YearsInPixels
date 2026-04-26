import { defineStore } from 'pinia'
import { getApiErrorMessages, isUnauthorizedError } from '~/api/apiClient'
import { pixelService } from '~/services/pixelService'
import type { DailyPixelDto, PixelDay, RandomPixelDto, SaveDailyPixelInput } from '~/types/mood'
import { translateApiMessages } from '~/utils/apiErrorTranslations'
import { translate } from '~/utils/translate'
import { useToastStore } from '~/stores/toast'

const getDateKey = (date: Date | string) => {
  if (typeof date === 'string') {
    return date.slice(0, 10)
  }

  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')

  return `${year}-${month}-${day}`
}

const getApiDate = (dateKey: string) => `${dateKey}T00:00:00`
const randomFlashbackCooldownMs = 5000

const generateYearDays = (year: number, entries: DailyPixelDto[]): PixelDay[] => {
  const todayKey = getDateKey(new Date())
  const entryMap = new Map(entries.map(entry => [getDateKey(entry.date), entry]))
  const firstDay = new Date(year, 0, 1)
  const lastDay = new Date(year, 11, 31)
  const days: PixelDay[] = []

  for (
    const currentDate = new Date(firstDay);
    currentDate <= lastDay;
    currentDate.setDate(currentDate.getDate() + 1)
  ) {
    const date = new Date(currentDate)
    const dateKey = getDateKey(date)

    days.push({
      date,
      dateKey,
      dayOfYear: days.length + 1,
      isToday: dateKey === todayKey,
      isFuture: dateKey > todayKey,
      entry: entryMap.get(dateKey) ?? null
    })
  }

  return days
}

export const useMoodEntriesStore = defineStore('moodEntries', () => {
  const toastStore = useToastStore()
  const authStore = useAuthStore()
  const selectedYear = ref(new Date().getFullYear())
  const entries = ref<DailyPixelDto[]>([])
  const randomFlashback = ref<RandomPixelDto | null>(null)
  const isLoading = ref(false)
  const isSaving = ref(false)
  const isFetchingRandomFlashback = ref(false)
  const isRandomFlashbackCooldownActive = ref(false)
  const randomFlashbackCooldownSeconds = ref(0)
  const errorMessage = ref('')
  let randomFlashbackCooldownInterval: ReturnType<typeof setInterval> | null = null

  const yearDays = computed(() => generateYearDays(selectedYear.value, entries.value))
  const todayKey = computed(() => getDateKey(new Date()))
  const todayEntry = computed(() => entries.value.find(entry => getDateKey(entry.date) === todayKey.value) ?? null)

  const setApiError = (error: unknown, fallbackKey: string, showToast = false) => {
    const translatedMessages = translateApiMessages(getApiErrorMessages(error, translate(fallbackKey)), translate)

    errorMessage.value = translatedMessages.join('\n')

    if (showToast) {
      toastStore.showError(errorMessage.value)
    }
  }

  const getAccessToken = async () => {
    if (authStore.accessToken) {
      return authStore.accessToken
    }

    const refreshed = await authStore.refreshTokens()

    if (!refreshed || !authStore.accessToken) {
      throw new Error(translate('auth.errors.sessionExpired'))
    }

    return authStore.accessToken
  }

  const retryAfterRefresh = async <T>(request: (accessToken: string) => Promise<T>) => {
    const accessToken = await getAccessToken()

    try {
      return await request(accessToken)
    } catch (error) {
      if (!isUnauthorizedError(error)) {
        throw error
      }

      const refreshed = await authStore.refreshTokens()

      if (!refreshed || !authStore.accessToken) {
        throw error
      }

      return request(authStore.accessToken)
    }
  }

  const fetchEntriesForYear = async (year = selectedYear.value) => {
    selectedYear.value = year
    isLoading.value = true
    errorMessage.value = ''

    try {
      entries.value = await retryAfterRefresh(accessToken => pixelService.getYear(accessToken, year))
    } catch (error) {
      setApiError(error, 'home.errors.fetchYear')
      throw error
    } finally {
      isLoading.value = false
    }
  }

  const savePixel = async (dateKey: string, input: SaveDailyPixelInput) => {
    const payload: DailyPixelDto = {
      date: getApiDate(dateKey),
      moodValue: input.moodValue,
      dayWord: input.dayWord,
      journalNote: input.journalNote
    }
    const shouldFetchFlashbackAfterSave = !randomFlashback.value

    isSaving.value = true
    errorMessage.value = ''

    try {
      await retryAfterRefresh(accessToken => pixelService.addOrUpdate(accessToken, payload))

      const existingIndex = entries.value.findIndex(entry => getDateKey(entry.date) === dateKey)
      const isRandomFlashbackUpdated = randomFlashback.value
        ? getDateKey(randomFlashback.value.date) === dateKey
        : false

      if (isRandomFlashbackUpdated && randomFlashback.value) {
        randomFlashback.value = {
          ...payload,
          id: randomFlashback.value.id
        }
      }

      if (existingIndex >= 0) {
        entries.value.splice(existingIndex, 1, payload)
        return
      }

      entries.value.push(payload)

      if (shouldFetchFlashbackAfterSave) {
        await fetchRandomFlashback()
      }
    } catch (error) {
      setApiError(error, 'home.errors.saveToday', true)
      throw error
    } finally {
      isSaving.value = false
    }
  }

  const saveToday = async (input: SaveDailyPixelInput) => {
    await savePixel(todayKey.value, input)
  }

  const fetchRandomFlashback = async (excludedId?: number | null) => {
    isFetchingRandomFlashback.value = true

    try {
      randomFlashback.value = await retryAfterRefresh(accessToken => pixelService.getRandomFlashback(accessToken, excludedId))
    } catch (error) {
      randomFlashback.value = null

      if (!isUnauthorizedError(error)) {
        const messages = translateApiMessages(getApiErrorMessages(error, ''), translate)
        const message = messages.filter(Boolean).join('\n')

        if (message && message !== translate('apiErrors.noMemories')) {
          toastStore.showError(message)
        }
      }
    } finally {
      isFetchingRandomFlashback.value = false
    }
  }

  const startRandomFlashbackCooldown = () => {
    isRandomFlashbackCooldownActive.value = true
    randomFlashbackCooldownSeconds.value = randomFlashbackCooldownMs / 1000

    if (randomFlashbackCooldownInterval) {
      clearInterval(randomFlashbackCooldownInterval)
    }

    randomFlashbackCooldownInterval = setInterval(() => {
      randomFlashbackCooldownSeconds.value -= 1

      if (randomFlashbackCooldownSeconds.value > 0) {
        return
      }

      isRandomFlashbackCooldownActive.value = false
      randomFlashbackCooldownSeconds.value = 0

      if (randomFlashbackCooldownInterval) {
        clearInterval(randomFlashbackCooldownInterval)
        randomFlashbackCooldownInterval = null
      }
    }, 1000)
  }

  const rerollRandomFlashback = async () => {
    if (isFetchingRandomFlashback.value || isRandomFlashbackCooldownActive.value) {
      return
    }

    await fetchRandomFlashback(randomFlashback.value?.id)
    startRandomFlashbackCooldown()
  }

  return {
    selectedYear,
    entries,
    randomFlashback,
    isLoading,
    isSaving,
    isFetchingRandomFlashback,
    isRandomFlashbackCooldownActive,
    randomFlashbackCooldownSeconds,
    errorMessage,
    yearDays,
    todayEntry,
    fetchEntriesForYear,
    savePixel,
    saveToday,
    fetchRandomFlashback,
    rerollRandomFlashback
  }
})
