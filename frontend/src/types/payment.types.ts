export interface Payment {
  id: string;
  businessId: string;
  recipientId: string;
  recipientName: string;
  recipientType: 'Partner' | 'Vector';
  value: number;
  level: number;
  status: 'Pending' | 'Paid';
  paidAt?: string;
  paidByUserId?: string;
  paidByUserName?: string;
  createdAt: string;
}

export interface ProcessPaymentRequest {
  paymentIds: string[];
}

export interface PaymentFilter {
  vectorId?: string;
  partnerId?: string;
  status?: 'Pending' | 'Paid';
  startDate?: string;
  endDate?: string;
  level?: number;
  page?: number;
  pageSize?: number;
}
