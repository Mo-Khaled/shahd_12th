import { useEffect, useState } from 'react'
import Card from '../components/Card'
import Loader from '../components/Loader'
import { getMyInstructorCoursesApi } from '../api/coursesApi'
import { getMyEnrollmentsApi } from '../api/enrollmentsApi'
import { useAuth } from '../context/AuthContext'

export default function MyCoursesPage() {
  const { user } = useAuth()
  const [data, setData] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    const load = async () => {
      setLoading(true)
      setError('')

      try {
        const response = user?.role === 'Instructor' ? await getMyInstructorCoursesApi() : await getMyEnrollmentsApi()
        setData(response)
      } catch {
        setError('Failed to load your courses.')
      } finally {
        setLoading(false)
      }
    }

    load()
  }, [user])

  if (loading) {
    return <Loader text="Loading your courses..." />
  }

  return (
    <section>
      <div className="page-head">
        <h2>My Courses</h2>
      </div>
      {error && <div className="alert error">{error}</div>}
      <div className="grid-cards">
        {data.map((item) => (
          <Card key={item.id ?? item.enrollmentId}>
            <h3>{item.title ?? item.courseTitle}</h3>
            <p>{item.description ?? item.courseDescription}</p>
            {user?.role === 'Student' && <small>Enrolled at: {new Date(item.enrolledAt).toLocaleDateString()}</small>}
            {user?.role === 'Instructor' && <small>Instructor: {item.instructorName}</small>}
          </Card>
        ))}
      </div>
    </section>
  )
}
