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
    
    console.log('🔍 businessTypesApi.list - Resposta completa:', response.data);
    
    // A API retorna: { isSuccess, message, BusinessTypes: [...], Pagination: {...} }
    // ou { isSuccess, message, businessTypes: [...], pagination: {...} }
    const businessTypes = response.data?.BusinessTypes || response.data?.businessTypes;
    const pagination = response.data?.Pagination || response.data?.pagination;
    
    console.log('🔍 businessTypes array:', businessTypes);
    console.log('🔍 pagination:', pagination);
    
    if (businessTypes && pagination) {
      const items = businessTypes.map(adaptBusinessTypeFromApi);
      console.log('✅ Items adaptados:', items);
      
      return {
        items,
        currentPage: pagination.page || pagination.Page,
        pageSize: pagination.pageSize || pagination.PageSize,
        totalItems: pagination.totalItems || pagination.TotalItems,
        totalPages: pagination.totalPages || pagination.TotalPages,
      };
    }
    
    console.error('❌ Formato inesperado da resposta');
    return { items: [], currentPage: 1, pageSize: 10, totalItems: 0, totalPages: 0 };
  },

  getById: async (id: string): Promise<BusinessType> => {
    const response = await api.get(`/business-types/${id}`);
    console.log('businessTypesApi.getById - Resposta completa:', response.data);
    
    // A API retorna: { isSuccess, message, businessType: {...} }
    if (response.data?.businessType) {
      return adaptBusinessTypeFromApi(response.data.businessType);
    }
    
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
