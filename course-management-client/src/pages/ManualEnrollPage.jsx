import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { getCoursesApi } from '../api/coursesApi'
import { getStudentsApi } from '../api/usersApi'
import { adminEnrollApi } from '../api/enrollmentsApi'
import FormField from '../components/FormField'

export default function ManualEnrollPage() {
  const navigate = useNavigate()
  const [courses, setCourses] = useState([])
  const [students, setStudents] = useState([])
  const [formData, setFormData] = useState({
    studentId: '',
    courseId: ''
  })
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [coursesData, studentsData] = await Promise.all([
          getCoursesApi(),
          getStudentsApi()
        ])
        setCourses(coursesData)
        setStudents(studentsData)
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

    if (!formData.studentId || !formData.courseId) {
      setError('Please select both student and course')
      return
    }

    setSubmitting(true)

    try {
      await adminEnrollApi({
        studentId: parseInt(formData.studentId),
        courseId: parseInt(formData.courseId)
      })

      setSuccess('Student enrolled successfully!')
      setFormData({ studentId: '', courseId: '' })

      setTimeout(() => navigate('/enrollments'), 2000)
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to enroll student')
    } finally {
      setSubmitting(false)
    }
  }

  if (loading) return <div className="page-wrap"><p>Loading...</p></div>

  return (
    <div className="page-wrap">
      <h1>Enroll Student in Course</h1>

      <form onSubmit={handleSubmit} className="form">
        <div className="form-field">
          <label htmlFor="studentId">Student</label>
          <select
            id="studentId"
            name="studentId"
            value={formData.studentId}
            onChange={handleChange}
            required
            className="form-field"
          >
            <option value="">Select a student...</option>
            {students.map(student => (
              <option key={student.id} value={student.id}>
                {student.fullName} ({student.email})
              </option>
            ))}
          </select>
        </div>

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

        {error && <div className="error-message">{error}</div>}
        {success && <div className="success-message">{success}</div>}

        <button type="submit" className="btn btn-primary" disabled={submitting || !formData.studentId || !formData.courseId}>
          {submitting ? 'Enrolling...' : 'Enroll Student'}
        </button>
      </form>
    </div>
  )
}
