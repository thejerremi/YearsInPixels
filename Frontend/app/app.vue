<template>
  <VApp class="app-shell">
    <NuxtRouteAnnouncer />
    <VMain>
      <NuxtPage />
    </VMain>
    <AppToast />
  </VApp>
</template>

<style>
html {
  scroll-behavior: smooth;
}

body {
  background: #f5f7fb;
}

.app-shell {
  color: #172033;
}

.v-card {
  border: 1px solid rgba(124, 58, 237, 0.08);
}

.v-btn {
  letter-spacing: 0;
  text-transform: none;
}

.v-field {
  border-radius: 18px;
}
</style>
