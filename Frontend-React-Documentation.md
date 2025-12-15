# Documentação Frontend React - Sistema de Rede de Credenciamento / Vetores

## 1. Visão Geral

Este documento descreve a implementação do frontend em React para o Sistema de Rede de Credenciamento / Vetores, conectado ao backend já desenvolvido em .NET 8.

### 1.1 Tecnologias

- **React 18+** com TypeScript
- **Vite** como bundler
- **React Router v6** para navegação
- **Axios** para comunicação com API
- **React Query (TanStack Query)** para gerenciamento de estado do servidor
- **Zustand** para estado global da aplicação
- **React Hook Form** para formulários
- **Zod** para validação de dados
- **Tailwind CSS** para estilização (tema preto e branco)
- **Lucide React** para ícones

### 1.2 Paleta de Cores (Preto e Branco)

```css
/* Cores principais */
--color-black: #000000;
--color-white: #FFFFFF;
--color-gray-50: #FAFAFA;
--color-gray-100: #F5F5F5;
--color-gray-200: #E5E5E5;
--color-gray-300: #D4D4D4;
--color-gray-400: #A3A3A3;
--color-gray-500: #737373;
--color-gray-600: #525252;
--color-gray-700: #404040;
--color-gray-800: #262626;
--color-gray-900: #171717;

/* Estados */
--color-success: #000000;
--color-error: #404040;
--color-warning: #525252;
--color-info: #262626;
```

---

## 2. Estrutura de Pastas

```
frontend/
├── public/
│   └── favicon.ico
├── src/
│   ├── @types/
│   │   └── index.d.ts
│   ├── api/
│   │   ├── axios.config.ts
│   │   ├── endpoints/
│   │   │   ├── auth.api.ts
│   │   │   ├── users.api.ts
│   │   │   ├── vectors.api.ts
│   │   │   ├── partners.api.ts
│   │   │   ├── businessTypes.api.ts
│   │   │   ├── business.api.ts
│   │   │   ├── payments.api.ts
│   │   │   ├── reports.api.ts
│   │   │   └── audit.api.ts
│   │   └── interceptors.ts
│   ├── assets/
│   │   └── images/
│   ├── components/
│   │   ├── common/
│   │   │   ├── Button/
│   │   │   │   ├── Button.tsx
│   │   │   │   └── Button.styles.ts
│   │   │   ├── Input/
│   │   │   ├── Select/
│   │   │   ├── Table/
│   │   │   ├── Modal/
│   │   │   ├── Card/
│   │   │   ├── Loading/
│   │   │   ├── Alert/
│   │   │   └── Pagination/
│   │   ├── layout/
│   │   │   ├── Header/
│   │   │   ├── Sidebar/
│   │   │   ├── Footer/
│   │   │   └── Layout.tsx
│   │   └── features/
│   │       ├── auth/
│   │       │   ├── LoginForm.tsx
│   │       │   └── ChangePasswordForm.tsx
│   │       ├── users/
│   │       │   ├── UserList.tsx
│   │       │   ├── UserForm.tsx
│   │       │   └── UserDetails.tsx
│   │       ├── vectors/
│   │       │   ├── VectorList.tsx
│   │       │   ├── VectorForm.tsx
│   │       │   └── VectorDetails.tsx
│   │       ├── partners/
│   │       │   ├── PartnerList.tsx
│   │       │   ├── PartnerForm.tsx
│   │       │   ├── PartnerDetails.tsx
│   │       │   └── PartnerTree.tsx
│   │       ├── businessTypes/
│   │       │   ├── BusinessTypeList.tsx
│   │       │   └── BusinessTypeForm.tsx
│   │       ├── business/
│   │       │   ├── BusinessList.tsx
│   │       │   ├── BusinessForm.tsx
│   │       │   └── BusinessDetails.tsx
│   │       ├── payments/
│   │       │   ├── PaymentList.tsx
│   │       │   └── PaymentActions.tsx
│   │       ├── reports/
│   │       │   ├── PartnersReport.tsx
│   │       │   ├── FinancialReport.tsx
│   │       │   └── BusinessReport.tsx
│   │       └── audit/
│   │           └── AuditLogList.tsx
│   ├── hooks/
│   │   ├── useAuth.ts
│   │   ├── usePermission.ts
│   │   ├── useDebounce.ts
│   │   └── useToast.ts
│   ├── pages/
│   │   ├── Login.tsx
│   │   ├── Dashboard.tsx
│   │   ├── Users/
│   │   ├── Vectors/
│   │   ├── Partners/
│   │   ├── BusinessTypes/
│   │   ├── Business/
│   │   ├── Payments/
│   │   ├── Reports/
│   │   ├── Audit/
│   │   └── NotFound.tsx
│   ├── routes/
│   │   ├── index.tsx
│   │   ├── PrivateRoute.tsx
│   │   └── PermissionRoute.tsx
│   ├── services/
│   │   ├── auth.service.ts
│   │   ├── storage.service.ts
│   │   └── format.service.ts
│   ├── store/
│   │   ├── auth.store.ts
│   │   ├── user.store.ts
│   │   └── app.store.ts
│   ├── styles/
│   │   ├── global.css
│   │   └── tailwind.css
│   ├── types/
│   │   ├── auth.types.ts
│   │   ├── user.types.ts
│   │   ├── vector.types.ts
│   │   ├── partner.types.ts
│   │   ├── business.types.ts
│   │   ├── payment.types.ts
│   │   ├── report.types.ts
│   │   └── common.types.ts
│   ├── utils/
│   │   ├── constants.ts
│   │   ├── validators.ts
│   │   ├── formatters.ts
│   │   └── helpers.ts
│   ├── App.tsx
│   ├── main.tsx
│   └── vite-env.d.ts
├── .env.example
├── .gitignore
├── index.html
├── package.json
├── tsconfig.json
├── tailwind.config.js
├── vite.config.ts
└── README.md
```

