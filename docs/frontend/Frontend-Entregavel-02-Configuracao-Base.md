# Entregável 02 - Configuração Base

## 📋 Informações Gerais

- **Duração estimada:** 2-3 dias
- **Prioridade:** Crítica
- **Dependências:** Entregável 01
- **Desenvolvedor(es):** 1 desenvolvedor

---

## 🎯 Objetivo

Implementar a infraestrutura base de comunicação com API, gerenciamento de estado e tipagem TypeScript completa do sistema.

---

## 📦 Entregáveis

### 1. Tipos TypeScript
- [ ] `auth.types.ts` completo
- [ ] `user.types.ts` completo
- [ ] `vector.types.ts` completo
- [ ] `partner.types.ts` completo
- [ ] `business.types.ts` completo
- [ ] `payment.types.ts` completo
- [ ] `report.types.ts` completo
- [ ] `common.types.ts` completo

### 2. Configuração Axios
- [ ] `axios.config.ts` implementado
- [ ] Interceptor de request (token)
- [ ] Interceptor de response (refresh token)
- [ ] Tratamento de erros

### 3. API Endpoints
- [ ] `auth.api.ts` completo
- [ ] `users.api.ts` completo
- [ ] `vectors.api.ts` completo
- [ ] `partners.api.ts` completo
- [ ] `businessTypes.api.ts` completo
- [ ] `business.api.ts` completo
- [ ] `payments.api.ts` completo
- [ ] `reports.api.ts` completo
- [ ] `audit.api.ts` completo

### 4. Estado Global (Zustand)
- [ ] `auth.store.ts` implementado
- [ ] Persistência de autenticação
- [ ] Funções de verificação de permissão

### 5. React Query
- [ ] Provider configurado
- [ ] Query client com configurações
- [ ] DevTools habilitado (dev)

### 6. Rotas Base
- [ ] Router configurado
- [ ] PrivateRoute component
- [ ] PermissionRoute component
- [ ] Estrutura de rotas definida

---

## 🔧 Tarefas Detalhadas

### Tarefa 2.1 - Criar Tipos TypeScript - OK

**Arquivo: `src/types/auth.types.ts`**

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
  updatedAt?: string;
}
```

**Arquivo: `src/types/vector.types.ts`**

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

**Arquivo: `src/types/partner.types.ts`**

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
  updatedAt?: string;
}

export interface CreatePartnerRequest {
  name: string;
  contact: string;
  recommenderId?: string;
  recommenderType?: 'Partner' | 'Vector';
  vectorId: string;
}

export interface UpdatePartnerRequest {
  name: string;
  contact: string;
  isActive: boolean;
}

export interface PartnerTree {
  id: string;
  name: string;
  level: number;
  totalRecommended: number;
  children: PartnerTree[];
}
```

**Arquivo: `src/types/business.types.ts`**

```typescript
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
```

**Arquivo: `src/types/payment.types.ts`**

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

**Arquivo: `src/types/report.types.ts`**

```typescript
export interface PartnerReport {
  partnerId: string;
  partnerName: string;
  level: number;
  totalRecommended: number;
  totalEarned: number;
  totalPending: number;
  activeRecommendations: number;
}

export interface FinancialReport {
  totalPaid: number;
  totalPending: number;
  totalBusiness: number;
  totalPartners: number;
  paymentsByLevel: {
    level: number;
    total: number;
  }[];
  paymentsByMonth: {
    month: string;
    total: number;
  }[];
}

export interface BusinessReport {
  businessId: string;
  partnerName: string;
  businessTypeName: string;
  value: number;
  totalCommission: number;
  date: string;
  status: string;
  paymentsPending: number;
  paymentsPaid: number;
}
```

**Arquivo: `src/types/common.types.ts`**

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
  statusCode?: number;
}

export interface SelectOption {
  value: string;
  label: string;
  disabled?: boolean;
}

export interface FilterParams {
  page?: number;
  pageSize?: number;
  search?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}
