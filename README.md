# Course Management System

A complete full-stack Course Management System built with:

- Backend: ASP.NET Core Web API (.NET 8)
- Frontend: React + Vite
- Database: SQLite + Entity Framework Core
- Authentication: JWT
- Architecture: Controllers → Services → Repositories → DbContext

## Features

- JWT authentication (register/login)
- Role-based authorization (Admin, Instructor, Student)
- Course CRUD with ownership rules
- Course enrollment workflow
- Admin dashboard with student/instructor/enrollment management
- User management for admins
- Swagger with Bearer token support
- Axios interceptor for JWT and 401 handling
- Protected frontend routes and role-based navigation
- Modern sidebar dashboard UI with dark teal color scheme

## Roles & Permissions

### Admin Dashboard

- Dashboard overview
- Manage Students (view, delete)
- Manage Instructors (view, delete)
- Manage Courses (CRUD)
- View all Enrollments
- Manage Users (change roles)

### Instructor

- View assigned courses
- Create/edit/delete own courses
- View students enrolled in own courses

### Student

- View all courses
- Enroll in courses
- View own enrolled courses

## Project Structure

### Backend

- `CourseManagement.Api/Controllers` - API endpoints
- `CourseManagement.Api/Services` - Business logic
- `CourseManagement.Api/Repositories` - Data access
- `CourseManagement.Api/DTOs` - Data transfer objects
- `CourseManagement.Api/Models` - Domain models
- `CourseManagement.Api/Data` - EF Core context

### Frontend

- `course-management-client/src/api` - Axios API clients
- `course-management-client/src/components` - Reusable components
- `course-management-client/src/pages` - Page components
- `course-management-client/src/context` - React context (auth)
- `course-management-client/src/layouts` - Layout components
- `course-management-client/src/routes` - Routing setup

## Default Admin User

- Email: admin@course.local
- Password: Admin@123

Seeded automatically on first API run.

## Run Backend

1. Navigate to API folder:

```bash
cd CourseManagement.Api
```

2. Run API:

```bash
dotnet run
```

3. Access Swagger:

- http://localhost:5053/swagger

## Run Frontend

1. Navigate to client folder:

```bash
cd course-management-client
```

2. Install packages (if needed):

```bash
npm install
```

3. Start dev server:

```bash
npm run dev
```

4. Open browser:

- http://localhost:5173

## Environment Notes

Frontend API URL defaults to:

- http://localhost:5053/api

To override, create `course-management-client/.env`:

```env
VITE_API_URL=http://localhost:5053/api
```

## API Endpoints

### Auth

- `POST /api/auth/register`
- `POST /api/auth/login`

### Courses

- `GET /api/courses` - Get all courses
- `GET /api/courses/{id}` - Get course by ID
- `GET /api/courses/my` - Get instructor's courses (Instructor only)
- `POST /api/courses` - Create course (Admin/Instructor)
- `PUT /api/courses/{id}` - Update course (Admin/Instructor)
- `DELETE /api/courses/{id}` - Delete course (Admin/Instructor)

### Enrollments

- `POST /api/enrollments` - Enroll in course (Student)
- `GET /api/enrollments/my-courses` - Get my enrollments
- `GET /api/enrollments/course/{courseId}` - Get course enrollments (Instructor/Admin)
- `GET /api/enrollments/all` - Get all enrollments (Admin)

### Users

- `GET /api/users` - Get all users (Admin)
- `GET /api/users/students` - Get all students (Admin)
- `GET /api/users/instructors` - Get all instructors (Admin)
- `PATCH /api/users/{id}/role` - Update user role (Admin)
- `DELETE /api/users/{id}` - Delete user (Admin)

## Authorization Rules

### Admin

- Full access to all features
- Can manage students, instructors, and courses
- Can assign instructors to courses
- Can view all enrollments

### Instructor

- Can create and manage own courses
- Can view students enrolled in own courses
- Cannot manage users
- Cannot create/manage other instructors' courses

### Student

- Can view all available courses
- Can enroll in courses
- Can view own enrolled courses only
- Cannot create or manage courses

## Frontend Pages (by role)

### Public

- `/login` - Login page
- `/register` - Registration page

### Protected (All Authenticated Users)

- `/courses` - Browse all courses

### Admin Only

- `/dashboard` - Admin dashboard overview
- `/students` - Manage students
- `/instructors` - Manage instructors
- `/enrollments` - View all enrollments
- `/users` - Manage all users (change roles)

### Instructor Only

- `/courses/new` - Create new course
- `/courses/:id/edit` - Edit course
- `/my-courses` - View assigned courses

### Student Only

- `/my-courses` - View enrolled courses

## Build Verification

- Backend: `dotnet build` ✓
- Frontend: `npm run build` ✓

## Technology Stack

- **Backend**
  - .NET 8
  - Entity Framework Core 8
  - JWT Bearer Authentication
  - SQLite
  - Swagger/OpenAPI

- **Frontend**
  - React 19
  - Vite 8
  - React Router DOM 7
  - Axios
  - Context API for state management

## Design

The UI features:

- Modern sidebar navigation (dark teal color scheme)
- Responsive layout with top bar for user profile
- Clean card-based design for course listings
- Table layout for admin data management
- Role-based UI elements (buttons/menu items hidden based on role)
- Professional spacing and typography
