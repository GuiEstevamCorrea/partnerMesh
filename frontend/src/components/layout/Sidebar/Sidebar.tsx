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
  BarChart3,
  FileSearch,
} from 'lucide-react';
import { authStore } from '@/store/auth.store';
import { Permission } from '@/types';

interface MenuItem {
  path: string;
  label: string;
  icon: React.ReactNode;
  permissions: Permission[];
}

const menuItems: MenuItem[] = [
  {
    path: '/dashboard',
    label: 'Dashboard',
    icon: <LayoutDashboard className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal, Permission.AdminVetor, Permission.Operador],
  },
  {
    path: '/users',
    label: 'Usuários',
    icon: <Users className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal, Permission.AdminVetor],
  },
  {
    path: '/vectors',
    label: 'Vetores',
    icon: <GitBranch className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal],
  },
  {
    path: '/partners',
    label: 'Parceiros',
    icon: <UserCheck className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal, Permission.AdminVetor, Permission.Operador],
  },
  {
    path: '/business-types',
    label: 'Tipos de Negócio',
    icon: <Briefcase className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal, Permission.AdminVetor],
  },
  {
    path: '/business',
    label: 'Negócios',
    icon: <FileText className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal, Permission.AdminVetor, Permission.Operador],
  },
  {
    path: '/payments',
    label: 'Pagamentos',
    icon: <DollarSign className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal, Permission.AdminVetor, Permission.Operador],
  },
  {
    path: '/reports',
    label: 'Relatórios',
    icon: <BarChart3 className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal, Permission.AdminVetor, Permission.Operador],
  },
  {
    path: '/audit',
    label: 'Auditoria',
    icon: <FileSearch className="h-5 w-5" />,
    permissions: [Permission.AdminGlobal],
  },
];

export const Sidebar: React.FC = () => {
  const { hasPermission } = authStore();

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
            <span className="font-medium">{item.label}</span>
          </NavLink>
        ))}
      </nav>
    </aside>
  );
};
