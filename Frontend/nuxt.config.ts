import vuetify from 'vite-plugin-vuetify'

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  ssr: false,
  app: {
    head: {
      title: 'Years in pixels'
    }
  },
  devtools: { enabled: true },
  modules: [
    '@pinia/nuxt',
    '@nuxt/eslint',
    '@nuxt/icon',
    '@nuxtjs/color-mode',
    '@nuxtjs/i18n'
  ],
  css: [
    'vuetify/styles',
    '@mdi/font/css/materialdesignicons.css'
  ],
  build: {
    transpile: ['vuetify']
  },
  runtimeConfig: {
    public: {
      apiBaseUrl: 'http://localhost:5277'
    }
  },
  i18n: {
    defaultLocale: 'pl',
    langDir: 'locales',
    locales: [
      {
        code: 'pl',
        file: 'pl.json',
        language: 'pl-PL',
        name: 'Polski'
      },
      {
        code: 'en',
        file: 'en.json',
        language: 'en-US',
        name: 'English'
      }
    ]
  },
  vite: {
    plugins: [
      vuetify({ autoImport: true })
    ]
  }
})