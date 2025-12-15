import api from '../axios.config';
import { 
  Business, 
  CreateBusinessRequest, 
  UpdateBusinessRequest,
  Payment,
  PaginatedResponse, 
  FilterParams 
} from '@/types';

export const businessApi = {
  list: async (params?: FilterParams): Promise<PaginatedResponse<Business>> => {
    const response = await api.get('/business', { params });
    return response.data;
  },

  getById: async (id: string): Promise<Business> => {
    const response = await api.get(`/business/${id}`);
    return response.data;
  },

  getPayments: async (id: string): Promise<Payment[]> => {
    const response = await api.get(`/business/${id}/payments`);
    return response.data;
  },

  create: async (data: CreateBusinessRequest): Promise<Business> => {
    const response = await api.post('/business', data);
    return response.data;
  },

  update: async (id: string, data: UpdateBusinessRequest): Promise<Business> => {
    const response = await api.put(`/business/${id}`, data);
    return response.data;
  },

  cancel: async (id: string): Promise<void> => {
    await api.patch(`/business/${id}/cancel`);
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/business/${id}`);
  },
};
