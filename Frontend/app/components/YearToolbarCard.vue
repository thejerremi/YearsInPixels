<script setup lang="ts">
const authStore = useAuthStore()

defineProps<{
  canGoToNextYear: boolean
  selectedYearLabel: string
}>()

const emit = defineEmits<{
  'next-year': []
  'previous-year': []
}>()

const handlePreviousYear = () => {
  emit('previous-year')
}

const handleNextYear = () => {
  emit('next-year')
}

const handleLogout = () => {
  authStore.logout()
}
</script>

<template>
  <VCard class="year-toolbar dashboard-card" elevation="0" rounded="xl">
    <div>
      <VChip
        class="year-toolbar__eyebrow mb-2"
        prepend-icon="mdi-chart-timeline-variant"
        variant="flat"
      >
        {{ $t('home.eyebrow') }}
      </VChip>
      <div class="year-toolbar__title-row">
        <h1 class="year-toolbar__title">
          {{ selectedYearLabel }}
        </h1>
        <div class="d-flex flex-wrap ga-2">
          <VBtn
            class="year-control year-control--previous"
            :aria-label="$t('home.previousYear')"
            prepend-icon="mdi-chevron-left"
            variant="flat"
            @click="handlePreviousYear"
          >
            {{ $t('home.previousYearShort') }}
          </VBtn>
          <VBtn
            class="year-control"
            color="primary"
            :aria-label="$t('home.nextYear')"
            append-icon="mdi-chevron-right"
            :disabled="!canGoToNextYear"
            variant="flat"
            @click="handleNextYear"
          >
            {{ $t('home.nextYearShort') }}
          </VBtn>
        </div>
      </div>
      <p class="year-toolbar__copy">
        {{ $t('home.yearSubtitle') }}
      </p>
    </div>
    <div class="year-toolbar__meta">
      <div class="year-toolbar__brand">
        <div class="year-canvas__heading">
          <p class="year-toolbar__app-name">
            {{ $t('app.name') }}
          </p>
          <p class="year-toolbar__user">
            {{ authStore.user?.username }}
          </p>
        </div>
      </div>
      <div class="year-toolbar__actions">
        <LanguageSwitcher />
        <VBtn
          class="year-control year-control--logout"
          :aria-label="$t('common.logout')"
          prepend-icon="mdi-logout"
          variant="flat"
          @click="handleLogout"
        >
          {{ $t('common.logout') }}
        </VBtn>
      </div>
    </div>
  </VCard>
</template>

<style scoped src="~/assets/styles/year-toolbar-card.css"></style>
