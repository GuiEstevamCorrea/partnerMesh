export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  user: User;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export enum Permission {
  AdminGlobal = 'AdminGlobal',
  AdminVetor = 'AdminVetor',
  Operador = 'Operador',
  Parceiro = 'Parceiro'
}

export interface User {
  id: string;
  name: string;
  email: string;
  permission: Permission;
  vectorId?: string;
  vectorName?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}
