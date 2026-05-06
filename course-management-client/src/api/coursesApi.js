import axiosClient from './axiosClient'

export const getCoursesApi = async () => {
  const { data } = await axiosClient.get('/courses')
  return data
}

export const getCourseByIdApi = async (id) => {
  const { data } = await axiosClient.get(`/courses/${id}`)
  return data
}

export const getMyInstructorCoursesApi = async () => {
  const { data } = await axiosClient.get('/courses/my')
  return data
}

export const createCourseApi = async (payload) => {
  const { data } = await axiosClient.post('/courses', payload)
  return data
}

export const updateCourseApi = async (id, payload) => {
  const { data } = await axiosClient.put(`/courses/${id}`, payload)
  return data
}

export const deleteCourseApi = async (id) => {
  await axiosClient.delete(`/courses/${id}`)
}
