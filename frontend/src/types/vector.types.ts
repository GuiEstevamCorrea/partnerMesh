export interface Vector {
  id: string;
  name: string;
  email: string;
  login: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateVectorRequest {
  name: string;
  email: string;
  login: string;
  adminUserId?: string;
}

export interface UpdateVectorRequest {
  name: string;
  email: string;
  isActive: boolean;
}