---

## 3. Configuração Inicial

### 3.1 Criar Projeto

```bash
npm create vite@latest frontend -- --template react-ts
cd frontend
```

### 3.2 Instalar Dependências

```bash
# Dependências principais
npm install react-router-dom axios @tanstack/react-query zustand
npm install react-hook-form @hookform/resolvers zod
npm install lucide-react date-fns

# Tailwind CSS
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p

# TypeScript types
npm install -D @types/node
```

### 3.3 Configurar Tailwind CSS

**tailwind.config.js:**

```javascript
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        black: '#000000',
        white: '#FFFFFF',
        gray: {
          50: '#FAFAFA',
          100: '#F5F5F5',
          200: '#E5E5E5',
          300: '#D4D4D4',
          400: '#A3A3A3',
          500: '#737373',
          600: '#525252',
          700: '#404040',
          800: '#262626',
          900: '#171717',
        },
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
```

### 3.4 Variáveis de Ambiente

**.env.example:**

```env
VITE_API_BASE_URL=http://localhost:5000/api
VITE_APP_NAME=Sistema de Rede de Credenciamento
```

---

## 4. Tipos TypeScript

### 4.1 auth.types.ts

```typescript
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
}
```

### 4.2 vector.types.ts

```typescript
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
```

### 4.3 partner.types.ts

```typescript
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
}

export interface CreatePartnerRequest {
  name: string;
  contact: string;
  recommenderId?: string;
  recommenderType?: 'Partner' | 'Vector';
  vectorId: string;
}

export interface PartnerTree {
  id: string;
  name: string;
  level: number;
  totalRecommended: number;
  children: PartnerTree[];
}
```

### 4.4 business.types.ts

```typescript
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
}

export interface CreateBusinessRequest {
  partnerId: string;
  businessTypeId: string;
  value: number;
  date: string;
  observations?: string;
}

export interface BusinessType {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
}
```

### 4.5 payment.types.ts

```typescript
export interface Payment {
  id: string;
  businessId: string;
  recipientId: string;
  recipientName: string;
  recipientType: 'Partner' | 'Vector';
  value: number;
  level: number;
  status: 'Pending' | 'Paid';
  paidAt?: string;
  paidByUserId?: string;
  paidByUserName?: string;
  createdAt: string;
}

export interface ProcessPaymentRequest {
  paymentIds: string[];
}

export interface PaymentFilter {
  vectorId?: string;
  partnerId?: string;
  status?: 'Pending' | 'Paid';
  startDate?: string;
  endDate?: string;
  level?: number;
  page?: number;
  pageSize?: number;
}
```

### 4.6 common.types.ts

```typescript
export interface PaginatedResponse<T> {
  items: T[];
  totalItems: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}

export interface SelectOption {
  value: string;
  label: string;
}
```

---

## 5. Configuração da API

### 5.1 axios.config.ts

```typescript
import axios from 'axios';
import { authStore } from '../store/auth.store';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para adicionar token
api.interceptors.request.use(
  (config) => {
    const token = authStore.getState().token;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor para tratar erros
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // Token expirado
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = authStore.getState().refreshToken;
        if (refreshToken) {
          const response = await axios.post(
            `${import.meta.env.VITE_API_BASE_URL}/auth/refresh`,
            { refreshToken }
          );

          const { token, refreshToken: newRefreshToken } = response.data;
          authStore.getState().setTokens(token, newRefreshToken);

          originalRequest.headers.Authorization = `Bearer ${token}`;
          return api(originalRequest);
        }
      } catch (refreshError) {
        authStore.getState().logout();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default api;
```

### 5.2 Exemplo de Endpoint - auth.api.ts

