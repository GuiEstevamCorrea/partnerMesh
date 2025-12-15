import api from '../axios.config';
import { AuditLog, PaginatedResponse, FilterParams } from '@/types';

export const auditApi = {
  list: async (params?: FilterParams): Promise<PaginatedResponse<AuditLog>> => {
    const response = await api.get('/audit', { params });
    return response.data;
  },

  getById: async (id: string): Promise<AuditLog> => {
    const response = await api.get(`/audit/${id}`);
    return response.data;
  },
};
