import { Link } from 'react-router-dom'

export default function NotFoundPage() {
  return (
    <div className="auth-wrap">
      <div className="card auth-card">
        <h2>Page Not Found</h2>
        <p>The page you requested does not exist.</p>
        <Link to="/courses" className="primary-btn">
          Back to Courses
        </Link>
      </div>
    </div>
  )
}
