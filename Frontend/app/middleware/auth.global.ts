const publicRoutes = ['/login', '/register']

export default defineNuxtRouteMiddleware(async (to) => {
  const authStore = useAuthStore()
  const localePath = useLocalePath()

  authStore.loadSessionFromCookies()

  const isPublicRoute = publicRoutes.some(route => to.path === route || to.path.endsWith(route))

  if (!authStore.isAuthenticated && isPublicRoute) {
    return
  }

  if (!authStore.isAuthenticated) {
    return navigateTo(localePath('/login'))
  }

  const isSessionValid = await authStore.checkAuthenticated()

  if (!isSessionValid && !isPublicRoute) {
    return navigateTo(localePath('/login'))
  }

  if (isSessionValid && isPublicRoute) {
    return navigateTo(localePath('/'))
  }
})
