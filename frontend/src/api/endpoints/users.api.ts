import api from '../axios.config';
import { User, CreateUserRequest, UpdateUserRequest, PaginatedResponse, FilterParams } from '@/types';

export const usersApi = {
  list: async (params?: FilterParams): Promise<PaginatedResponse<User>> => {
    const response = await api.get('/users', { params });
    return response.data;
  },

  getById: async (id: string): Promise<User> => {
    const response = await api.get(`/users/${id}`);
    return response.data;
  },

  create: async (data: CreateUserRequest): Promise<User> => {
    const response = await api.post('/users', data);
    return response.data;
  },

  update: async (id: string, data: UpdateUserRequest): Promise<User> => {
    const response = await api.put(`/users/${id}`, data);
    return response.data;
  },

  toggleActive: async (id: string): Promise<void> => {
    await api.patch(`/users/${id}/toggle-active`);
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/users/${id}`);
  },
};
