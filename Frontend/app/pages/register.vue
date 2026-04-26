<script setup lang="ts">
const authStore = useAuthStore()
const { t } = useI18n()
const localePath = useLocalePath()

const username = ref('')
const password = ref('')
const confirmPassword = ref('')
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

const confirmPasswordRules = [
  (value: string) => value === password.value || t('auth.validation.passwordMatch')
]

const isFormValid = computed(() => Boolean(
  username.value.trim().length >= 3
  && username.value.trim().length <= 20
  && password.value.length >= 6
  && password.value === confirmPassword.value
))

const handleSubmit = async () => {
  if (!isFormValid.value || authStore.isLoading) {
    return
  }

  const credentials = {
    username: username.value.trim(),
    password: password.value
  }

  await authStore.register(credentials)
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
        <VCardTitle class="text-h4 font-weight-bold">
          {{ $t('auth.register.title') }}
        </VCardTitle>
        <VCardSubtitle class="w-100" style="white-space: normal; overflow: visible; word-break: break-word;">
          {{ $t('auth.register.subtitle') }}
        </VCardSubtitle>

      </VCardItem>

      <VCardText>
        <div class="auth-features mb-6">
          <div class="auth-feature">
            <VIcon color="primary" icon="mdi-notebook-edit" />
            <span>{{ $t('auth.register.featureOne') }}</span>
          </div>
          <div class="auth-feature">
            <VIcon color="secondary" icon="mdi-view-grid-plus" />
            <span>{{ $t('auth.register.featureTwo') }}</span>
          </div>
          <div class="auth-feature">
            <VIcon color="primary" icon="mdi-history" />
            <span>{{ $t('auth.register.featureThree') }}</span>
          </div>
        </div>

        <VAlert v-if="authStore.errorMessage" class="error-message mb-4" type="error" variant="tonal" role="alert">
          {{ authStore.errorMessage }}
        </VAlert>

        <VForm @submit.prevent="handleSubmit">
          <VTextField v-model="username" autocomplete="username" :label="$t('auth.fields.username')"
            prepend-inner-icon="mdi-account-plus" :rules="usernameRules" variant="outlined" />

          <VTextField v-model="password" autocomplete="new-password" :label="$t('auth.fields.password')"
            prepend-inner-icon="mdi-lock" :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
            :rules="passwordRules" :type="showPassword ? 'text' : 'password'" variant="outlined"
            @click:append-inner="showPassword = !showPassword" />

          <VTextField v-model="confirmPassword" autocomplete="new-password" :label="$t('auth.fields.confirmPassword')"
            prepend-inner-icon="mdi-lock-check" :rules="confirmPasswordRules" :type="showPassword ? 'text' : 'password'"
            variant="outlined" />

          <VBtn :aria-label="$t('auth.register.submitAria')" block class="auth-submit mt-2" color="primary"
            :disabled="!isFormValid" :loading="authStore.isLoading" size="large" type="submit">
            {{ $t('auth.register.submit') }}
          </VBtn>
        </VForm>
      </VCardText>

      <VCardActions class="justify-center pb-6">
        <span class="text-body-2 text-medium-emphasis">{{ $t('auth.register.hasAccount') }}</span>
        <VBtn :aria-label="$t('auth.register.loginAria')" color="primary" :to="localePath('/login')" variant="text">
          {{ $t('auth.register.loginLink') }}
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
    radial-gradient(circle at top right, rgba(6, 182, 212, 0.22), transparent 30rem),
    radial-gradient(circle at bottom left, rgba(124, 58, 237, 0.2), transparent 32rem),
    linear-gradient(135deg, #f8fbff 0%, #eef2ff 48%, #ecfeff 100%);
  padding: 24px;
}

.auth-card {
  position: relative;
  z-index: 1;
  width: min(100%, 460px);
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
  top: 12%;
  right: 12%;
  width: 180px;
  height: 180px;
  background: rgba(6, 182, 212, 0.16);
}

.auth-orb--secondary {
  bottom: 10%;
  left: 10%;
  width: 220px;
  height: 220px;
  background: rgba(124, 58, 237, 0.18);
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
