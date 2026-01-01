import api from '../axios.config';
import { 
  BusinessType, 
  CreateBusinessTypeRequest, 
  UpdateBusinessTypeRequest,
  PaginatedResponse, 
  FilterParams 
} from '@/types';

// Adaptador para transformar resposta da API
const adaptBusinessTypeFromApi = (apiBusinessType: any): BusinessType => ({
  id: apiBusinessType.id,
  name: apiBusinessType.name,
  description: apiBusinessType.description,
  isActive: apiBusinessType.active !== undefined ? apiBusinessType.active : apiBusinessType.isActive,
  createdAt: apiBusinessType.createdAt,
  updatedAt: apiBusinessType.lastModified || apiBusinessType.updatedAt,
});

export const businessTypesApi = {
  list: async (params?: FilterParams): Promise<PaginatedResponse<BusinessType>> => {
    const response = await api.get('/business-types', { params });
    
    console.log('businessTypesApi.list - Resposta completa:', response.data);
    
    // A API retorna: { isSuccess, message, businessTypes: [...], pagination: {...} }
    if (response.data?.businessTypes && response.data?.pagination) {
      return {
        items: response.data.businessTypes.map(adaptBusinessTypeFromApi),
        currentPage: response.data.pagination.page,
        pageSize: response.data.pagination.pageSize,
        totalItems: response.data.pagination.totalItems,
        totalPages: response.data.pagination.totalPages,
      };
    }
    
    return response.data;
  },

  getById: async (id: string): Promise<BusinessType> => {
    const response = await api.get(`/business-types/${id}`);
    return response.data;
  },

  create: async (data: CreateBusinessTypeRequest): Promise<BusinessType> => {
    const response = await api.post('/business-types', data);
    return response.data;
  },

  update: async (id: string, data: UpdateBusinessTypeRequest): Promise<BusinessType> => {
    const response = await api.put(`/business-types/${id}`, data);
    return response.data;
  },

  toggleActive: async (id: string): Promise<void> => {
    await api.patch(`/business-types/${id}/toggle-active`);
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/business-types/${id}`);
  },
};
