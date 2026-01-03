import api from '../axios.config';
import { AuditLog, PaginatedResponse, FilterParams } from '@/types';

export const auditApi = {
  list: async (params?: FilterParams): Promise<PaginatedResponse<AuditLog>> => {
    const response = await api.get('/audit/logs', { params });
    return response.data;
  },

  getById: async (id: string): Promise<AuditLog> => {
    const response = await api.get(`/audit/logs/${id}`);
    return response.data;
  },
};
