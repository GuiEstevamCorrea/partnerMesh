import api from '../axios.config';
import { 
  Partner, 
  CreatePartnerRequest, 
  UpdatePartnerRequest, 
  PartnerTree,
  PaginatedResponse, 
  FilterParams 
} from '@/types';

// Adaptador para transformar resposta da API
const adaptPartnerFromApi = (apiPartner: any): Partner => {
  // Para GetById, vetorId vem em uppercase V, para List vem em lowercase v
  const vectorId = apiPartner.vetorId || apiPartner.VetorId;
  
  return {
    id: apiPartner.id,
    name: apiPartner.name,
    contact: apiPartner.phoneNumber || apiPartner.email,
    recommenderId: apiPartner.recommenderId,
    recommenderName: apiPartner.recommenderName,
    // Se tem recommenderId mas não tem recommenderType, assumir que é Partner
    recommenderType: apiPartner.recommenderId ? 'Partner' : undefined,
    vectorId: vectorId,
    vectorName: apiPartner.vetorName || apiPartner.VetorName,
    isActive: apiPartner.isActive,
    level: apiPartner.level || 0,
    totalRecommended: apiPartner.totalRecommended || 0,
    totalEarned: apiPartner.totalEarned || 0,
    totalPending: apiPartner.totalPending || 0,
    createdAt: apiPartner.createdAt,
    updatedAt: apiPartner.updatedAt,
  };
};

export const partnersApi = {
  list: async (params?: FilterParams): Promise<PaginatedResponse<Partner>> => {
    const response = await api.get('/partners', { params });
    
    console.log('partnersApi.list - Resposta completa:', response.data);
    
    // A API retorna: { isSuccess, message, partners: [...], pagination: {...} }
    if (response.data?.partners && response.data?.pagination) {
      return {
        items: response.data.partners.map(adaptPartnerFromApi),
        currentPage: response.data.pagination.page,
        pageSize: response.data.pagination.pageSize,
        totalItems: response.data.pagination.totalItems,
        totalPages: response.data.pagination.totalPages,
      };
    }
    
    return response.data;
  },

  getById: async (id: string): Promise<Partner> => {
    const response = await api.get(`/partners/${id}`);
    console.log('partnersApi.getById - Resposta completa:', response.data);
    
    // A API retorna: { isSuccess, message, partner: {...} }
    if (response.data?.partner) {
      return adaptPartnerFromApi(response.data.partner);
    }
    return response.data;
  },

  getTree: async (id: string): Promise<PartnerTree> => {
    // Determinar se o ID é de um vetor ou parceiro
    // Se for chamado com vectorId do user, usar vetorId
    // Se for chamado com partnerId, usar rootPartnerId
    const params: any = {};
    
    // Assumindo que se vier do usuário é vetorId, senão é partnerId
    // Você pode ajustar essa lógica conforme necessário
    const response = await api.get('/partners/tree', { 
      params: { rootPartnerId: id }
    });
    
    console.log('partnersApi.getTree - Resposta completa:', response.data);
    
    // A API retorna: { isSuccess, message, tree: {...} }
    if (response.data?.tree) {
      return response.data.tree;
    }
    return response.data;
  },

  create: async (data: CreatePartnerRequest): Promise<Partner> => {
    console.log('partnersApi.create - Dados recebidos:', data);
    const response = await api.post('/partners', data);
    console.log('partnersApi.create - Resposta da API:', response);
    if (response.data?.partner) {
      return adaptPartnerFromApi(response.data.partner);
    }
    return response.data;
  },

  update: async (id: string, data: UpdatePartnerRequest): Promise<Partner> => {
    const response = await api.put(`/partners/${id}`, data);
    if (response.data?.partner) {
      return adaptPartnerFromApi(response.data.partner);
    }
    return response.data;
  },

  toggleActive: async (id: string): Promise<void> => {
    await api.patch(`/partners/${id}/toggle-active`);
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/partners/${id}`);
  },
};
