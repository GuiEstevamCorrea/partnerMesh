import api from '../axios.config';
import { DashboardData } from '@/types/dashboard.types';

export const dashboardApi = {
  /**
   * Busca dados do dashboard (estatísticas, negócios recentes, pagamentos pendentes)
   */
  getDashboardData: async (): Promise<DashboardData> => {
    const { data } = await api.get<DashboardData>('/dashboard');
    return data;
  },
};