```typescript
import api from '../axios.config';
import { LoginRequest, LoginResponse, RefreshTokenRequest, ChangePasswordRequest } from '../../types/auth.types';

export const authApi = {
  login: async (data: LoginRequest): Promise<LoginResponse> => {
    const response = await api.post('/auth/login', data);
    return response.data;
  },

  refreshToken: async (data: RefreshTokenRequest): Promise<LoginResponse> => {
    const response = await api.post('/auth/refresh', data);
    return response.data;
  },

  logout: async (): Promise<void> => {
    await api.post('/auth/logout');
  },

  changePassword: async (data: ChangePasswordRequest): Promise<void> => {
    await api.post('/auth/change-password', data);
  },
};
```

### 5.3 Exemplo de Endpoint - partners.api.ts

```typescript
import api from '../axios.config';
import { Partner, CreatePartnerRequest, PartnerTree, PaginatedResponse } from '../../types';

export const partnersApi = {
  list: async (params?: {
    vectorId?: string;
    isActive?: boolean;
    search?: string;
    page?: number;
    pageSize?: number;
  }): Promise<PaginatedResponse<Partner>> => {
    const response = await api.get('/partners', { params });
    return response.data;
  },

  getById: async (id: string): Promise<Partner> => {
    const response = await api.get(`/partners/${id}`);
    return response.data;
  },

  create: async (data: CreatePartnerRequest): Promise<Partner> => {
    const response = await api.post('/partners', data);
    return response.data;
  },

  update: async (id: string, data: Partial<CreatePartnerRequest>): Promise<Partner> => {
    const response = await api.put(`/partners/${id}`, data);
    return response.data;
  },

  toggleActive: async (id: string): Promise<void> => {
    await api.patch(`/partners/${id}/toggle-active`);
  },

  getTree: async (vectorId?: string): Promise<PartnerTree> => {
    const params = vectorId ? { vectorId } : undefined;
    const response = await api.get('/partners/tree', { params });
    return response.data;
  },
};
```

---

## 6. Gerenciamento de Estado

### 6.1 auth.store.ts (Zustand)

```typescript
import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { User, Permission } from '../types/auth.types';

interface AuthState {
  token: string | null;
  refreshToken: string | null;
  user: User | null;
  isAuthenticated: boolean;
  
  setTokens: (token: string, refreshToken: string) => void;
  setUser: (user: User) => void;
  logout: () => void;
  hasPermission: (requiredPermissions: Permission[]) => boolean;
  hasVectorAccess: (vectorId: string) => boolean;
}

export const authStore = create<AuthState>()(
  persist(
    (set, get) => ({
      token: null,
      refreshToken: null,
      user: null,
      isAuthenticated: false,

      setTokens: (token, refreshToken) => {
        set({ token, refreshToken, isAuthenticated: true });
      },

      setUser: (user) => {
        set({ user });
      },

      logout: () => {
        set({
          token: null,
          refreshToken: null,
          user: null,
          isAuthenticated: false,
        });
      },

      hasPermission: (requiredPermissions) => {
        const { user } = get();
        if (!user) return false;
        
        // Admin Global tem todas as permissões
        if (user.permission === Permission.AdminGlobal) return true;
        
        return requiredPermissions.includes(user.permission);
      },

      hasVectorAccess: (vectorId) => {
        const { user } = get();
        if (!user) return false;
        
        // Admin Global tem acesso a todos os vetores
        if (user.permission === Permission.AdminGlobal) return true;
        
        // Outros usuários só têm acesso ao seu próprio vetor
        return user.vectorId === vectorId;
      },
    }),
    {
      name: 'auth-storage',
    }
  )
);
```

---

## 7. Componentes Comuns

### 7.1 Button Component

```typescript
// src/components/common/Button/Button.tsx
import React from 'react';
import { Loader2 } from 'lucide-react';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger';
  size?: 'sm' | 'md' | 'lg';
  isLoading?: boolean;
  icon?: React.ReactNode;
}

export const Button: React.FC<ButtonProps> = ({
  children,
  variant = 'primary',
  size = 'md',
  isLoading = false,
  icon,
  className = '',
  disabled,
  ...props
}) => {
  const baseStyles = 'inline-flex items-center justify-center font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed';
  
  const variants = {
    primary: 'bg-black text-white hover:bg-gray-800 focus:ring-black',
    secondary: 'bg-gray-200 text-black hover:bg-gray-300 focus:ring-gray-400',
    outline: 'border-2 border-black text-black hover:bg-black hover:text-white focus:ring-black',
    ghost: 'text-black hover:bg-gray-100 focus:ring-gray-300',
    danger: 'bg-gray-700 text-white hover:bg-gray-800 focus:ring-gray-700',
  };

  const sizes = {
    sm: 'px-3 py-1.5 text-sm rounded',
    md: 'px-4 py-2 text-base rounded-md',
    lg: 'px-6 py-3 text-lg rounded-lg',
  };

  return (
    <button
      className={`${baseStyles} ${variants[variant]} ${sizes[size]} ${className}`}
      disabled={disabled || isLoading}
      {...props}
    >
      {isLoading ? (
        <>
          <Loader2 className="mr-2 h-4 w-4 animate-spin" />
          Carregando...
        </>
      ) : (
        <>
          {icon && <span className="mr-2">{icon}</span>}
          {children}
        </>
      )}
    </button>
  );
};
```

