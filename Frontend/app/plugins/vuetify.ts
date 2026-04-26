import { createVuetify } from 'vuetify'
import { aliases, mdi } from 'vuetify/iconsets/mdi'

export default defineNuxtPlugin((nuxtApp) => {
  const vuetify = createVuetify({
    icons: {
      defaultSet: 'mdi',
      aliases,
      sets: {
        mdi
      }
    },
    theme: {
      defaultTheme: 'light',
      themes: {
        light: {
          dark: false,
          colors: {
            primary: '#7657f5',
            secondary: '#06b6d4',
            background: '#f5f7fb',
            surface: '#ffffff',
            error: '#ef4444',
            info: '#2563eb',
            success: '#16a34a',
            warning: '#f59e0b'
          }
        }
      }
    }
  })

  nuxtApp.vueApp.use(vuetify)
})
