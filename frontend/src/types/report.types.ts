export interface PartnerReport {
  partnerId: string;
  partnerName: string;
  level: number;
  totalRecommended: number;
  totalEarned: number;
  totalPending: number;
  activeRecommendations: number;
}

// Tipos para a resposta real do backend
export interface PartnerTreeNode {
  id: string;
  name: string;
  email: string;
  phoneNumber: string;
  isActive: boolean;
  createdAt: string;
  level: number;
  totalReceived: number;
  totalPending: number;
  businessCount: number;
  recommenderId?: string;
  recommenderName: string;
  children: PartnerTreeNode[];
}

export interface LevelSummary {
  level: number;
  activePartnersCount: number;
  inactivePartnersCount: number;
  totalPartnersCount: number;
  totalReceived: number;
  totalPending: number;
  totalBusinessCount: number;
}

export interface ReportTotals {
  totalActivePartners: number;
  totalInactivePartners: number;
  totalPartners: number;
  totalReceived: number;
  totalPending: number;
  totalBusinessCount: number;
  maxDepth: number;
}

export interface PartnersReportData {
  vetorName: string;
  vetorId: string;
  generatedAt: string;
  partnersTree: PartnerTreeNode[];
  levelsSummary: LevelSummary[];
  totals: ReportTotals;
}

export interface PartnersReportResult {
  isSuccess: boolean;
  message: string;
  report?: PartnersReportData;
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
