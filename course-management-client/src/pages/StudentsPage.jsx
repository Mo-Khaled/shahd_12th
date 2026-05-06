import { useEffect, useState } from 'react'
import { deleteUserApi, getStudentsApi } from '../api/usersApi'
import Card from '../components/Card'
import Loader from '../components/Loader'

export default function StudentsPage() {
  const [students, setStudents] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')

  const loadStudents = async () => {
    setLoading(true)
    try {
      const data = await getStudentsApi()
      setStudents(data)
    } catch {
      setError('Failed to load students.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadStudents()
  }, [])

  const deleteStudent = async (studentId) => {
    if (!window.confirm('Delete this student?')) {
      return
    }

    setError('')
    setMessage('')

    try {
      await deleteUserApi(studentId)
      setMessage('Student deleted.')
      await loadStudents()
    } catch (apiError) {
      setError(apiError.response?.data?.message || 'Delete failed.')
    }
  }

  return (
    <section>
      <h2>Students</h2>
      {message && <div className="alert success">{message}</div>}
      {error && <div className="alert error">{error}</div>}
      {loading ? (
        <Loader text="Loading students..." />
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
              {students.map((student) => (
                <tr key={student.id}>
                  <td>{student.fullName}</td>
                  <td>{student.email}</td>
                  <td>
                    <button type="button" className="danger-btn" onClick={() => deleteStudent(student.id)}>
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
