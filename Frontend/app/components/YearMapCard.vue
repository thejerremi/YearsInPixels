<script setup lang="ts">
import type { PixelDay } from '~/types/mood'
import { getMoodColor } from '~/utils/moodVisuals'

const moodEntriesStore = useMoodEntriesStore()
const { locale, t } = useI18n()

const emit = defineEmits<{
  'select-day': [day: PixelDay]
}>()

const getDateKey = (date: Date) => {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')

  return `${year}-${month}-${day}`
}

const getWeekStart = (date: Date) => {
  const dayOfWeek = date.getDay() || 7
  const weekStart = new Date(date)

  weekStart.setHours(0, 0, 0, 0)
  weekStart.setDate(date.getDate() - dayOfWeek + 1)

  return weekStart
}

const mobileWeekStart = ref(getWeekStart(new Date()))

const currentYear = computed(() => new Date().getFullYear())
const isTodayInSelectedYear = computed(() => moodEntriesStore.selectedYear === currentYear.value)

const mobileWeekDateKeys = computed(() => {
  return Array.from({ length: 7 }, (_, index) => {
    const date = new Date(mobileWeekStart.value)
    date.setDate(mobileWeekStart.value.getDate() + index)

    return getDateKey(date)
  })
})

const formatDate = (date: Date | string) => new Intl.DateTimeFormat(locale.value, {
  day: 'numeric',
  month: 'short',
  year: 'numeric'
}).format(typeof date === 'string' ? new Date(date) : date)

const mobileWeekLabel = computed(() => {
  const weekEnd = new Date(mobileWeekStart.value)
  weekEnd.setDate(mobileWeekStart.value.getDate() + 6)

  return `${formatDate(mobileWeekStart.value)} - ${formatDate(weekEnd)}`
})

const mobileMinWeekStart = computed(() => getWeekStart(new Date(moodEntriesStore.selectedYear, 0, 1)))
const mobileMaxWeekStart = computed(() => {
  const maxDate = moodEntriesStore.selectedYear === currentYear.value
    ? new Date()
    : new Date(moodEntriesStore.selectedYear, 11, 31)

  return getWeekStart(maxDate)
})

const canGoToPreviousMobileWeek = computed(() => mobileWeekStart.value > mobileMinWeekStart.value)
const canGoToNextMobileWeek = computed(() => mobileWeekStart.value < mobileMaxWeekStart.value)

const getPixelStyle = (day: PixelDay) => {
  if (day.entry) {
    return {
      background: getMoodColor(day.entry.moodValue)
    }
  }

  if (day.isToday && isTodayInSelectedYear.value) {
    return {
      background: 'linear-gradient(135deg, #ef4444, #f59e0b, #22c55e)'
    }
  }

  if (!day.entry) {
    return {
      background: '#e2e8f0'
    }
  }
}

const getPixelLabel = (day: PixelDay) => {
  const dateLabel = new Intl.DateTimeFormat(locale.value, {
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  }).format(day.date)

  if (!day.entry) {
    return `${dateLabel}: ${t('home.noEntry')}`
  }

  return `${dateLabel}: ${day.entry.dayWord}, ${t('home.moodOutOf', { value: day.entry.moodValue })}`
}

const getPixelDateLabel = (day: PixelDay) => new Intl.DateTimeFormat(locale.value, {
  day: 'numeric',
  month: 'long',
  year: 'numeric'
}).format(day.date)

const getPixelTooltipSummary = (day: PixelDay) => {
  if (!day.entry) {
    return t('home.tooltip.emptySummary')
  }

  return `${day.entry.dayWord} · ${t('home.moodOutOf', { value: day.entry.moodValue })}`
}

const canEditDay = (day: PixelDay) => !day.isFuture

const isVisibleInMobileWeek = (day: PixelDay) => {
  if (!isTodayInSelectedYear.value) {
    return true
  }

  return mobileWeekDateKeys.value.includes(day.dateKey)
}

const handlePreviousMobileWeek = () => {
  if (!canGoToPreviousMobileWeek.value) {
    return
  }

  const previousWeek = new Date(mobileWeekStart.value)
  previousWeek.setDate(mobileWeekStart.value.getDate() - 7)
  mobileWeekStart.value = previousWeek < mobileMinWeekStart.value ? mobileMinWeekStart.value : previousWeek
}

const handleNextMobileWeek = () => {
  if (!canGoToNextMobileWeek.value) {
    return
  }

  const nextWeek = new Date(mobileWeekStart.value)
  nextWeek.setDate(mobileWeekStart.value.getDate() + 7)
  mobileWeekStart.value = nextWeek > mobileMaxWeekStart.value ? mobileMaxWeekStart.value : nextWeek
}

