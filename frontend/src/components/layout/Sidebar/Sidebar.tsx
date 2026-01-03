import React from 'react';
import { NavLink } from 'react-router-dom';
import {
  LayoutDashboard,
  Users,
  GitBranch,
  UserCheck,
  Briefcase,
  FileText,
  DollarSign,
  FileSearch,
} from 'lucide-react';
import { useAuthStore } from '@/store/auth.store';
import { Permission } from '@/types';
import { useI18n } from '@/hooks/useI18n';

interface MenuItem {
  path: string;
  labelKey: string;
  icon: React.ReactNode;
  permissions: Permission[];
}

const menuItems: MenuItem[] = [
  {
    path: '/dashboard',
    labelKey: 'navigation.dashboard',
    icon: <LayoutDashboard className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal, Permission.AdminVetor, Permission.Operador],
  },
  {
    path: '/usuarios',
    labelKey: 'navigation.users',
    icon: <Users className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal, Permission.AdminVetor],
  },
  {
    path: '/vetores',
    labelKey: 'navigation.vectors',
    icon: <GitBranch className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal],
  },
  {
    path: '/parceiros',
    labelKey: 'navigation.partners',
    icon: <UserCheck className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal, Permission.AdminVetor, Permission.Operador],
  },
  {
    path: '/tipos-negocio',
    labelKey: 'navigation.businessTypes',
    icon: <Briefcase className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal, Permission.AdminVetor],
  },
  {
    path: '/negocios',
    labelKey: 'navigation.business',
    icon: <FileText className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal, Permission.AdminVetor, Permission.Operador],
  },
  {
    path: '/pagamentos',
    labelKey: 'navigation.payments',
    icon: <DollarSign className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal, Permission.AdminVetor, Permission.Operador],
  },
  {
    path: '/auditoria',
    labelKey: 'navigation.audit',
    icon: <FileSearch className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal],
  },
];

export const Sidebar: React.FC = () => {
  const hasPermission = useAuthStore((state) => state.hasPermission);
  const { t } = useI18n();

  const filteredMenuItems = menuItems.filter((item) =>
    hasPermission(item.permissions)
  );

  return (
    <aside className="w-64 bg-white border-r border-gray-200 h-full">
      <nav className="p-4 space-y-1">
        {filteredMenuItems.map((item) => (
          <NavLink
            key={item.path}
            to={item.path}
            className={({ isActive }) =>
              `flex items-center space-x-3 px-4 py-3 rounded-md transition-colors ${
                isActive
                  ? 'bg-black text-white'
                  : 'text-gray-700 hover:bg-gray-100'
              }`
            }
          >
            {item.icon}
            <span className="font-medium">{t(item.labelKey)}</span>
          </NavLink>
        ))}
      </nav>
    </aside>
  );
};
