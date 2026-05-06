import { useAuth } from '../context/AuthContext'

export default function DashboardPage() {
  const { user } = useAuth()

  return (
    <section>
      <h1>Dashboard</h1>
      <p>Modern academic operations dashboard</p>

      <div className="dashboard-grid">
        <div className="stat-card">
          <div className="stat-value">Quick actions</div>
          <p>Use the navigation to manage students, instructors, courses, and enrollments.</p>
        </div>
      </div>

      <div className="info-box">
        <h3>Welcome, {user?.fullName}!</h3>
        <p>You are logged in as an {user?.role.toLowerCase()}. Use the sidebar to navigate through the portal.</p>
      </div>
    </section>
  )
}
