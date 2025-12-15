export interface PartnerReport {
  partnerId: string;
  partnerName: string;
  level: number;
  totalRecommended: number;
  totalEarned: number;
  totalPending: number;
  activeRecommendations: number;
}

export interface FinancialReport {
  totalPaid: number;
  totalPending: number;
  totalBusiness: number;
  totalPartners: number;
  paymentsByLevel: {
    level: number;
    total: number;
  }[];
  paymentsByMonth: {
    month: string;
    total: number;
  }[];
}

export interface BusinessReport {
  businessId: string;
  partnerName: string;
  businessTypeName: string;
  value: number;
  totalCommission: number;
  date: string;
  status: string;
  paymentsPending: number;
  paymentsPaid: number;
}

export interface ReportFilter {
  vectorId?: string;
  startDate?: string;
  endDate?: string;
  page?: number;
  pageSize?: number;
}
