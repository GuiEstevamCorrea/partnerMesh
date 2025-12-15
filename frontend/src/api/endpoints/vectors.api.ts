import api from '../axios.config';
import { Vector, CreateVectorRequest, UpdateVectorRequest, PaginatedResponse, FilterParams } from '@/types';

export const vectorsApi = {
  list: async (params?: FilterParams): Promise<PaginatedResponse<Vector>> => {
    const response = await api.get('/vectors', { params });
    return response.data;
  },

  getById: async (id: string): Promise<Vector> => {
    const response = await api.get(`/vectors/${id}`);
    return response.data;
  },

  create: async (data: CreateVectorRequest): Promise<Vector> => {
    const response = await api.post('/vectors', data);
    return response.data;
  },

  update: async (id: string, data: UpdateVectorRequest): Promise<Vector> => {
    const response = await api.put(`/vectors/${id}`, data);
    return response.data;
  },

  toggleActive: async (id: string): Promise<void> => {
    await api.patch(`/vectors/${id}/toggle-active`);
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/vectors/${id}`);
  },
};
