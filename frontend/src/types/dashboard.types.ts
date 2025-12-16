export interface DashboardStats {
  totalActiveUsers: number;
  totalActiveVectors: number;
  totalPartners: number;
  totalBusinessThisMonth: number;
  pendingCommissionsAmount: number;
  paidCommissionsThisMonth: number;
}

export interface DashboardRecentBusiness {
  id: string;
  partnerName: string;
  businessTypeName: string;
  value: number;
  totalCommission: number;
  createdAt: string;
  status: 'active' | 'cancelled';
}

export interface DashboardPendingPayment {
  id: string;
  recipientName: string;
  businessId: string;
  amount: number;
  level: 1 | 2 | 3;
  createdAt: string;
}

export interface DashboardData {
  stats: DashboardStats;
  recentBusiness: DashboardRecentBusiness[];
  pendingPayments: DashboardPendingPayment[];
}
