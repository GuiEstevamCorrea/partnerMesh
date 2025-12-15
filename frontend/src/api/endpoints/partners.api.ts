import api from '../axios.config';
import { 
  Partner, 
  CreatePartnerRequest, 
  UpdatePartnerRequest, 
  PartnerTree,
  PaginatedResponse, 
  FilterParams 
} from '@/types';

export const partnersApi = {
  list: async (params?: FilterParams): Promise<PaginatedResponse<Partner>> => {
    const response = await api.get('/partners', { params });
    return response.data;
  },

  getById: async (id: string): Promise<Partner> => {
    const response = await api.get(`/partners/${id}`);
    return response.data;
  },

  getTree: async (id: string): Promise<PartnerTree> => {
    const response = await api.get(`/partners/${id}/tree`);
    return response.data;
  },

  create: async (data: CreatePartnerRequest): Promise<Partner> => {
    const response = await api.post('/partners', data);
    return response.data;
  },

  update: async (id: string, data: UpdatePartnerRequest): Promise<Partner> => {
    const response = await api.put(`/partners/${id}`, data);
    return response.data;
  },

  toggleActive: async (id: string): Promise<void> => {
    await api.patch(`/partners/${id}/toggle-active`);
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/partners/${id}`);
  },
};
