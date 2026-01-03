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
    console.log('usersApi.getById - Resposta completa:', response.data);
    
    // A API retorna: { isSuccess, message, user: {...} }
    if (response.data?.user) {
      const apiUser = response.data.user;
      
      // Adaptar dados da API para o tipo User esperado pelo frontend
      const adaptedUser: User = {
        id: apiUser.id,
        name: apiUser.name,
        email: apiUser.email,
        permission: apiUser.permission as Permission,
        vectorId: apiUser.vetores?.[0]?.vetorId || undefined,
        vectorName: apiUser.vetores?.[0]?.vetorName || undefined,
        isActive: apiUser.active,
        createdAt: apiUser.createdAt,
      };
      
      console.log('usersApi.getById - Usuário adaptado:', adaptedUser);
      return adaptedUser;
    }
    
    throw new Error(response.data?.message || 'Erro ao carregar usuário');
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
