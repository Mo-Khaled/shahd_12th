import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import Card from '../components/Card'
import FormField from '../components/FormField'
import { registerApi } from '../api/authApi'
import { useAuth } from '../context/AuthContext'

const roles = ['Student', 'Instructor']

export default function RegisterPage() {
  const navigate = useNavigate()
  const { login } = useAuth()
  const [form, setForm] = useState({ fullName: '', email: '', password: '', role: 'Student' })
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
      const response = await registerApi(form)
      login(response)
      navigate('/courses')
    } catch (apiError) {
      setError(apiError.response?.data?.message || 'Registration failed.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="auth-wrap">
      <Card className="auth-card">
        <h1>Create Account</h1>
        <p>Join as a student or instructor</p>
        {error && <div className="alert error">{error}</div>}
        <form onSubmit={onSubmit} className="form-grid">
          <FormField label="Full Name" name="fullName" value={form.fullName} onChange={onChange} required />
          <FormField label="Email" name="email" type="email" value={form.email} onChange={onChange} required />
          <FormField label="Password" name="password" type="password" value={form.password} onChange={onChange} required />
          <label className="form-field">
            <span>Role</span>
            <select name="role" value={form.role} onChange={onChange}>
              {roles.map((role) => (
                <option key={role} value={role}>
                  {role}
                </option>
              ))}
            </select>
          </label>
          <button type="submit" className="primary-btn" disabled={loading}>
            {loading ? 'Creating account...' : 'Register'}
          </button>
        </form>
        <div className="muted-text">
          Already have an account? <Link to="/login">Sign in</Link>
        </div>
      </Card>
    </div>
  )
}
