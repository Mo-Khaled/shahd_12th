import { Navigate, Route, Routes } from 'react-router-dom'
import MainLayout from '../layouts/MainLayout'
import ProtectedRoute from './ProtectedRoute'
import LoginPage from '../pages/LoginPage'
import RegisterPage from '../pages/RegisterPage'
import DashboardPage from '../pages/DashboardPage'
import CoursesPage from '../pages/CoursesPage'
import CourseFormPage from '../pages/CourseFormPage'
import MyCoursesPage from '../pages/MyCoursesPage'
import StudentsPage from '../pages/StudentsPage'
import InstructorsPage from '../pages/InstructorsPage'
import EnrollmentsPage from '../pages/EnrollmentsPage'
import UsersPage from '../pages/UsersPage'
import CreateUserPage from '../pages/CreateUserPage'
import AssignInstructorPage from '../pages/AssignInstructorPage'
import ManualEnrollPage from '../pages/ManualEnrollPage'
import NotFoundPage from '../pages/NotFoundPage'

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/courses" replace />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      <Route
        element={
          <ProtectedRoute>
            <MainLayout />
          </ProtectedRoute>
        }
      >
        <Route path="/dashboard" element={<ProtectedRoute allowedRoles={['Admin']}><DashboardPage /></ProtectedRoute>} />
        <Route path="/courses" element={<CoursesPage />} />
        <Route
          path="/courses/new"
          element={
            <ProtectedRoute allowedRoles={['Admin', 'Instructor']}>
              <CourseFormPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/courses/:id/edit"
          element={
            <ProtectedRoute allowedRoles={['Admin', 'Instructor']}>
              <CourseFormPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/my-courses"
          element={
            <ProtectedRoute allowedRoles={['Student', 'Instructor']}>
              <MyCoursesPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/students"
          element={
            <ProtectedRoute allowedRoles={['Admin']}>
              <StudentsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/instructors"
          element={
            <ProtectedRoute allowedRoles={['Admin']}>
              <InstructorsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enrollments"
          element={
            <ProtectedRoute allowedRoles={['Admin']}>
              <EnrollmentsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/users"
          element={
            <ProtectedRoute allowedRoles={['Admin']}>
              <UsersPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/users/create"
          element={
            <ProtectedRoute allowedRoles={['Admin']}>
              <CreateUserPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/courses/assign-instructor"
          element={
            <ProtectedRoute allowedRoles={['Admin']}>
              <AssignInstructorPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/enrollments/manual-enroll"
          element={
            <ProtectedRoute allowedRoles={['Admin', 'Instructor']}>
              <ManualEnrollPage />
            </ProtectedRoute>
          }
        />
      </Route>

      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  )
}
