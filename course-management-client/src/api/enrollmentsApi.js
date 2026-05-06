import axiosClient from './axiosClient'

export const enrollInCourseApi = async (courseId) => {
  const { data } = await axiosClient.post('/enrollments', { courseId })
  return data
}

export const adminEnrollApi = async (payload) => {
  const { data } = await axiosClient.post('/enrollments/admin', payload)
  return data
}

export const getMyEnrollmentsApi = async () => {
  const { data } = await axiosClient.get('/enrollments/my-courses')
  return data
}

export const getEnrollmentsByCourseApi = async (courseId) => {
  const { data } = await axiosClient.get(`/enrollments/course/${courseId}`)
  return data
}

export const getAllEnrollmentsApi = async () => {
  const { data } = await axiosClient.get('/enrollments/all')
  return data
}
