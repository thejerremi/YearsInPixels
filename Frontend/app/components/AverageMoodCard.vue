<script setup lang="ts">
defineProps<{
  detail: string
  icon: string
  label: string
  value: number | string
}>()
</script>

<template>
  <VCard class="average-mood-card dashboard-card" elevation="0" rounded="xl">
    <VAvatar class="average-mood-card__icon" color="primary" variant="tonal">
      <VIcon :icon="icon" />
    </VAvatar>
    <div>
      <p class="average-mood-card__label">
        {{ label }}
      </p>
      <p class="average-mood-card__value">
        {{ value }}
      </p>
      <p class="average-mood-card__detail">
        {{ detail }}
      </p>
    </div>
  </VCard>
</template>

<style scoped src="~/assets/styles/average-mood-card.css"></style>
