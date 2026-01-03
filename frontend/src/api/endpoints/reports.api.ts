import api from '../axios.config';
import { 
  PartnerReport, 
  FinancialReport, 
  BusinessReport,
  ReportFilter,
  PaginatedResponse,
  PartnersReportResult,
  PartnerTreeNode
} from '@/types';

export const reportsApi = {
  partners: async (params?: ReportFilter): Promise<PaginatedResponse<PartnerReport>> => {
    const response = await api.get('/reports/partners', { params });
    const result: PartnersReportResult = response.data;
    
    if (!result.isSuccess || !result.report) {
      throw new Error(result.message || 'Erro ao buscar relatório de parceiros');
    }

    // Converter estrutura hierárquica em lista plana para compatibilidade
    const flattenPartners = (nodes: PartnerTreeNode[]): PartnerReport[] => {
      const result: PartnerReport[] = [];
      
      for (const node of nodes) {
        result.push({
          partnerId: node.id,
          partnerName: node.name,
          level: node.level,
          totalRecommended: node.children.length,
          totalEarned: node.totalReceived,
          totalPending: node.totalPending,
          activeRecommendations: node.children.filter(child => child.isActive).length,
        });
        
        // Adicionar filhos recursivamente
        if (node.children && node.children.length > 0) {
          result.push(...flattenPartners(node.children));
        }
      }
      
      return result;
    };

    const items = flattenPartners(result.report.partnersTree);

    return {
      items,
      totalItems: items.length,
      currentPage: params?.page || 1,
      totalPages: Math.ceil(items.length / (params?.pageSize || 20)),
    };
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
