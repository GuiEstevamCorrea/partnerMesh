import api from '../axios.config';
import { User, CreateUserRequest, UpdateUserRequest, PaginatedResponse, FilterParams, Permission } from '@/types';

interface UsersFilterParams extends FilterParams {
  permission?: Permission;
  vectorId?: string;
  isActive?: boolean;
}

export const usersApi = {
  list: async (params?: UsersFilterParams): Promise<PaginatedResponse<User>> => {
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

  activate: async (id: string): Promise<void> => {
    await api.patch(`/users/${id}/activate`);
  },

  deactivate: async (id: string): Promise<void> => {
    await api.patch(`/users/${id}/deactivate`);
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/users/${id}`);
  },
};
