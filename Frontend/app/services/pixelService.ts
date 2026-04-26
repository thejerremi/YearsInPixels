import { createApiClient } from '~/api/apiClient'
import type { DailyPixelDto, RandomPixelDto } from '~/types/mood'

export const pixelService = {
  async getYear(accessToken: string, year: number) {
    const response = await createApiClient(() => accessToken).get<DailyPixelDto[]>(`/api/Pixels/year/${year}`)

    return response.data
  },

  async addOrUpdate(accessToken: string, pixel: DailyPixelDto) {
    const response = await createApiClient(() => accessToken).post<{ message: string }>('/api/Pixels/addOrUpdate', pixel)

    return response.data
  },

  async getRandomFlashback(accessToken: string, excludedId?: number | null) {
    const endpoint = excludedId !== undefined && excludedId !== null
      ? `/api/Pixels/random-flashback/${excludedId}`
      : '/api/Pixels/random-flashback'
    const response = await createApiClient(() => accessToken).get<RandomPixelDto>(endpoint)

    return response.data
  }
}
