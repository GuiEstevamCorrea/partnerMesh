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
  phoneNumber: string;
  email: string;
  vetorId: string;
  recommenderId?: string;
}

export interface UpdatePartnerRequest {
  name: string;
  contact: string;
  isActive: boolean;
}

export interface PartnerTree {
  vetor: {
    id: string;
    name: string;
    email: string;
    isActive: boolean;
  };
  rootPartners: PartnerTreeNode[];
  orphanPartners: PartnerTreeNode[];
}

export interface PartnerTreeNode {
  id: string;
  name: string;
  email: string;
  phoneNumber: string;
  isActive: boolean;
  createdAt: string;
  level: number;
  recommenderId?: string;
  recommenderName?: string;
  children: PartnerTreeNode[];
  stats: {
    directChildren: number;
    activeDirectChildren: number;
    totalDescendants: number;
    activeDescendants: number;
    maxDepth: number;
  };
}