const handleSelectDay = (day: PixelDay) => {
  if (!canEditDay(day)) {
    return
  }

  emit('select-day', day)
}
</script>

<template>
  <VCard class="year-canvas dashboard-card h-100" elevation="0" rounded="xl">
    <VCardItem class="pb-2">
      <div class="d-flex align-center justify-space-between flex-wrap ga-3">
        <div class="mt-2">
          <VCardTitle class="text-h5 font-weight-bold">
            {{ $t('home.canvas.title') }}
          </VCardTitle>
          <VCardSubtitle class="year-canvas__subtitle">
            {{ $t('home.canvas.subtitle') }}
          </VCardSubtitle>
        </div>
        <VChip color="secondary" prepend-icon="mdi-cursor-pointer" variant="tonal">
          {{ $t('home.canvas.hint') }}
        </VChip>
      </div>
    </VCardItem>

    <VCardText>
      <div>
        <VAlert
          v-if="moodEntriesStore.errorMessage"
          class="error-message mb-3"
          density="compact"
          role="alert"
          type="error"
          variant="tonal"
        >
          {{ moodEntriesStore.errorMessage }}
        </VAlert>
      </div>

      <VProgressLinear v-if="moodEntriesStore.isLoading" class="mb-4" color="primary" indeterminate />

      <div class="mood-legend mb-5" aria-hidden="true">
        <span>{{ $t('home.legendLow') }}</span>
        <div class="mood-legend__bar" />
        <span>{{ $t('home.legendHigh') }}</span>
        <div class="mood-legend__empty">
          <span class="mood-legend__empty-dot" />
          <span>{{ $t('home.legendEmpty') }}</span>
        </div>
      </div>

      <div v-if="isTodayInSelectedYear" class="mobile-week-nav mb-4">
        <VBtn
          :aria-label="$t('home.week.previous')"
          :disabled="!canGoToPreviousMobileWeek"
          icon="mdi-chevron-left"
          size="small"
          variant="tonal"
          @click="handlePreviousMobileWeek"
        />
        <span>{{ mobileWeekLabel }}</span>
        <VBtn
          :aria-label="$t('home.week.next')"
          :disabled="!canGoToNextMobileWeek"
          icon="mdi-chevron-right"
          size="small"
          variant="tonal"
          @click="handleNextMobileWeek"
        />
      </div>

      <div class="pixel-grid" :aria-label="$t('home.gridLabel')" role="list">
        <template v-for="day in moodEntriesStore.yearDays" :key="day.dateKey">
          <VTooltip
            v-if="canEditDay(day)"
            class="pixel-tooltip"
            content-class="pixel-tooltip-card"
            location="top"
            open-delay="180"
          >
            <template #activator="{ props }">
              <VBtn
                v-bind="props"
                :aria-label="getPixelLabel(day)"
                class="pixel-button"
                :class="{
                  'pixel-button--today': day.isToday && isTodayInSelectedYear && !day.entry,
                  'pixel-button--editable': canEditDay(day),
                  'pixel-button--filled': Boolean(day.entry),
                  'pixel-button--mobile-hidden': !isVisibleInMobileWeek(day)
                }"
                :style="getPixelStyle(day)"
                role="listitem"
                variant="flat"
                @click="handleSelectDay(day)"
              />
            </template>
            <div
              class="pixel-tooltip__content"
              :class="{ 'pixel-tooltip__content--empty': !day.entry }"
              :style="day.entry ? { '--tooltip-color': getMoodColor(day.entry.moodValue) } : undefined"
            >
              <div class="pixel-tooltip__header">
                <VIcon icon="mdi-calendar-blank" size="16" />
                <p class="pixel-tooltip__date">
                  {{ getPixelDateLabel(day) }}
                </p>
              </div>
              <div class="pixel-tooltip__summary">
                <VIcon :icon="day.entry ? 'mdi-emoticon' : 'mdi-plus-circle-outline'" size="16" />
                <span>{{ getPixelTooltipSummary(day) }}</span>
              </div>
              <p v-if="day.entry?.journalNote" class="pixel-tooltip__note">
                {{ day.entry.journalNote }}
              </p>
            </div>
          </VTooltip>
          <VBtn
            v-else
            :aria-label="getPixelLabel(day)"
            class="pixel-button"
            :class="{
              'pixel-button--filled': Boolean(day.entry),
              'pixel-button--future': day.isFuture,
              'pixel-button--mobile-hidden': !isVisibleInMobileWeek(day)
            }"
            disabled
            :style="getPixelStyle(day)"
            role="listitem"
            variant="flat"
          />
        </template>
      </div>
    </VCardText>
  </VCard>
</template>

<style scoped src="~/assets/styles/year-map-card.css"></style>
