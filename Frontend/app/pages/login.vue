<script setup lang="ts">
const authStore = useAuthStore()
const { t } = useI18n()
const localePath = useLocalePath()

const username = ref('')
const password = ref('')
const showPassword = ref(false)

const usernameRules = [
  (value: string) => Boolean(value.trim()) || t('auth.validation.usernameRequired'),
  (value: string) => value.trim().length >= 3 || t('auth.validation.usernameMin'),
  (value: string) => value.trim().length <= 20 || t('auth.validation.usernameMax')
]

const passwordRules = [
  (value: string) => Boolean(value) || t('auth.validation.passwordRequired'),
  (value: string) => value.length >= 6 || t('auth.validation.passwordMin')
]

const isFormValid = computed(() => Boolean(
  username.value.trim().length >= 3
  && username.value.trim().length <= 20
  && password.value.length >= 6
))

const handleSubmit = async () => {
  if (!isFormValid.value || authStore.isLoading) {
    return
  }

  await authStore.login({
    username: username.value.trim(),
    password: password.value
  })

  await navigateTo(localePath('/'))
}
</script>

<template>
  <VContainer class="auth-page d-flex align-center justify-center" fluid>
    <div class="auth-orb auth-orb--primary" />
    <div class="auth-orb auth-orb--secondary" />

    <VCard class="auth-card" elevation="0" rounded="xl">
      <div class="d-flex justify-end pa-4 pb-0">
        <LanguageSwitcher />
      </div>

      <VCardItem class="auth-card__header">
        <VChip class="mb-4" color="primary" prepend-icon="mdi-creation" variant="tonal">
          {{ $t('auth.login.eyebrow') }}
        </VChip>
        <VCardTitle class="text-h4 font-weight-bold">
          {{ $t('auth.login.title') }}
        </VCardTitle>
        <VCardSubtitle>
          {{ $t('auth.login.subtitle') }}
        </VCardSubtitle>
      </VCardItem>

      <VCardText>
        <div class="auth-features mb-6">
          <div class="auth-feature">
            <VIcon color="primary" icon="mdi-calendar-heart" />
            <span>{{ $t('auth.login.featureOne') }}</span>
          </div>
          <div class="auth-feature">
            <VIcon color="secondary" icon="mdi-gradient-horizontal" />
            <span>{{ $t('auth.login.featureTwo') }}</span>
          </div>
          <div class="auth-feature">
            <VIcon color="primary" icon="mdi-shield-lock" />
            <span>{{ $t('auth.login.featureThree') }}</span>
          </div>
        </div>

        <VAlert v-if="authStore.errorMessage" class="error-message mb-4" type="error" variant="tonal" role="alert">
          {{ authStore.errorMessage }}
        </VAlert>

        <VForm @submit.prevent="handleSubmit">
          <VTextField v-model="username" autocomplete="username" :label="$t('auth.fields.username')"
            prepend-inner-icon="mdi-account" :rules="usernameRules" variant="outlined" />

          <VTextField v-model="password" autocomplete="current-password" :label="$t('auth.fields.password')"
            prepend-inner-icon="mdi-lock" :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
            :rules="passwordRules" :type="showPassword ? 'text' : 'password'" variant="outlined"
            @click:append-inner="showPassword = !showPassword" />

          <VBtn :aria-label="$t('auth.login.submitAria')" block class="auth-submit mt-2" color="primary"
            :disabled="!isFormValid" :loading="authStore.isLoading" size="large" type="submit">
            {{ $t('auth.login.submit') }}
          </VBtn>
        </VForm>
      </VCardText>

      <VCardActions class="justify-center pb-6">
        <span class="text-body-2 text-medium-emphasis">{{ $t('auth.login.noAccount') }}</span>
        <VBtn :aria-label="$t('auth.login.registerAria')" color="primary" :to="localePath('/register')" variant="text">
          {{ $t('auth.login.registerLink') }}
        </VBtn>
      </VCardActions>
    </VCard>
  </VContainer>
</template>

<style scoped>
.auth-page {
  position: relative;
  overflow: hidden;
  min-height: 100vh;
  background:
    radial-gradient(circle at top left, rgba(124, 58, 237, 0.26), transparent 30rem),
    radial-gradient(circle at bottom right, rgba(6, 182, 212, 0.2), transparent 32rem),
    linear-gradient(135deg, #f8fbff 0%, #eef2ff 48%, #ecfeff 100%);
  padding: 24px;
}

.auth-card {
  position: relative;
  z-index: 1;
  width: min(100%, 440px);
  background: rgba(255, 255, 255, 0.78);
  border: 1px solid rgba(255, 255, 255, 0.72);
  box-shadow: 0 32px 90px rgba(31, 41, 55, 0.16);
  backdrop-filter: blur(24px);
  animation: auth-enter 520ms ease-out both;
}

.auth-card__header {
  padding-top: 10px;
}

.auth-features {
  display: grid;
  gap: 10px;
}

.auth-feature {
  display: flex;
  align-items: center;
  gap: 10px;
  border: 1px solid rgba(124, 58, 237, 0.1);
  border-radius: 18px;
  background: rgba(255, 255, 255, 0.58);
  padding: 10px 12px;
}

.auth-submit {
  min-height: 52px;
  border-radius: 18px;
  box-shadow: 0 18px 34px rgba(124, 58, 237, 0.28);
}

.auth-orb {
  position: absolute;
  border-radius: 999px;
  filter: blur(8px);
  opacity: 0.8;
  animation: float-orb 8s ease-in-out infinite;
}

.auth-orb--primary {
  top: 10%;
  left: 12%;
  width: 180px;
  height: 180px;
  background: rgba(124, 58, 237, 0.18);
}

.auth-orb--secondary {
  right: 10%;
  bottom: 12%;
  width: 220px;
  height: 220px;
  background: rgba(6, 182, 212, 0.16);
  animation-delay: -3s;
}

.error-message {
  white-space: pre-line;
}

@keyframes auth-enter {
  from {
    opacity: 0;
    transform: translateY(18px) scale(0.98);
  }

  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

@keyframes float-orb {

  0%,
  100% {
    transform: translate3d(0, 0, 0);
  }

  50% {
    transform: translate3d(16px, -18px, 0);
  }
}
</style>
