import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { getCoursesApi, updateCourseApi } from '../api/coursesApi'
import { getInstructorsApi } from '../api/usersApi'
import FormField from '../components/FormField'

export default function AssignInstructorPage() {
  const navigate = useNavigate()
  const [courses, setCourses] = useState([])
  const [instructors, setInstructors] = useState([])
  const [formData, setFormData] = useState({
    courseId: '',
    instructorId: ''
  })
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [coursesData, instructorsData] = await Promise.all([
          getCoursesApi(),
          getInstructorsApi()
        ])
        setCourses(coursesData)
        setInstructors(instructorsData)
      } catch (err) {
        setError('Failed to load data')
      } finally {
        setLoading(false)
      }
    }

    fetchData()
  }, [])

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({ ...prev, [name]: value }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setSuccess('')

    if (!formData.courseId || !formData.instructorId) {
      setError('Please select both course and instructor')
      return
    }

    setSubmitting(true)

    try {
      const course = courses.find(c => c.id === parseInt(formData.courseId))
      await updateCourseApi(course.id, {
        title: course.title,
        description: course.description,
        instructorId: parseInt(formData.instructorId)
      })

      setSuccess('Instructor assigned successfully!')
      setFormData({ courseId: '', instructorId: '' })

      setTimeout(() => navigate('/courses'), 2000)
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to assign instructor')
    } finally {
      setSubmitting(false)
    }
  }

  if (loading) return <div className="page-wrap"><p>Loading...</p></div>

  return (
    <div className="page-wrap">
      <h1>Assign Instructor to Course</h1>

      <form onSubmit={handleSubmit} className="form">
        <div className="form-field">
          <label htmlFor="courseId">Course</label>
          <select
            id="courseId"
            name="courseId"
            value={formData.courseId}
            onChange={handleChange}
            required
            className="form-field"
          >
            <option value="">Select a course...</option>
            {courses.map(course => (
              <option key={course.id} value={course.id}>
                {course.title} (Instructor: {course.instructorName})
              </option>
            ))}
          </select>
        </div>

        <div className="form-field">
          <label htmlFor="instructorId">Instructor</label>
          <select
            id="instructorId"
            name="instructorId"
            value={formData.instructorId}
            onChange={handleChange}
            required
            className="form-field"
          >
            <option value="">Select an instructor...</option>
            {instructors.map(instructor => (
              <option key={instructor.id} value={instructor.id}>
                {instructor.fullName} ({instructor.email})
              </option>
            ))}
          </select>
        </div>

        {error && <div className="error-message">{error}</div>}
        {success && <div className="success-message">{success}</div>}

        <button type="submit" className="btn btn-primary" disabled={submitting || !formData.courseId || !formData.instructorId}>
          {submitting ? 'Assigning...' : 'Assign Instructor'}
        </button>
      </form>
    </div>
  )
}
