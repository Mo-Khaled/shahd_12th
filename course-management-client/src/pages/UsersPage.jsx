import { useEffect, useState } from 'react'
import Card from '../components/Card'
import Loader from '../components/Loader'
import { deleteUserApi, getUsersApi, updateUserRoleApi } from '../api/usersApi'

const roles = ['Admin', 'Instructor', 'Student']

export default function UsersPage() {
  const [users, setUsers] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')

  const loadUsers = async () => {
    setLoading(true)
    try {
      const data = await getUsersApi()
      setUsers(data)
    } catch {
      setError('Failed to load users.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadUsers()
  }, [])

  const changeRole = async (userId, role) => {
    setError('')
    setMessage('')

    try {
      await updateUserRoleApi(userId, role)
      setMessage('Role updated.')
      await loadUsers()
    } catch (apiError) {
      setError(apiError.response?.data?.message || 'Role update failed.')
    }
  }

  const deleteUser = async (userId) => {
    if (!window.confirm('Delete this user?')) {
      return
    }

    setError('')
    setMessage('')

    try {
      await deleteUserApi(userId)
      setMessage('User deleted.')
      await loadUsers()
    } catch (apiError) {
      setError(apiError.response?.data?.message || 'Delete failed.')
    }
  }

  return (
    <section>
      <div className="page-head">
        <h2>Users Management</h2>
      </div>
      {message && <div className="alert success">{message}</div>}
      {error && <div className="alert error">{error}</div>}
      {loading ? (
        <Loader text="Loading users..." />
      ) : (
        <div className="table-wrap card">
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Role</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {users.map((user) => (
                <tr key={user.id}>
                  <td>{user.fullName}</td>
                  <td>{user.email}</td>
                  <td>
                    <select defaultValue={user.role} onChange={(event) => changeRole(user.id, event.target.value)}>
                      {roles.map((role) => (
                        <option key={role} value={role}>
                          {role}
                        </option>
                      ))}
                    </select>
                  </td>
                  <td>
                    <button type="button" className="danger-btn" onClick={() => deleteUser(user.id)}>
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
