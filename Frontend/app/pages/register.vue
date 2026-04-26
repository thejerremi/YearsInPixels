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

<style scoped src="~/assets/styles/register-page.css"></style>
