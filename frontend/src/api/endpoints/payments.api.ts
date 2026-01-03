import api from '../axios.config';
import { Payment, ProcessPaymentRequest, PaymentFilter, PaginatedResponse } from '@/types';

export interface CancelledBusinessSummary {
  totalCancelledBusinesses: number;
  totalCancelledValue: number;
  totalCancelledCommissions: number;
  totalCancelledPayments: number;
  cancelledPaymentsCount: number;
  paidBeforeCancellation: number;
  paidBeforeCancellationCount: number;
}

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

  getCancelledBusinessSummary: async (): Promise<CancelledBusinessSummary> => {
    const response = await api.get('/payments/cancelled-business-summary');
    return response.data.summary;
  },
};
