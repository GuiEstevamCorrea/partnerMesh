export interface Payment {
  id: string;
  partnerId: string;
  partnerName: string;
  tipoPagamento: string;
  value: number;
  status: string;
  paidOn?: string;
  createdAt: string;
  // Campos legados (manter para compatibilidade)
  businessId?: string;
  recipientId?: string;
  recipientName?: string;
  recipientType?: 'Partner' | 'Vector';
  level?: number;
  paidAt?: string;
  paidByUserId?: string;
  paidByUserName?: string;
}

export interface ProcessPaymentRequest {
  paymentIds: string[];
}

export interface PaymentFilter {
  vectorId?: string;
  partnerId?: string;
  status?: 'Pending' | 'Paid' | 'Cancelled';
  startDate?: string;
  endDate?: string;
  level?: number;
  page?: number;
  pageSize?: number;
}
