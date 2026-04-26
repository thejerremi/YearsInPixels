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

<style scoped src="~/assets/styles/login-page.css"></style>