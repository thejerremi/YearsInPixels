<script setup lang="ts">
import type { PixelDay, SaveDailyPixelInput } from '~/types/mood'
import { getMoodColor } from '~/utils/moodVisuals'

type MoodSuggestionRange = keyof typeof moodSuggestionKeys

const moodSuggestionKeys = {
  veryLow: ['heaviness', 'tiredness', 'overwhelm', 'sadness', 'tension', 'emptiness', 'helplessness', 'melancholy'],
  low: ['anxiety', 'weariness', 'chaos', 'scattered', 'irritation', 'sleepiness', 'reflection', 'lowEnergy'],
  middle: ['neutral', 'calm', 'steady', 'routine', 'balance', 'quiet', 'focus', 'ordinary'],
  high: ['lightness', 'energy', 'gratitude', 'motivation', 'smile', 'hope', 'satisfaction', 'goodRhythm'],
  veryHigh: ['joy', 'euphoria', 'flow', 'fulfillment', 'delight', 'pride', 'enthusiasm', 'greatDay']
} as const

const props = defineProps<{
  day: PixelDay | null
  isSaving: boolean
  modelValue: boolean
}>()

const emit = defineEmits<{
  close: []
  save: [input: SaveDailyPixelInput]
  'update:modelValue': [value: boolean]
}>()

const { locale, t } = useI18n()
const moodValue = ref(50)
const dayWord = ref('')
const journalNote = ref('')

const selectedDateLabel = computed(() => {
  if (!props.day) {
    return ''
  }

  return new Intl.DateTimeFormat(locale.value, {
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  }).format(props.day.date)
})

const dayWordRules = [
  (value: string) => Boolean(value.trim()) || t('home.validation.dayWordRequired'),
  (value: string) => value.trim().length <= 30 || t('home.validation.dayWordMax')
]

const journalRules = [
  (value: string) => value.length <= 2000 || t('home.validation.journalMax')
]

const isMoodFormValid = computed(() => Boolean(
  moodValue.value >= 0
  && moodValue.value <= 100
  && dayWord.value.trim().length > 0
  && dayWord.value.trim().length <= 30
  && journalNote.value.length <= 2000
))

const moodSuggestionRange = computed<MoodSuggestionRange>(() => {
  if (moodValue.value <= 20) {
    return 'veryLow'
  }

  if (moodValue.value <= 40) {
    return 'low'
  }

  if (moodValue.value <= 60) {
    return 'middle'
  }

  if (moodValue.value <= 80) {
    return 'high'
  }

  return 'veryHigh'
})

const moodSuggestions = computed(() => {
  return moodSuggestionKeys[moodSuggestionRange.value].map(key => t(`home.dialog.moodSuggestions.${key}`))
})

const moodPreviewStyle = computed(() => ({
  background: `linear-gradient(135deg, ${getMoodColor(moodValue.value)}, rgba(255, 255, 255, 0.68))`
}))

const resetFormFromDay = () => {
  moodValue.value = props.day?.entry?.moodValue ?? 50
  dayWord.value = props.day?.entry?.dayWord ?? ''
  journalNote.value = props.day?.entry?.journalNote ?? ''
}

const handleModelChange = (isOpen: boolean) => {
  emit('update:modelValue', isOpen)

  if (!isOpen) {
    emit('close')
  }
}

const handleClose = () => {
  handleModelChange(false)
}

const handleMoodSuggestionClick = (suggestion: string) => {
  dayWord.value = suggestion.slice(0, 30)
}

const handleSave = () => {
  if (!isMoodFormValid.value || props.isSaving) {
    return
  }

  emit('save', {
    moodValue: moodValue.value,
    dayWord: dayWord.value.trim(),
    journalNote: journalNote.value.trim()
  })
}

watch(() => props.day, resetFormFromDay, { immediate: true })
watch(() => props.modelValue, (isOpen) => {
  if (isOpen) {
    resetFormFromDay()
  }
})
</script>

<template>
  <VDialog
    :model-value="modelValue"
    max-width="920"
    @update:model-value="handleModelChange"
  >
    <VCard class="dialog-card" rounded="xl">
      <VRow no-gutters>
        <VCol cols="12" md="5">
          <div class="dialog-preview" :style="moodPreviewStyle">
            <VBtn
              class="dialog-preview__close"
              icon="mdi-close"
              variant="text"
              @click="handleClose"
            />
            <VChip
              v-if="selectedDateLabel"
              class="mb-6"
              color="white"
              prepend-icon="mdi-calendar-edit"
              variant="tonal"
            >
              {{ selectedDateLabel }}
            </VChip>
            <div class="dialog-preview__score">
              <span>{{ moodValue }}</span>
              <small>/100</small>
            </div>
            <div class="dialog-preview__word">
              {{ dayWord || $t('home.dialog.dayWordPlaceholder') }}
            </div>
          </div>
        </VCol>

        <VCol cols="12" md="7">
          <div class="dialog-form">
            <VCardItem class="px-0 pt-0">
              <VCardTitle class="text-h4 font-weight-bold">
                {{ $t('home.dialog.title') }}
              </VCardTitle>
              <VCardSubtitle>
                {{ $t('home.dialog.subtitle') }}
              </VCardSubtitle>
            </VCardItem>

            <VSlider
              v-model="moodValue"
              class="mt-5"
              color="primary"
              :label="$t('home.dialog.moodLabel')"
              max="100"
              min="0"
              :show-ticks="false"
              step="1"
              thumb-label="always"
            />

            <VTextField
              v-model="dayWord"
              counter="30"
              :label="$t('home.dialog.dayWord')"
              maxlength="30"
              prepend-inner-icon="mdi-emoticon-outline"
              :rules="dayWordRules"
              variant="outlined"
            />

            <div class="mood-suggestions">
              <p class="mood-suggestions__label">
                {{ $t('home.dialog.moodSuggestionsLabel') }}
              </p>
              <div class="mood-suggestions__chips">
                <VChip
                  v-for="suggestion in moodSuggestions"
                  :key="suggestion"
                  class="mood-suggestions__chip"
                  color="primary"
                  size="small"
                  variant="tonal"
                  @click="handleMoodSuggestionClick(suggestion)"
                >
                  {{ suggestion }}
                </VChip>
              </div>
            </div>

            <VTextarea
              v-model="journalNote"
              class="journal-textarea"
              counter="2000"
              :label="$t('home.dialog.journal')"
              maxlength="2000"
              prepend-inner-icon="mdi-notebook-edit-outline"
              :rules="journalRules"
              rows="7"
              variant="outlined"
            />

            <div class="dialog-footer">
              <div class="d-flex ga-2 justify-end">
                <VBtn
                  class="dialog-cancel"
                  :aria-label="$t('home.dialog.cancelAria')"
                  variant="text"
                  @click="handleClose"
                >
                  {{ $t('common.cancel') }}
                </VBtn>
                <VBtn
                  class="dialog-save"
                  :aria-label="$t('home.dialog.saveAria')"
                  color="primary"
                  :disabled="!isMoodFormValid"
                  :loading="isSaving"
                  prepend-icon="mdi-content-save"
                  variant="flat"
                  @click="handleSave"
                >
                  {{ $t('common.save') }}
                </VBtn>
              </div>
            </div>
          </div>
        </VCol>
      </VRow>
    </VCard>
  </VDialog>
</template>

<style scoped src="~/assets/styles/mood-entry-dialog.css"></style>
