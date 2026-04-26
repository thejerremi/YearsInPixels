<script setup lang="ts">
const toastStore = useToastStore()
</script>

<template>
  <VSnackbar
    v-model="toastStore.isVisible"
    :color="toastStore.type"
    location="bottom right"
    multi-line
    timeout="5000"
  >
    <span class="toast-message">{{ toastStore.message }}</span>

    <template #actions>
      <VBtn
        icon="mdi-close"
        variant="text"
        @click="toastStore.isVisible = false"
      />
    </template>
  </VSnackbar>
</template>

<style scoped>
.toast-message {
  white-space: pre-line;
}
</style>