### 7.2 Input Component

```typescript
// src/components/common/Input/Input.tsx
import React, { forwardRef } from 'react';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  helperText?: string;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ label, error, helperText, className = '', ...props }, ref) => {
    return (
      <div className="w-full">
        {label && (
          <label className="block text-sm font-medium text-gray-900 mb-1">
            {label}
            {props.required && <span className="text-gray-700 ml-1">*</span>}
          </label>
        )}
        <input
          ref={ref}
          className={`
            w-full px-3 py-2 
            border-2 border-gray-300 rounded-md
            text-gray-900 placeholder-gray-400
            focus:outline-none focus:ring-2 focus:ring-black focus:border-black
            disabled:bg-gray-100 disabled:cursor-not-allowed
            ${error ? 'border-gray-700' : ''}
            ${className}
          `}
          {...props}
        />
        {error && (
          <p className="mt-1 text-sm text-gray-700">{error}</p>
        )}
        {helperText && !error && (
          <p className="mt-1 text-sm text-gray-500">{helperText}</p>
        )}
      </div>
    );
  }
);

Input.displayName = 'Input';
```

### 7.3 Table Component

```typescript
// src/components/common/Table/Table.tsx
import React from 'react';

interface Column<T> {
  key: string;
  header: string;
  render?: (item: T) => React.ReactNode;
  width?: string;
}

interface TableProps<T> {
  data: T[];
  columns: Column<T>[];
  onRowClick?: (item: T) => void;
  isLoading?: boolean;
  emptyMessage?: string;
}

export function Table<T extends { id: string }>({
  data,
  columns,
  onRowClick,
  isLoading,
  emptyMessage = 'Nenhum registro encontrado',
}: TableProps<T>) {
  if (isLoading) {
    return (
      <div className="flex justify-center items-center py-12">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-black"></div>
      </div>
    );
  }

  if (data.length === 0) {
    return (
      <div className="text-center py-12 text-gray-500">
        {emptyMessage}
      </div>
    );
  }

  return (
    <div className="overflow-x-auto border border-gray-200 rounded-lg">
      <table className="min-w-full divide-y divide-gray-200">
        <thead className="bg-gray-100">
          <tr>
            {columns.map((column) => (
              <th
                key={column.key}
                className="px-6 py-3 text-left text-xs font-semibold text-gray-900 uppercase tracking-wider"
                style={{ width: column.width }}
              >
                {column.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="bg-white divide-y divide-gray-200">
          {data.map((item) => (
            <tr
              key={item.id}
              onClick={() => onRowClick?.(item)}
              className={onRowClick ? 'hover:bg-gray-50 cursor-pointer transition-colors' : ''}
            >
              {columns.map((column) => (
                <td key={column.key} className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {column.render ? column.render(item) : (item as any)[column.key]}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
```

---

## 8. Layout

### 8.1 Layout Principal

```typescript
// src/components/layout/Layout.tsx
import React from 'react';
import { Outlet } from 'react-router-dom';
import { Header } from './Header/Header';
import { Sidebar } from './Sidebar/Sidebar';
import { Footer } from './Footer/Footer';

export const Layout: React.FC = () => {
  return (
    <div className="min-h-screen bg-gray-50 flex flex-col">
      <Header />
      <div className="flex flex-1">
        <Sidebar />
        <main className="flex-1 p-6 overflow-auto">
          <Outlet />
        </main>
      </div>
      <Footer />
    </div>
  );
};
```

### 8.2 Header

```typescript
// src/components/layout/Header/Header.tsx
import React from 'react';
import { LogOut, User } from 'lucide-react';
import { authStore } from '../../../store/auth.store';
import { Button } from '../../common/Button/Button';

export const Header: React.FC = () => {
  const { user, logout } = authStore();

  const handleLogout = async () => {
    try {
      await authApi.logout();
    } finally {
      logout();
      window.location.href = '/login';
    }
  };

  return (
    <header className="bg-black text-white border-b-2 border-gray-800">
      <div className="container mx-auto px-6 py-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-4">
            <h1 className="text-2xl font-bold">Sistema de Rede</h1>
          </div>
          
          <div className="flex items-center space-x-4">
            <div className="flex items-center space-x-2 text-sm">
              <User className="h-4 w-4" />
              <span>{user?.name}</span>
              <span className="text-gray-400">|</span>
              <span className="text-gray-300">{user?.permission}</span>
            </div>
            <Button
              variant="ghost"
              size="sm"
              icon={<LogOut className="h-4 w-4" />}
              onClick={handleLogout}
              className="text-white hover:bg-gray-800"
            >
              Sair
            </Button>
          </div>
        </div>
      </div>
    </header>
  );
};
```

