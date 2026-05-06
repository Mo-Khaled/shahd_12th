import { Link, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function MainLayout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  const getInitials = (name) => {
    const parts = name.split(' ')
    return (parts[0][0] + (parts[1]?.[0] || '')).toUpperCase()
  }

  const navItems =
    user?.role === 'Admin'
      ? [
          { label: 'Dashboard', icon: 'D', path: '/dashboard' },
          { label: 'Students', icon: 'S', path: '/students' },
          { label: 'Instructors', icon: 'I', path: '/instructors' },
          { label: 'Courses', icon: 'C', path: '/courses' },
          { label: 'Enrollments', icon: 'E', path: '/enrollments' },
          { label: 'Create User', icon: '+', path: '/users/create' },
          { label: 'Assign Instructor', icon: 'A', path: '/courses/assign-instructor' },
          { label: 'Enroll Student', icon: 'N', path: '/enrollments/manual-enroll' },
        ]
      : user?.role === 'Instructor'
        ? [
            { label: 'Courses', icon: 'C', path: '/courses' },
            { label: 'My Courses', icon: 'M', path: '/my-courses' },
            { label: 'Enroll Student', icon: 'N', path: '/enrollments/manual-enroll' },
          ]
        : [
            { label: 'Courses', icon: 'C', path: '/courses' },
            { label: 'My Courses', icon: 'M', path: '/my-courses' },
          ]

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="sidebar-brand">
          <div className="brand-icon">📚</div>
          <span>Course Hub</span>
        </div>
        <nav className="sidebar-nav">
          {navItems.map((item) => (
            <Link key={item.path} to={item.path} className="nav-item">
              <span className="nav-icon">{item.icon}</span>
              <span className="nav-label">{item.label}</span>
            </Link>
          ))}
        </nav>
      </aside>
      <div className="main-section">
        <header className="top-bar">
          <div className="top-bar-spacer"></div>
          <div className="user-profile">
            <div className="avatar">{getInitials(user?.fullName)}</div>
            <div className="user-info">
              <div className="user-role">{user?.role}</div>
              <div className="user-name">Welcome back</div>
            </div>
            <button type="button" className="logout-btn" onClick={handleLogout}>
              Logout
            </button>
          </div>
        </header>
        <main className="page-wrap">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
