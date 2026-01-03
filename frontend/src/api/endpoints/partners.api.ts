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
    
    // A API retorna: { isSuccess, message, partners: [...], pagination: {...} }
    if (response.data?.partners && response.data?.pagination) {
      return {
        items: response.data.partners.map(adaptPartnerFromApi),
        page: response.data.pagination.page,
        pageSize: response.data.pagination.pageSize,
        totalItems: response.data.pagination.totalItems,
        totalPages: response.data.pagination.totalPages,
      };
    }
    
    return response.data;
  },

  getById: async (id: string): Promise<Partner> => {
    const response = await api.get(`/partners/${id}`);
    
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
    
    // A API retorna: { isSuccess, message, tree: {...} }
    if (response.data?.tree) {
      return response.data.tree;
    }
    return response.data;
  },

  create: async (data: CreatePartnerRequest): Promise<Partner> => {
    const response = await api.post('/partners', data);
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
    // Primeiro, buscar o parceiro atual para saber o estado
    const partner = await api.get(`/partners/${id}`);
    const isActive = partner.data?.partner?.isActive ?? false;
    
    // Preparar o request para inverter o estado
    const requestBody = {
      active: !isActive,
      reason: !isActive ? 'Ativação via interface' : 'Desativação via interface'
    };
    
    await api.patch(`/partners/${id}/status`, requestBody);
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/partners/${id}`);
  },
};
