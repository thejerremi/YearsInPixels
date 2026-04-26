export type DailyPixelDto = {
  date: string
  moodValue: number
  dayWord: string
  journalNote: string
}

export type RandomPixelDto = DailyPixelDto & {
  id: number
}

export type PixelDay = {
  date: Date
  dateKey: string
  dayOfYear: number
  isToday: boolean
  isFuture: boolean
  entry: DailyPixelDto | null
}

export type SaveDailyPixelInput = {
  moodValue: number
  dayWord: string
  journalNote: string
}
