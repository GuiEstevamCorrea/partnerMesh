import api from '../axios.config';
import { Vector, CreateVectorRequest, UpdateVectorRequest, PaginatedResponse, FilterParams } from '@/types';

export interface VectorsFilterParams extends FilterParams {
  isActive?: boolean;
}

export const vectorsApi = {
  list: async (params?: VectorsFilterParams): Promise<PaginatedResponse<Vector>> => {
    const response = await api.get('/vectors', { params });
    
    // Transformar resposta da API para o formato esperado pelo frontend
    if (response.data?.data) {
      const apiData = response.data.data;
      const transformedItems = (apiData.vetores || []).map((vetor: any) => ({
        id: vetor.id,
        name: vetor.name,
        email: vetor.email,
        login: vetor.email, // Usar email como login por enquanto
        isActive: vetor.active,
        partnersCount: vetor.totalParceiros || 0,
        createdAt: vetor.createdAt,
        updatedAt: vetor.createdAt, // Usar createdAt como updatedAt por enquanto
      }));
      
      return {
        items: transformedItems,
        currentPage: apiData.currentPage,
        pageSize: apiData.pageSize,
        totalItems: apiData.totalVetores,
        totalPages: apiData.totalPages,
      };
    }
    
    return response.data;
  },

  getById: async (id: string): Promise<Vector> => {
    const response = await api.get(`/vectors/${id}`);
    return response.data;
  },

  create: async (data: CreateVectorRequest): Promise<Vector> => {
    const response = await api.post('/vectors', data);
    return response.data;
  },

  update: async (id: string, data: UpdateVectorRequest): Promise<Vector> => {
    const response = await api.put(`/vectors/${id}`, data);
    return response.data;
  },

  toggleActive: async (id: string): Promise<void> => {
    await api.patch(`/vectors/${id}/toggle-active`);
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/vectors/${id}`);
  },
};
