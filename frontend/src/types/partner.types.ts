export interface Partner {
  id: string;
  name: string;
  contact: string;
  recommenderId?: string;
  recommenderName?: string;
  recommenderType?: 'Partner' | 'Vector';
  vectorId: string;
  vectorName: string;
  isActive: boolean;
  level: number;
  totalRecommended: number;
  totalEarned: number;
  totalPending: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreatePartnerRequest {
  name: string;
  contact: string;
  recommenderId?: string;
  recommenderType?: 'Partner' | 'Vector';
  vectorId: string;
}

export interface UpdatePartnerRequest {
  name: string;
  contact: string;
  isActive: boolean;
}

export interface PartnerTree {
  id: string;
  name: string;
  level: number;
  totalRecommended: number;
  children: PartnerTree[];
}
