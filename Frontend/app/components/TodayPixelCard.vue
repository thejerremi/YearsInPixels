<script setup lang="ts">
import type { DailyPixelDto } from '~/types/mood'
import { getMoodColor } from '~/utils/moodVisuals'

defineProps<{
  todayEntry: DailyPixelDto | null
}>()

const emit = defineEmits<{
  open: []
}>()

const { t } = useI18n()

const handleActivate = () => {
  emit('open')
}
</script>

<template>
  <VCard
    class="today-card dashboard-card today-card--interactive"
    elevation="0"
    rounded="xl"
    role="button"
    tabindex="0"
    :aria-label="t('home.today.openAria')"
    @click="handleActivate"
    @keydown.enter="handleActivate"
    @keydown.space.prevent="handleActivate"
  >
    <div class="today-card__halo" aria-hidden="true" />
    <VCardItem class="position-relative">
      <VCardTitle class="d-flex align-center ga-2">
        <VIcon color="primary" icon="mdi-calendar-today" />
        {{ $t('home.today.title') }}
      </VCardTitle>
      <VCardSubtitle>{{ $t('home.today.subtitle') }}</VCardSubtitle>
    </VCardItem>
    <VCardText class="position-relative">
      <template v-if="todayEntry">
        <div class="today-mood-row">
          <div
            class="today-score"
            :style="{ '--today-color': getMoodColor(todayEntry.moodValue) }"
          >
            <span>{{ todayEntry.moodValue }}</span>
            <small>/100</small>
          </div>
          <VChip
            class="today-word-chip"
            color="primary"
            prepend-icon="mdi-emoticon"
            variant="tonal"
          >
            {{ todayEntry.dayWord }}
          </VChip>
        </div>
        <p class="today-note text-body-2 text-medium-emphasis">
          {{ todayEntry.journalNote }}
        </p>
      </template>
      <div v-else class="empty-state">
        <VIcon color="primary" icon="mdi-plus-circle" size="42" />
        <p>{{ $t('home.today.empty') }}</p>
      </div>
    </VCardText>
  </VCard>
</template>

<style scoped src="~/assets/styles/today-pixel-card.css"></style>