```

**Criar arquivo índice `src/types/index.ts`:**

```typescript
export * from './auth.types';
export * from './user.types';
export * from './vector.types';
export * from './partner.types';
export * from './business.types';
export * from './payment.types';
export * from './report.types';
export * from './common.types';
```

---

### Tarefa 2.2 - Configurar Axios - OK

**Arquivo: `src/api/axios.config.ts`**

```typescript
import axios, { AxiosError } from 'axios';
import { authStore } from '@/store/auth.store';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
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

// Interceptor para tratar erros e refresh token
api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as any;

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

---

### Tarefa 2.3 - Criar Endpoints da API - OK 

**Arquivo: `src/api/endpoints/auth.api.ts`**

```typescript
import api from '../axios.config';
import {
  LoginRequest,
  LoginResponse,
  RefreshTokenRequest,
  ChangePasswordRequest,
} from '@/types';

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

**Arquivo: `src/api/endpoints/users.api.ts`**

```typescript
import api from '../axios.config';
import { User, PaginatedResponse, FilterParams } from '@/types';

export const usersApi = {
  list: async (params?: FilterParams): Promise<PaginatedResponse<User>> => {
    const response = await api.get('/users', { params });
    return response.data;
  },

  getById: async (id: string): Promise<User> => {
    const response = await api.get(`/users/${id}`);
    return response.data;
  },

  create: async (data: Partial<User>): Promise<User> => {
    const response = await api.post('/users', data);
    return response.data;
  },

  update: async (id: string, data: Partial<User>): Promise<User> => {
    const response = await api.put(`/users/${id}`, data);
    return response.data;
  },

  toggleActive: async (id: string): Promise<void> => {
    await api.patch(`/users/${id}/toggle-active`);
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/users/${id}`);
  },
};
```

Continue criando os demais arquivos de API seguindo o mesmo padrão...

---

### Tarefa 2.4 - Implementar Auth Store - OK

**Arquivo: `src/store/auth.store.ts`**

```typescript
import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { User, Permission } from '@/types';

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
      partialize: (state) => ({
        token: state.token,
        refreshToken: state.refreshToken,
        user: state.user,
        isAuthenticated: state.isAuthenticated,
      }),
    }
  )
);
```

---

### Tarefa 2.5 - Configurar React Query - OK

**Arquivo: `src/App.tsx` atualizado:**

```typescript
import React from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
      staleTime: 5 * 60 * 1000, // 5 minutos
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <div className="min-h-screen bg-gray-50">
        {/* Conteúdo temporário */}
        <div className="flex items-center justify-center h-screen">
          <h1 className="text-3xl font-bold">
            Configuração Base Completa ✓
          </h1>
        </div>
      </div>
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  );
}

export default App;
```

---

### Tarefa 2.6 - Criar Rotas Base

**Arquivo: `src/routes/PrivateRoute.tsx`**

```typescript
import React from 'react';
import { Navigate } from 'react-router-dom';
import { authStore } from '@/store/auth.store';

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

**Arquivo: `src/routes/PermissionRoute.tsx`**

```typescript
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
```

---

## ✅ Critérios de Aceitação

- [ ] Todos os tipos TypeScript criados e sem erros
- [ ] Axios configurado com interceptors funcionando
- [ ] Todos os endpoints da API implementados
- [ ] Auth store com persistência funcionando
- [ ] React Query configurado e DevTools acessível
- [ ] Rotas de proteção implementadas
- [ ] Imports funcionando com path aliases
- [ ] Nenhum erro no TypeScript compiler

---

## 🧪 Testes de Validação

1. **Teste de Tipos:**
   ```bash
   npx tsc --noEmit
   # Não deve ter erros
   ```

2. **Teste de Import:**
   Criar arquivo de teste e importar todos os tipos:
   ```typescript
   import * as types from '@/types';
   console.log(Object.keys(types));
   ```

3. **Teste do Auth Store:**
   ```typescript
   import { authStore } from '@/store/auth.store';
   const { setUser, hasPermission } = authStore.getState();
   // Deve funcionar sem erros
   ```

---

## 📝 Notas Importantes

- Todos os tipos devem estar alinhados com o backend .NET
- Use tipos estritos (evite `any`)
- Mantenha consistência nos nomes
- Documente funções complexas

---

## 🔄 Próximo Entregável

**[Entregável 03 - Componentes Comuns](./Frontend-Entregavel-03-Componentes-Comuns.md)**
