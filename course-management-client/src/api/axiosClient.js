import axios from 'axios'

const axiosClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5053/api',
})

axiosClient.interceptors.request.use((config) => {
  const authRaw = localStorage.getItem('cms_auth')
  if (authRaw) {
    const auth = JSON.parse(authRaw)
    if (auth?.token) {
      config.headers.Authorization = `Bearer ${auth.token}`
    }
  }

  return config
})

axiosClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('cms_auth')
      if (window.location.pathname !== '/login') {
        window.location.href = '/login'
      }
    }

    return Promise.reject(error)
  }
)

export default axiosClient
