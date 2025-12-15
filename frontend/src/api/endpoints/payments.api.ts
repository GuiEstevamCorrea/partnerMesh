import api from '../axios.config';
import { Payment, ProcessPaymentRequest, PaymentFilter, PaginatedResponse } from '@/types';

export const paymentsApi = {
  list: async (params?: PaymentFilter): Promise<PaginatedResponse<Payment>> => {
    const response = await api.get('/payments', { params });
    return response.data;
  },

  getById: async (id: string): Promise<Payment> => {
    const response = await api.get(`/payments/${id}`);
    return response.data;
  },

  process: async (data: ProcessPaymentRequest): Promise<void> => {
    await api.post('/payments/process', data);
  },
};
