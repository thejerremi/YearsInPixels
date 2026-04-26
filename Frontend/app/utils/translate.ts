type TranslationParams = Record<string, string | number>

type I18nInstance = {
  t: (key: string, params?: TranslationParams) => string
}

export const translate = (key: string, params?: TranslationParams) => {
  const { $i18n } = useNuxtApp()

  return ($i18n as I18nInstance).t(key, params)
}
