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
    // Backend retorna { isSuccess, message, businesses, pagination, summary }
    if (response.data.businesses && response.data.pagination) {
      return {
        items: response.data.businesses,
        totalItems: response.data.pagination.totalItems,
        page: response.data.pagination.currentPage,
        pageSize: response.data.pagination.pageSize,
        totalPages: response.data.pagination.totalPages,
      };
    }
    throw new Error(response.data.message || 'Erro ao carregar negócios');
  },

  getById: async (id: string): Promise<Business> => {
    const response = await api.get(`/business/${id}`);
    // Backend retorna { isSuccess, message, business }
    if (response.data.business) {
      return response.data.business;
    }
    throw new Error(response.data.message || 'Erro ao carregar negócio');
  },

  getPayments: async (id: string): Promise<Payment[]> => {
    const response = await api.get(`/business/${id}/payments`);
    // Backend retorna { isSuccess, message, businessPayments: { payments: [...] } }
    if (response.data.businessPayments?.payments) {
      return response.data.businessPayments.payments;
    }
    return [];
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
    await api.delete(`/business/${id}`, {
      data: {
        cancellationReason: "Cancelado pelo usuário",
        forceCancel: false
      }
    });
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/business/${id}`);
  },
};
