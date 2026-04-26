<script setup lang="ts">
const { locale, locales, setLocale, t } = useI18n()

const languageItems = computed(() => locales.value.map(item => ({
  title: item.name ?? item.code,
  value: item.code
})))

const handleLocaleChange = async (nextLocale: string) => {
  await setLocale(nextLocale)
}
</script>

<template>
  <VSelect
    class="language-switcher"
    density="compact"
    hide-details
    menu-icon="mdi-chevron-down"
    prepend-inner-icon="mdi-translate"
    :items="languageItems"
    :label="t('app.language')"
    :model-value="locale"
    rounded="lg"
    variant="solo-filled"
    @update:model-value="handleLocaleChange"
  />
</template>

<style scoped>
.language-switcher {
  width: 160px;
}

.language-switcher :deep(.v-field) {
  border: 1px solid rgba(124, 58, 237, 0.12);
  border-radius: 16px !important;
  background: rgba(124, 58, 237, 0.08) !important;
  box-shadow: none;
  backdrop-filter: blur(18px);
}

.language-switcher :deep(.v-field__overlay) {
  border-radius: inherit;
  opacity: 0;
}

.language-switcher :deep(.v-field__outline) {
  border-radius: inherit;
}

.language-switcher :deep(.v-field__input) {
  min-height: 42px;
  color: #4631a8;
  font-weight: 700;
}

.language-switcher :deep(.v-icon) {
  color: #7657f5;
}
</style>
