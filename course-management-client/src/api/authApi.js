import axiosClient from './axiosClient'

export const loginApi = async (payload) => {
  const { data } = await axiosClient.post('/auth/login', payload)
  return data
}

export const registerApi = async (payload) => {
  const { data } = await axiosClient.post('/auth/register', payload)
  return data
}
