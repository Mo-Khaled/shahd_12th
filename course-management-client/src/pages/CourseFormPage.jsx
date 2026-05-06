import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import Card from '../components/Card'
import FormField from '../components/FormField'
import Loader from '../components/Loader'
import { createCourseApi, getCourseByIdApi, updateCourseApi } from '../api/coursesApi'
import { getUsersApi } from '../api/usersApi'
import { useAuth } from '../context/AuthContext'

export default function CourseFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { user } = useAuth()
  const isEdit = Boolean(id)

  const [form, setForm] = useState({ title: '', description: '', instructorId: 0 })
  const [instructors, setInstructors] = useState([])
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    const load = async () => {
      try {
        if (user?.role === 'Admin') {
          const users = await getUsersApi()
          const list = users.filter((u) => u.role === 'Instructor')
          setInstructors(list)
          if (list.length > 0) {
            setForm((prev) => ({ ...prev, instructorId: list[0].id }))
          }
        } else {
          setForm((prev) => ({ ...prev, instructorId: user.userId }))
        }

        if (isEdit) {
          const course = await getCourseByIdApi(id)
          setForm({
            title: course.title,
            description: course.description,
            instructorId: course.instructorId,
          })
        }
      } catch (apiError) {
        setError(apiError.response?.data?.message || 'Failed to load form data.')
      } finally {
        setLoading(false)
      }
    }

    load()
  }, [id, isEdit, user])

  const onChange = (event) => {
    setForm((prev) => ({ ...prev, [event.target.name]: event.target.value }))
  }

  const onSubmit = async (event) => {
    event.preventDefault()
    setSaving(true)
    setError('')

    const payload = {
      title: form.title,
      description: form.description,
      instructorId: Number(form.instructorId),
    }

    try {
      if (isEdit) {
        await updateCourseApi(id, payload)
      } else {
        await createCourseApi(payload)
      }
      navigate('/courses')
    } catch (apiError) {
      setError(apiError.response?.data?.message || 'Failed to save course.')
    } finally {
      setSaving(false)
    }
  }

  if (loading) {
    return <Loader text="Loading course form..." />
  }

  return (
    <Card className="form-card">
      <h2>{isEdit ? 'Edit Course' : 'Create Course'}</h2>
      {error && <div className="alert error">{error}</div>}
      <form onSubmit={onSubmit} className="form-grid">
        <FormField label="Title" name="title" value={form.title} onChange={onChange} required />
        <label className="form-field">
          <span>Description</span>
          <textarea name="description" value={form.description} onChange={onChange} rows={4} />
        </label>

        {user?.role === 'Admin' && (
          <label className="form-field">
            <span>Instructor</span>
            <select name="instructorId" value={form.instructorId} onChange={onChange} required>
              {instructors.map((instructor) => (
                <option key={instructor.id} value={instructor.id}>
                  {instructor.fullName}
                </option>
              ))}
            </select>
          </label>
        )}

        <button type="submit" className="primary-btn" disabled={saving}>
          {saving ? 'Saving...' : 'Save'}
        </button>
      </form>
    </Card>
  )
}
