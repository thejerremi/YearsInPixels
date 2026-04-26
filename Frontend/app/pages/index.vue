<script setup lang="ts">
import type { PixelDay, SaveDailyPixelInput } from '~/types/mood'

const moodEntriesStore = useMoodEntriesStore()
const { t } = useI18n()

const isDialogOpen = ref(false)
const selectedDay = ref<PixelDay | null>(null)
const displayedYear = ref(new Date().getFullYear())
const yearNavigationDebounceMs = 500
let yearNavigationDebounceTimeout: ReturnType<typeof setTimeout> | null = null
let yearFetchRequestId = 0

const selectedYearLabel = computed(() => String(displayedYear.value))
const currentYear = computed(() => new Date().getFullYear())
const completedEntries = computed(() => moodEntriesStore.entries.length)
const daysInSelectedYear = computed(() => {
  return new Date(displayedYear.value, 1, 29).getMonth() === 1 ? 366 : 365
})
const completionPercent = computed(() => {
  if (!daysInSelectedYear.value) {
    return 0
  }

  return Math.round((completedEntries.value / daysInSelectedYear.value) * 100)
})
const averageMood = computed(() => {
  if (!moodEntriesStore.entries.length) {
    return null
  }

  const moodSum = moodEntriesStore.entries.reduce((sum, entry) => sum + entry.moodValue, 0)

  return Math.round(moodSum / moodEntriesStore.entries.length)
})
const yearProgressPercent = computed(() => {
  if (displayedYear.value < currentYear.value) {
    return 100
  }

  const today = new Date()
  const yearStart = new Date(today.getFullYear(), 0, 1)
  const elapsedDays = Math.floor((today.getTime() - yearStart.getTime()) / 86_400_000) + 1

  return Math.min(100, Math.round((elapsedDays / daysInSelectedYear.value) * 100))
})
const yearProgressDetail = computed(() => {
  const dayOfYear = displayedYear.value < currentYear.value
    ? daysInSelectedYear.value
    : Math.round((yearProgressPercent.value / 100) * daysInSelectedYear.value)

  return t('home.stats.yearProgressDetail', {
    day: dayOfYear,
    total: daysInSelectedYear.value
  })
})
const isTodayInSelectedYear = computed(() => moodEntriesStore.selectedYear === currentYear.value)
const canGoToNextYear = computed(() => displayedYear.value < currentYear.value)

const averageMoodCard = computed(() => ({
  icon: 'mdi-chart-bell-curve',
  label: t('home.stats.average', { year: displayedYear.value }),
  value: averageMood.value ?? '-',
  detail: averageMood.value === null ? t('home.stats.noData') : t('home.moodOutOf', { value: averageMood.value })
}))

const resetDialogFromDay = (day: PixelDay) => {
  selectedDay.value = day
}

const handleOpenDayDialog = (day: PixelDay) => {
  resetDialogFromDay(day)
  isDialogOpen.value = true
}

const handleOpenTodayDialog = () => {
  if (!isTodayInSelectedYear.value) {
    return
  }

  const todayDay = moodEntriesStore.yearDays.find(day => day.isToday)

  if (!todayDay) {
    return
  }

  handleOpenDayDialog(todayDay)
}

const handleFetchRandomFlashback = async () => {
  await moodEntriesStore.rerollRandomFlashback()
}

const scheduleYearFetch = (year: number) => {
  if (yearNavigationDebounceTimeout) {
    clearTimeout(yearNavigationDebounceTimeout)
  }

  yearNavigationDebounceTimeout = setTimeout(async () => {
    yearNavigationDebounceTimeout = null
    const requestId = ++yearFetchRequestId

    try {
      await moodEntriesStore.fetchEntriesForYear(year)

      if (requestId === yearFetchRequestId) {
        displayedYear.value = moodEntriesStore.selectedYear
      }
    } catch {
      // The store exposes the error message for the page alert.
    }
  }, yearNavigationDebounceMs)
}

const handleYearChange = (year: number) => {
  if (year > currentYear.value) {
    return
  }

  displayedYear.value = year
  scheduleYearFetch(year)
}

const handlePreviousYear = () => {
  handleYearChange(displayedYear.value - 1)
}

const handleNextYear = () => {
  handleYearChange(displayedYear.value + 1)
}

const handleSaveSelectedDay = async (input: SaveDailyPixelInput) => {
  if (!selectedDay.value || moodEntriesStore.isSaving) {
    return
  }

  await moodEntriesStore.savePixel(selectedDay.value.dateKey, input)

  isDialogOpen.value = false
  selectedDay.value = null
}

const handleCloseDialog = () => {
  isDialogOpen.value = false
  selectedDay.value = null
}

onMounted(async () => {
  try {
    await moodEntriesStore.fetchEntriesForYear(new Date().getFullYear())
    displayedYear.value = moodEntriesStore.selectedYear
    await moodEntriesStore.fetchRandomFlashback()
  } catch {
    // The store exposes the error message for the page alert.
  }
})

onBeforeUnmount(() => {
  if (yearNavigationDebounceTimeout) {
    clearTimeout(yearNavigationDebounceTimeout)
  }
})
</script>

<template>
  <VContainer class="home-page" fluid>
    <div class="dashboard-orb dashboard-orb--primary" />
    <div class="dashboard-orb dashboard-orb--secondary" />

    <div class="main-content">
      <VContainer class="pa-0" fluid>
        <VRow class="dashboard-grid">
          <VCol cols="12">
            <YearToolbarCard
              :can-go-to-next-year="canGoToNextYear"
              :selected-year-label="selectedYearLabel"
              @next-year="handleNextYear"
              @previous-year="handlePreviousYear"
            />
          </VCol>

          <VCol cols="12" md="8">
            <YearSummaryCard
              :completed-entries="completedEntries"
              :completion-percent="completionPercent"
              :year-progress-detail="yearProgressDetail"
              :year-progress-percent="yearProgressPercent"
            />
          </VCol>

          <VCol cols="12" md="4">
            <AverageMoodCard
              :detail="averageMoodCard.detail"
              :icon="averageMoodCard.icon"
              :label="averageMoodCard.label"
              :value="averageMoodCard.value"
            />
          </VCol>

          <VCol cols="12" lg="8">
            <YearMapCard @select-day="handleOpenDayDialog" />
          </VCol>

          <VCol cols="12" lg="4">
            <div class="insight-stack">
              <TodayPixelCard
                v-if="isTodayInSelectedYear"
                :today-entry="moodEntriesStore.todayEntry"
                @open="handleOpenTodayDialog"
              />

              <FlashbackCard
                :cooldown-seconds="moodEntriesStore.randomFlashbackCooldownSeconds"
                :flashback="moodEntriesStore.randomFlashback"
                :is-cooldown-active="moodEntriesStore.isRandomFlashbackCooldownActive"
                :is-fetching="moodEntriesStore.isFetchingRandomFlashback"
                @reroll="handleFetchRandomFlashback"
              />
            </div>
          </VCol>
        </VRow>
      </VContainer>
    </div>

    <MoodEntryDialog
      v-model="isDialogOpen"
      :day="selectedDay"
      :is-saving="moodEntriesStore.isSaving"
      @close="handleCloseDialog"
      @save="handleSaveSelectedDay"
    />
  </VContainer>
</template>

<style scoped src="~/assets/styles/home-dashboard.css"></style>
