import api from '../axios.config';
import { 
  BusinessType, 
  CreateBusinessTypeRequest, 
  UpdateBusinessTypeRequest,
  PaginatedResponse, 
  FilterParams 
} from '@/types';

export const businessTypesApi = {
  list: async (params?: FilterParams): Promise<PaginatedResponse<BusinessType>> => {
    const response = await api.get('/business-types', { params });
    return response.data;
  },

  getById: async (id: string): Promise<BusinessType> => {
    const response = await api.get(`/business-types/${id}`);
    return response.data;
  },

  create: async (data: CreateBusinessTypeRequest): Promise<BusinessType> => {
    const response = await api.post('/business-types', data);
    return response.data;
  },

  update: async (id: string, data: UpdateBusinessTypeRequest): Promise<BusinessType> => {
    const response = await api.put(`/business-types/${id}`, data);
    return response.data;
  },

  toggleActive: async (id: string): Promise<void> => {
    await api.patch(`/business-types/${id}/toggle-active`);
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/business-types/${id}`);
  },
};