### 8.3 Sidebar

```typescript
// src/components/layout/Sidebar/Sidebar.tsx
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
import { authStore } from '../../../store/auth.store';
import { Permission } from '../../../types/auth.types';

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
    <aside className="w-64 bg-white border-r border-gray-200">
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
```

---

## 9. Rotas e Proteção

### 9.1 Router Principal

```typescript
// src/routes/index.tsx
import React from 'react';
import { createBrowserRouter, Navigate } from 'react-router-dom';
import { Layout } from '../components/layout/Layout';
import { PrivateRoute } from './PrivateRoute';
import { PermissionRoute } from './PermissionRoute';
import { Permission } from '../types/auth.types';

// Pages
import Login from '../pages/Login';
import Dashboard from '../pages/Dashboard';
import UsersPage from '../pages/Users/UsersPage';
import VectorsPage from '../pages/Vectors/VectorsPage';
import PartnersPage from '../pages/Partners/PartnersPage';
import BusinessTypesPage from '../pages/BusinessTypes/BusinessTypesPage';
import BusinessPage from '../pages/Business/BusinessPage';
import PaymentsPage from '../pages/Payments/PaymentsPage';
import ReportsPage from '../pages/Reports/ReportsPage';
import AuditPage from '../pages/Audit/AuditPage';
import NotFound from '../pages/NotFound';

export const router = createBrowserRouter([
  {
    path: '/login',
    element: <Login />,
  },
  {
    path: '/',
    element: (
      <PrivateRoute>
        <Layout />
      </PrivateRoute>
    ),
    children: [
      {
        index: true,
        element: <Navigate to="/dashboard" replace />,
      },
      {
        path: 'dashboard',
        element: <Dashboard />,
      },
      {
        path: 'users',
        element: (
          <PermissionRoute
            requiredPermissions={[Permission.AdminGlobal, Permission.AdminVetor]}
          >
            <UsersPage />
          </PermissionRoute>
        ),
      },
      {
        path: 'vectors',
        element: (
          <PermissionRoute requiredPermissions={[Permission.AdminGlobal]}>
            <VectorsPage />
          </PermissionRoute>
        ),
      },
      {
        path: 'partners',
        element: <PartnersPage />,
      },
      {
        path: 'business-types',
        element: (
          <PermissionRoute
            requiredPermissions={[Permission.AdminGlobal, Permission.AdminVetor]}
          >
            <BusinessTypesPage />
          </PermissionRoute>
        ),
      },
      {
        path: 'business',
        element: <BusinessPage />,
      },
      {
        path: 'payments',
        element: <PaymentsPage />,
      },
      {
        path: 'reports',
        element: <ReportsPage />,
      },
      {
        path: 'audit',
        element: (
          <PermissionRoute requiredPermissions={[Permission.AdminGlobal]}>
            <AuditPage />
          </PermissionRoute>
        ),
      },
    ],
  },
  {
    path: '*',
    element: <NotFound />,
  },
]);
```

### 9.2 PrivateRoute

```typescript
// src/routes/PrivateRoute.tsx
import React from 'react';
import { Navigate } from 'react-router-dom';
import { authStore } from '../store/auth.store';

interface PrivateRouteProps {
  children: React.ReactNode;
}

export const PrivateRoute: React.FC<PrivateRouteProps> = ({ children }) => {
  const { isAuthenticated } = authStore();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};
```

### 9.3 PermissionRoute

```typescript
// src/routes/PermissionRoute.tsx
import React from 'react';
import { Navigate } from 'react-router-dom';
import { authStore } from '../store/auth.store';
import { Permission } from '../types/auth.types';

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
```

---

## 10. Páginas Principais

### 10.1 Login Page

