import { createContext, useContext, useMemo, useState } from 'react'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [auth, setAuth] = useState(() => {
    const raw = localStorage.getItem('cms_auth')
    return raw ? JSON.parse(raw) : null
  })

  const login = (authResponse) => {
    const payload = {
      token: authResponse.token,
      user: {
        userId: authResponse.userId,
        fullName: authResponse.fullName,
        email: authResponse.email,
        role: authResponse.role,
      },
    }

    localStorage.setItem('cms_auth', JSON.stringify(payload))
    setAuth(payload)
  }

  const logout = () => {
    localStorage.removeItem('cms_auth')
    setAuth(null)
  }

  const value = useMemo(
    () => ({
      auth,
      user: auth?.user ?? null,
      token: auth?.token ?? null,
      isAuthenticated: Boolean(auth?.token),
      login,
      logout,
    }),
    [auth]
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used inside AuthProvider')
  }

  return context
}
