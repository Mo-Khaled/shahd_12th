import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import Card from '../components/Card'
import FormField from '../components/FormField'
import { loginApi } from '../api/authApi'
import { useAuth } from '../context/AuthContext'

export default function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ email: '', password: '' })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const onChange = (event) => {
    setForm((prev) => ({ ...prev, [event.target.name]: event.target.value }))
  }

  const onSubmit = async (event) => {
    event.preventDefault()
    setLoading(true)
    setError('')

    try {
      const response = await loginApi(form)
      login(response)
      navigate('/courses')
    } catch (apiError) {
      setError(apiError.response?.data?.message || 'Login failed.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="auth-wrap">
      <Card className="auth-card">
        <h1>Sign In</h1>
        <p>Access your course portal account</p>
        {error && <div className="alert error">{error}</div>}
        <form onSubmit={onSubmit} className="form-grid">
          <FormField label="Email" name="email" type="email" value={form.email} onChange={onChange} required />
          <FormField label="Password" name="password" type="password" value={form.password} onChange={onChange} required />
          <button type="submit" className="primary-btn" disabled={loading}>
            {loading ? 'Signing in...' : 'Login'}
          </button>
        </form>
        <div className="muted-text">
          New here? <Link to="/register">Create account</Link>
        </div>
      </Card>
    </div>
  )
}