```typescript
// src/pages/Login.tsx
import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation } from '@tanstack/react-query';
import { authApi } from '../api/endpoints/auth.api';
import { authStore } from '../store/auth.store';
import { Input } from '../components/common/Input/Input';
import { Button } from '../components/common/Button/Button';
import { LogIn } from 'lucide-react';

const loginSchema = z.object({
  email: z.string().email('Email inválido'),
  password: z.string().min(6, 'Senha deve ter no mínimo 6 caracteres'),
});

type LoginForm = z.infer<typeof loginSchema>;

const Login: React.FC = () => {
  const navigate = useNavigate();
  const { setTokens, setUser } = authStore();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginForm>({
    resolver: zodResolver(loginSchema),
  });

  const loginMutation = useMutation({
    mutationFn: authApi.login,
    onSuccess: (data) => {
      setTokens(data.token, data.refreshToken);
      setUser(data.user);
      navigate('/dashboard');
    },
  });

  const onSubmit = (data: LoginForm) => {
    loginMutation.mutate(data);
  };

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        <div className="bg-white rounded-lg shadow-xl border-2 border-black p-8">
          <div className="text-center mb-8">
            <h1 className="text-3xl font-bold text-black">Sistema de Rede</h1>
            <p className="text-gray-600 mt-2">
              Gestão de Credenciamento e Comissões
            </p>
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            <Input
              label="Email"
              type="email"
              placeholder="seu@email.com"
              error={errors.email?.message}
              {...register('email')}
            />

            <Input
              label="Senha"
              type="password"
              placeholder="••••••••"
              error={errors.password?.message}
              {...register('password')}
            />

            {loginMutation.isError && (
              <div className="p-3 bg-gray-100 border-l-4 border-gray-700 text-gray-900 rounded">
                Credenciais inválidas
              </div>
            )}

            <Button
              type="submit"
              variant="primary"
              size="lg"
              className="w-full"
              isLoading={loginMutation.isPending}
              icon={<LogIn className="h-5 w-5" />}
            >
              Entrar
            </Button>
          </form>
        </div>
      </div>
    </div>
  );
};

export default Login;
```

### 10.2 Dashboard Page

```typescript
// src/pages/Dashboard.tsx
import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { reportsApi } from '../api/endpoints/reports.api';
import { authStore } from '../store/auth.store';
import {
  Users,
  UserCheck,
  FileText,
  DollarSign,
  TrendingUp,
} from 'lucide-react';

const Dashboard: React.FC = () => {
  const { user } = authStore();

  const { data: financialReport, isLoading } = useQuery({
    queryKey: ['financial-report', user?.vectorId],
    queryFn: () => reportsApi.financial({
      vectorId: user?.vectorId,
    }),
  });

  const stats = [
    {
      label: 'Total Pago',
      value: financialReport?.totalPaid ?? 0,
      icon: <DollarSign className="h-8 w-8" />,
      color: 'bg-black',
    },
    {
      label: 'Total Pendente',
      value: financialReport?.totalPending ?? 0,
      icon: <TrendingUp className="h-8 w-8" />,
      color: 'bg-gray-700',
    },
    {
      label: 'Total Negócios',
      value: financialReport?.totalBusiness ?? 0,
      icon: <FileText className="h-8 w-8" />,
      color: 'bg-gray-600',
    },
    {
      label: 'Total Parceiros',
      value: financialReport?.totalPartners ?? 0,
      icon: <UserCheck className="h-8 w-8" />,
      color: 'bg-gray-500',
    },
  ];

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-screen">
        <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-black"></div>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-black">Dashboard</h1>
        <p className="text-gray-600 mt-1">
          Bem-vindo, {user?.name}
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat, index) => (
          <div
            key={index}
            className="bg-white rounded-lg shadow border border-gray-200 p-6"
          >
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600 font-medium">
                  {stat.label}
                </p>
                <p className="text-2xl font-bold text-black mt-2">
                  {typeof stat.value === 'number' && stat.label.includes('Total')
                    ? stat.value.toLocaleString('pt-BR', {
                        style: 'currency',
                        currency: 'BRL',
                      })
                    : stat.value}
                </p>
              </div>
              <div className={`${stat.color} p-3 rounded-lg text-white`}>
                {stat.icon}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Dashboard;
```

---

## 11. Hooks Customizados

### 11.1 useAuth Hook

```typescript
// src/hooks/useAuth.ts
import { authStore } from '../store/auth.store';

export const useAuth = () => {
  const { user, isAuthenticated, hasPermission, hasVectorAccess, logout } = authStore();

  return {
    user,
    isAuthenticated,
    hasPermission,
    hasVectorAccess,
    logout,
  };
};
```

### 11.2 useDebounce Hook

```typescript
// src/hooks/useDebounce.ts
import { useEffect, useState } from 'react';

export function useDebounce<T>(value: T, delay: number = 500): T {
  const [debouncedValue, setDebouncedValue] = useState<T>(value);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(handler);
    };
  }, [value, delay]);

  return debouncedValue;
}
```

---

## 12. Utilidades

### 12.1 Formatadores

