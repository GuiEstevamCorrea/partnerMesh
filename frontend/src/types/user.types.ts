import { Permission } from './auth.types';

export interface CreateUserRequest {
  name: string;
  email: string;
  password: string;
  permission: Permission;
  vetorId?: string | null;
}

export interface UpdateUserRequest {
  name?: string;
  email?: string;
  permission?: Permission;
  vetorId?: string | null;
  isActive?: boolean;
}
