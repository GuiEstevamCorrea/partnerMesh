export interface BusinessType {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateBusinessTypeRequest {
  name: string;
  description?: string;
}

export interface UpdateBusinessTypeRequest {
  name: string;
  description?: string;
  isActive: boolean;
}

export interface Business {
  id: string;
  partnerId: string;
  partnerName: string;
  partnerEmail?: string;
  partnerPhone?: string;
  businessTypeId: string;
  businessTypeName: string;
  businessTypeDescription?: string;
  value: number;
  date: string;
  observations?: string;
  status: 'Active' | 'Cancelled';
  totalCommission?: number;
  createdAt: string;
  updatedAt?: string;
  cancellationReason?: string;
  cancelledAt?: string;
  commission?: {
    commissionId: string;
    totalValue: number;
    createdAt: string;
    totalPayments: number;
    paidPayments: number;
    pendingPayments: number;
    cancelledPayments: number;
    totalPaidValue: number;
    totalPendingValue: number;
    totalCancelledValue: number;
    commissionStatus: string;
    payments: Array<{
      paymentId: string;
      partnerId: string;
      partnerName: string;
      paymentType: string;
      value: number;
      status: string;
      createdAt: string;
      paidOn?: string;
      level: string;
    }>;
  };
}

export interface CreateBusinessRequest {
  partnerIds: string[]; // Array de parceiros envolvidos no negócio
  businessTypeId: string;
  value: number;
  date: string;
  observations?: string;
}

export interface UpdateBusinessRequest {
  observations?: string;
}
