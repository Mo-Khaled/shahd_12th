import { useEffect, useState } from 'react'
import { deleteUserApi, getInstructorsApi } from '../api/usersApi'
import Card from '../components/Card'
import Loader from '../components/Loader'

export default function InstructorsPage() {
  const [instructors, setInstructors] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')

  const loadInstructors = async () => {
    setLoading(true)
    try {
      const data = await getInstructorsApi()
      setInstructors(data)
    } catch {
      setError('Failed to load instructors.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadInstructors()
  }, [])

  const deleteInstructor = async (instructorId) => {
    if (!window.confirm('Delete this instructor?')) {
      return
    }

    setError('')
    setMessage('')

    try {
      await deleteUserApi(instructorId)
      setMessage('Instructor deleted.')
      await loadInstructors()
    } catch (apiError) {
      setError(apiError.response?.data?.message || 'Delete failed.')
    }
  }

  return (
    <section>
      <h2>Instructors</h2>
      {message && <div className="alert success">{message}</div>}
      {error && <div className="alert error">{error}</div>}
      {loading ? (
        <Loader text="Loading instructors..." />
      ) : (
        <div className="table-wrap card">
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {instructors.map((instructor) => (
                <tr key={instructor.id}>
                  <td>{instructor.fullName}</td>
                  <td>{instructor.email}</td>
                  <td>
                    <button type="button" className="danger-btn" onClick={() => deleteInstructor(instructor.id)}>
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}
