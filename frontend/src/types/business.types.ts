export interface BusinessType {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateBusinessTypeRequest {
  name: string;
  description?: string;
}

export interface UpdateBusinessTypeRequest {
  name: string;
  description?: string;
  isActive: boolean;
}

export interface Business {
  id: string;
  partnerId: string;
  partnerName: string;
  businessTypeId: string;
  businessTypeName: string;
  value: number;
  date: string;
  observations?: string;
  status: 'Active' | 'Cancelled';
  totalCommission: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateBusinessRequest {
  partnerId: string;
  businessTypeId: string;
  value: number;
  date: string;
  observations?: string;
}

export interface UpdateBusinessRequest {
  observations?: string;
}
