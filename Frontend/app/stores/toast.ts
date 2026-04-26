import { defineStore } from 'pinia'

type ToastType = 'error' | 'success' | 'info'

export const useToastStore = defineStore('toast', () => {
  const isVisible = ref(false)
  const message = ref('')
  const type = ref<ToastType>('info')

  const show = (nextMessage: string, nextType: ToastType = 'info') => {
    const normalizedMessage = nextMessage.trim()

    if (!normalizedMessage) {
      return
    }

    message.value = normalizedMessage
    type.value = nextType
    isVisible.value = true
  }

  const showError = (nextMessage: string) => show(nextMessage, 'error')

  return {
    isVisible,
    message,
    type,
    show,
    showError
  }
})
