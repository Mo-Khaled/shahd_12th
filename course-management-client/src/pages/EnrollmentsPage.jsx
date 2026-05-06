import { useEffect, useState } from 'react'
import { getAllEnrollmentsApi } from '../api/enrollmentsApi'
import Card from '../components/Card'
import Loader from '../components/Loader'

export default function EnrollmentsPage() {
  const [enrollments, setEnrollments] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    const load = async () => {
      setLoading(true)
      try {
        const data = await getAllEnrollmentsApi()
        setEnrollments(data)
      } catch {
        setError('Failed to load enrollments.')
      } finally {
        setLoading(false)
      }
    }

    load()
  }, [])

  return (
    <section>
      <h2>Enrollments</h2>
      {error && <div className="alert error">{error}</div>}
      {loading ? (
        <Loader text="Loading enrollments..." />
      ) : (
        <div className="table-wrap card">
          <table>
            <thead>
              <tr>
                <th>Student</th>
                <th>Course</th>
                <th>Enrolled At</th>
              </tr>
            </thead>
            <tbody>
              {enrollments.map((enrollment) => (
                <tr key={enrollment.enrollmentId}>
                  <td>{enrollment.studentName}</td>
                  <td>{enrollment.courseTitle}</td>
                  <td>{new Date(enrollment.enrolledAt).toLocaleDateString()}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}
