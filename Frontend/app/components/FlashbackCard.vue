<script setup lang="ts">
import type { RandomPixelDto } from '~/types/mood'

const props = defineProps<{
  cooldownSeconds: number
  flashback: RandomPixelDto | null
  isCooldownActive: boolean
  isFetching: boolean
}>()

const emit = defineEmits<{
  reroll: []
}>()

const { locale, t } = useI18n()

const flashbackDateLabel = computed(() => {
  if (!props.flashback) {
    return ''
  }

  return new Intl.DateTimeFormat(locale.value, {
    day: 'numeric',
    month: 'short',
    year: 'numeric'
  }).format(new Date(props.flashback.date))
})

const flashbackMoodLabel = computed(() => {
  if (!props.flashback) {
    return ''
  }

  return t('home.moodOutOf', { value: props.flashback.moodValue })
})

const handleReroll = () => {
  if (props.isFetching || props.isCooldownActive) {
    return
  }

  emit('reroll')
}
</script>

<template>
  <VCard class="flashback-card dashboard-card" elevation="0" rounded="xl">
    <VCardItem>
      <div class="flashback-card__header">
        <div class="flashback-card__heading">
          <VCardTitle class="d-flex align-center ga-2">
            <VIcon color="secondary" icon="mdi-auto-fix" />
            {{ $t('home.flashback.title') }}
          </VCardTitle>
          <VCardSubtitle>{{ $t('home.flashback.subtitle') }}</VCardSubtitle>
        </div>
        <VBtn
          v-if="flashback"
          :aria-label="$t('home.flashback.rerollAria')"
          class="flashback-card__reroll"
          color="secondary"
          :disabled="isCooldownActive"
          :loading="isFetching"
          size="small"
          variant="tonal"
          @click="handleReroll"
        >
          <span v-if="isCooldownActive">
            {{ cooldownSeconds }}
          </span>
          <VIcon v-else icon="mdi-refresh" />
        </VBtn>
      </div>
    </VCardItem>

    <VCardText>
      <template v-if="flashback">
        <VChip class="mb-3" color="secondary" variant="tonal">
          {{ flashbackDateLabel }}
        </VChip>
        <p class="flashback-card__title">
          {{ flashback.dayWord }}
          · {{ flashbackMoodLabel }}
        </p>
        <p class="text-body-2 text-medium-emphasis">
          {{ flashback.journalNote }}
        </p>
      </template>
      <div v-else class="empty-state">
        <VIcon color="secondary" icon="mdi-history" size="42" />
        <p>{{ $t('home.flashback.empty') }}</p>
      </div>
    </VCardText>
  </VCard>
</template>

<style scoped src="~/assets/styles/flashback-card.css"></style>
