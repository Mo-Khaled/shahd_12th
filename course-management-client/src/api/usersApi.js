import axiosClient from './axiosClient'

export const getUsersApi = async () => {
  const { data } = await axiosClient.get('/users')
  return data
}

export const getStudentsApi = async () => {
  const { data } = await axiosClient.get('/users/students')
  return data
}

export const getInstructorsApi = async () => {
  const { data } = await axiosClient.get('/users/instructors')
  return data
}

export const createUserApi = async (payload) => {
  const { data } = await axiosClient.post('/users/create', payload)
  return data
}

export const updateUserRoleApi = async (id, role) => {
  const { data } = await axiosClient.patch(`/users/${id}/role`, { role })
  return data
}

export const deleteUserApi = async (id) => {
  await axiosClient.delete(`/users/${id}`)
}
