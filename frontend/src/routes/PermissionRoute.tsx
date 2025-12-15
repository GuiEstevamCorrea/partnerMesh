import React from 'react';
import { Navigate } from 'react-router-dom';
import { authStore } from '@/store/auth.store';
import { Permission } from '@/types';

interface PermissionRouteProps {
  children: React.ReactNode;
  requiredPermissions: Permission[];
}

export const PermissionRoute: React.FC<PermissionRouteProps> = ({
  children,
  requiredPermissions,
}) => {
  const { hasPermission } = authStore();

  if (!hasPermission(requiredPermissions)) {
    return <Navigate to="/dashboard" replace />;
  }

  return <>{children}</>;
};