```typescript
// src/utils/formatters.ts

export const formatCurrency = (value: number): string => {
  return value.toLocaleString('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  });
};

export const formatDate = (date: string | Date): string => {
  return new Date(date).toLocaleDateString('pt-BR');
};

export const formatDateTime = (date: string | Date): string => {
  return new Date(date).toLocaleString('pt-BR');
};

export const formatPercentage = (value: number): string => {
  return `${(value * 100).toFixed(2)}%`;
};
```

### 12.2 Constantes

```typescript
// src/utils/constants.ts

export const PERMISSION_LABELS = {
  AdminGlobal: 'Admin Global',
  AdminVetor: 'Admin de Vetor',
  Operador: 'Operador',
  Parceiro: 'Parceiro',
};

export const BUSINESS_STATUS_LABELS = {
  Active: 'Ativo',
  Cancelled: 'Cancelado',
};

export const PAYMENT_STATUS_LABELS = {
  Pending: 'Pendente',
  Paid: 'Pago',
};

export const COMMISSION_LEVELS = {
  1: 'Nível 1',
  2: 'Nível 2',
  3: 'Nível 3',
};

export const PAGE_SIZE_OPTIONS = [10, 25, 50, 100];
```

---

## 13. Guia de Estilização (Tema Preto e Branco)

### 13.1 Princípios de Design

