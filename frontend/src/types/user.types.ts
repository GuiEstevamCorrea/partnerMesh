import { Permission } from './auth.types';

export interface CreateUserRequest {
  name: string;
  email: string;
  password: string;
  permission: Permission;
  vectorId?: string;
}

export interface UpdateUserRequest {
  name?: string;
  email?: string;
  permission?: Permission;
  vectorId?: string;
  isActive?: boolean;
}
