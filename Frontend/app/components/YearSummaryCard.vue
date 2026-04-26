<script setup lang="ts">
defineProps<{
  completedEntries: number
  completionPercent: number
  yearProgressDetail: string
  yearProgressPercent: number
}>()
</script>

<template>
  <VCard class="year-summary-card dashboard-card" elevation="0" rounded="xl">
    <VAvatar class="stat-card__icon" color="primary" variant="tonal">
      <VIcon icon="mdi-calendar-check" />
    </VAvatar>
    <div class="year-summary-card__content">
      <p class="stat-card__label">
        {{ $t('home.stats.yearSummary') }}
      </p>
      <div class="year-summary-card__metrics">
        <div class="year-summary-card__metric">
          <div class="year-summary-card__metric-heading">
            <span>{{ $t('home.stats.completed') }}</span>
            <strong>{{ completedEntries }}</strong>
          </div>
          <p class="stat-card__detail">
            {{ $t('home.stats.completion', { value: completionPercent }) }}
          </p>
          <VProgressLinear
            class="stat-card__progress"
            color="primary"
            height="7"
            :model-value="completionPercent"
            rounded
          />
        </div>
        <div class="year-summary-card__metric">
          <div class="year-summary-card__metric-heading">
            <span>{{ $t('home.stats.yearProgress') }}</span>
            <strong>{{ yearProgressPercent }}%</strong>
          </div>
          <p class="stat-card__detail">
            {{ yearProgressDetail }}
          </p>
          <VProgressLinear
            class="stat-card__progress"
            color="secondary"
            height="7"
            :model-value="yearProgressPercent"
            rounded
          />
        </div>
      </div>
    </div>
  </VCard>
</template>

<style scoped src="~/assets/styles/year-summary-card.css"></style>