1. **Contraste Alto**: Use preto puro (#000000) e branco puro (#FFFFFF) para elementos principais
2. **Hierarquia Visual**: Use tons de cinza (gray-100 a gray-900) para criar profundidade
3. **Espaçamento**: Mantenha espaçamento generoso para melhor legibilidade
4. **Tipografia**: Use fonte sans-serif limpa (Inter ou System UI)
5. **Bordas**: Use bordas pretas (2px) para elementos importantes

### 13.2 Componentes de UI

```css
/* Cards */
.card {
  @apply bg-white rounded-lg shadow border border-gray-200 p-6;
}

/* Botões */
.btn-primary {
  @apply bg-black text-white hover:bg-gray-800 border-2 border-black;
}

.btn-secondary {
  @apply bg-white text-black hover:bg-gray-100 border-2 border-black;
}

/* Inputs */
.input {
  @apply border-2 border-gray-300 focus:border-black focus:ring-2 focus:ring-black;
}

/* Tables */
.table-header {
  @apply bg-gray-100 text-black font-semibold border-b-2 border-black;
}

.table-row {
  @apply hover:bg-gray-50 border-b border-gray-200;
}

/* Status Badges */
.badge-active {
  @apply bg-black text-white px-2 py-1 rounded text-xs font-medium;
}

.badge-inactive {
  @apply bg-gray-300 text-gray-700 px-2 py-1 rounded text-xs font-medium;
}
```

---

## 14. Testes

### 14.1 Configurar Testing Library

```bash
npm install -D @testing-library/react @testing-library/jest-dom @testing-library/user-event vitest jsdom
```

### 14.2 Exemplo de Teste - Button

```typescript
// src/components/common/Button/Button.test.tsx
import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { Button } from './Button';

describe('Button', () => {
  it('renderiza o botão com texto', () => {
    render(<Button>Clique aqui</Button>);
    expect(screen.getByText('Clique aqui')).toBeInTheDocument();
  });

  it('chama onClick quando clicado', () => {
    const handleClick = vi.fn();
    render(<Button onClick={handleClick}>Clique</Button>);
    
    fireEvent.click(screen.getByText('Clique'));
    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it('mostra loading quando isLoading é true', () => {
    render(<Button isLoading>Enviar</Button>);
    expect(screen.getByText('Carregando...')).toBeInTheDocument();
  });

  it('desabilita quando disabled é true', () => {
    render(<Button disabled>Botão</Button>);
    expect(screen.getByRole('button')).toBeDisabled();
  });
});
```

---

## 15. Build e Deploy

### 15.1 Build para Produção

```bash
npm run build
```

### 15.2 Preview da Build

```bash
npm run preview
```

### 15.3 Variáveis de Ambiente para Produção

```env
VITE_API_BASE_URL=https://api.production.com/api
VITE_APP_NAME=Sistema de Rede de Credenciamento
```

### 15.4 Docker

**Dockerfile:**

```dockerfile
FROM node:18-alpine as build

WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

FROM nginx:alpine

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
```

**nginx.conf:**

```nginx
server {
    listen 80;
    server_name localhost;

    root /usr/share/nginx/html;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api {
        proxy_pass http://backend:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

---

## 16. Checklist de Implementação

### Fase 1: Setup Inicial
- [ ] Criar projeto com Vite
- [ ] Instalar dependências
- [ ] Configurar Tailwind CSS (tema preto e branco)
- [ ] Configurar TypeScript
- [ ] Criar estrutura de pastas

### Fase 2: Configuração Base
- [ ] Configurar Axios e interceptors
- [ ] Implementar tipos TypeScript
- [ ] Configurar React Query
- [ ] Configurar Zustand (auth store)
- [ ] Configurar rotas e proteção

### Fase 3: Componentes Comuns
- [ ] Criar componentes de UI (Button, Input, Select, Table, Modal)
- [ ] Criar componentes de layout (Header, Sidebar, Footer)
- [ ] Criar componentes de feedback (Loading, Alert, Toast)

### Fase 4: Autenticação
- [ ] Página de login
- [ ] Sistema de refresh token
- [ ] Proteção de rotas
- [ ] Controle de permissões

### Fase 5: Features - Admin Global
- [ ] CRUD de Vetores
- [ ] CRUD de Usuários (todos os perfis)
- [ ] Visualização de auditoria

### Fase 6: Features - Admin Vetor/Operador
- [ ] CRUD de Parceiros
- [ ] CRUD de Tipos de Negócio
- [ ] CRUD de Negócios
- [ ] Gestão de Pagamentos
- [ ] Visualização de árvore de parceiros

### Fase 7: Relatórios
- [ ] Relatório de Parceiros
- [ ] Relatório Financeiro
- [ ] Relatório de Negócios
- [ ] Filtros e exportação

### Fase 8: Refinamentos
- [ ] Validações de formulários
- [ ] Mensagens de erro
- [ ] Feedback visual
- [ ] Loading states
- [ ] Responsive design

### Fase 9: Testes
- [ ] Testes unitários dos componentes
- [ ] Testes de integração
- [ ] Testes E2E (Cypress/Playwright)

### Fase 10: Deploy
- [ ] Build de produção
- [ ] Configurar Docker
- [ ] CI/CD pipeline
- [ ] Documentação

---

## 17. Boas Práticas

### 17.1 Código

1. **TypeScript**: Sempre use tipos explícitos
2. **Naming**: Use nomes descritivos e em inglês
3. **Componentes**: Mantenha componentes pequenos e focados
4. **Hooks**: Crie hooks customizados para lógica reutilizável
5. **Estado**: Use React Query para estado do servidor, Zustand para estado global

### 17.2 Performance

1. **Code Splitting**: Use lazy loading para rotas
2. **Memoization**: Use React.memo, useMemo, useCallback quando apropriado
3. **Paginação**: Sempre pagine listas grandes
4. **Debounce**: Use debounce para buscas
5. **Imagens**: Otimize e use lazy loading

### 17.3 Segurança

1. **XSS**: Sanitize user input
2. **CSRF**: Use tokens CSRF se necessário
3. **JWT**: Armazene tokens de forma segura
4. **HTTPS**: Use apenas em produção
5. **Validação**: Valide dados no frontend e backend

### 17.4 Acessibilidade

1. **Semântica**: Use HTML semântico
2. **ARIA**: Adicione labels ARIA quando necessário
3. **Keyboard**: Suporte navegação por teclado
4. **Contraste**: Mantenha contraste adequado (tema preto e branco ajuda)
5. **Screen readers**: Teste com leitores de tela

---

## 18. Troubleshooting

### 18.1 Problemas Comuns

**Erro de CORS:**
```typescript
// Certifique-se de que o backend permite requisições do frontend
// No backend .NET, adicione no Program.cs:
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
```

**Token expirado:**
- Verifique se o interceptor de refresh token está funcionando
- Confirme que o backend está retornando o refresh token corretamente

**Problema de build:**
- Limpe cache: `rm -rf node_modules package-lock.json && npm install`
- Verifique versões das dependências

---

## 19. Recursos Adicionais

### 19.1 Documentação Oficial

- [React](https://react.dev)
- [TypeScript](https://www.typescriptlang.org)
- [Vite](https://vitejs.dev)
- [React Router](https://reactrouter.com)
- [TanStack Query](https://tanstack.com/query)
- [Zustand](https://github.com/pmndrs/zustand)
- [Tailwind CSS](https://tailwindcss.com)

### 19.2 Ferramentas Úteis

- **DevTools**: React DevTools, Redux DevTools
- **Testing**: Vitest, Testing Library, Playwright
- **Linting**: ESLint, Prettier
- **Type Checking**: TypeScript Compiler

---

## 20. Conclusão

Esta documentação fornece uma base sólida para implementar o frontend React do Sistema de Rede de Credenciamento. O tema preto e branco oferece uma interface limpa, profissional e de alto contraste.

### Próximos Passos:

1. Configure o projeto seguindo a Fase 1 do checklist
2. Implemente a autenticação e rotas protegidas
3. Desenvolva os componentes comuns reutilizáveis
4. Implemente cada módulo funcional (Vetores, Parceiros, Negócios, etc.)
5. Adicione testes conforme desenvolve
6. Prepare para deploy

### Suporte:

Para questões específicas sobre o backend ou regras de negócio, consulte o arquivo [Projeto.md](Projeto.md).
