import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import Card from '../components/Card'
import Loader from '../components/Loader'
import { deleteCourseApi, getCoursesApi } from '../api/coursesApi'
import { enrollInCourseApi } from '../api/enrollmentsApi'
import { useAuth } from '../context/AuthContext'

export default function CoursesPage() {
  const { user } = useAuth()
  const [courses, setCourses] = useState([])
  const [loading, setLoading] = useState(true)
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')

  const loadCourses = async () => {
    setLoading(true)
    try {
      const data = await getCoursesApi()
      setCourses(data)
    } catch {
      setError('Failed to load courses.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadCourses()
  }, [])

  const handleEnroll = async (courseId) => {
    setMessage('')
    setError('')
    try {
      const response = await enrollInCourseApi(courseId)
      setMessage(response.message || 'Enrolled successfully.')
    } catch (apiError) {
      setError(apiError.response?.data?.message || 'Could not enroll.')
    }
  }

  const handleDelete = async (courseId) => {
    if (!window.confirm('Delete this course?')) {
      return
    }

    try {
      await deleteCourseApi(courseId)
      setMessage('Course deleted.')
      await loadCourses()
    } catch (apiError) {
      setError(apiError.response?.data?.message || 'Could not delete course.')
    }
  }

  return (
    <section>
      <div className="page-head">
        <h2>Courses</h2>
        {(user?.role === 'Admin' || user?.role === 'Instructor') && (
          <Link to="/courses/new" className="primary-btn">
            Create Course
          </Link>
        )}
      </div>

      {message && <div className="alert success">{message}</div>}
      {error && <div className="alert error">{error}</div>}

      {loading ? (
        <Loader text="Loading courses..." />
      ) : (
        <div className="grid-cards">
          {courses.map((course) => {
            const canManage = user?.role === 'Admin' || (user?.role === 'Instructor' && course.instructorId === user.userId)

            return (
              <Card key={course.id}>
                <h3>{course.title}</h3>
                <p>{course.description || 'No description yet.'}</p>
                <small>Instructor: {course.instructorName}</small>
                <div className="card-actions">
                  {user?.role === 'Student' && (
                    <button type="button" className="primary-btn" onClick={() => handleEnroll(course.id)}>
                      Enroll
                    </button>
                  )}
                  {canManage && (
                    <>
                      <Link to={`/courses/${course.id}/edit`} className="ghost-btn">
                        Edit
                      </Link>
                      <button type="button" className="danger-btn" onClick={() => handleDelete(course.id)}>
                        Delete
                      </button>
                    </>
                  )}
                </div>
              </Card>
            )
          })}
        </div>
      )}
    </section>
  )
}
