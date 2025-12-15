import api from '../axios.config';
import { 
  PartnerReport, 
  FinancialReport, 
  BusinessReport,
  ReportFilter,
  PaginatedResponse 
} from '@/types';

export const reportsApi = {
  partners: async (params?: ReportFilter): Promise<PaginatedResponse<PartnerReport>> => {
    const response = await api.get('/reports/partners', { params });
    return response.data;
  },

  financial: async (params?: ReportFilter): Promise<FinancialReport> => {
    const response = await api.get('/reports/financial', { params });
    return response.data;
  },

  business: async (params?: ReportFilter): Promise<PaginatedResponse<BusinessReport>> => {
    const response = await api.get('/reports/business', { params });
    return response.data;
  },
};
