# Entregável 03 - Componentes Comuns

## 📋 Informações Gerais

- **Duração estimada:** 3-4 dias
- **Prioridade:** Alta
- **Dependências:** Entregável 01, 02
- **Desenvolvedor(es):** 1-2 desenvolvedores

---

## 🎯 Objetivo

Criar uma biblioteca completa de componentes reutilizáveis com tema preto e branco, garantindo consistência visual e experiência de usuário padronizada em toda a aplicação.

---

## 📦 Entregáveis

### 1. Componentes de Formulário
- [ ] Button component
- [ ] Input component
- [ ] Select component
- [ ] Textarea component
- [ ] Checkbox component
- [ ] Radio component

### 2. Componentes de Dados
- [ ] Table component
- [ ] Pagination component
- [ ] Badge component
- [ ] Card component

### 3. Componentes de Feedback
- [ ] Loading/Spinner component
- [ ] Alert component
- [ ] Toast component
- [ ] Modal component
- [ ] ConfirmDialog component

### 4. Layout Components
- [ ] Header component
- [ ] Sidebar component
- [ ] Footer component
- [ ] Layout wrapper component

### 5. Utilitários
- [ ] Empty State component
- [ ] Error Boundary component

---

## 🔧 Tarefas Detalhadas

### Tarefa 3.1 - Button Component - OK

**Arquivo: `src/components/common/Button/Button.tsx`**

```typescript
import React from 'react';
import { Loader2 } from 'lucide-react';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger';
  size?: 'sm' | 'md' | 'lg';
  isLoading?: boolean;
  icon?: React.ReactNode;
  fullWidth?: boolean;
}

export const Button: React.FC<ButtonProps> = ({
  children,
  variant = 'primary',
  size = 'md',
  isLoading = false,
  icon,
  fullWidth = false,
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

  const widthClass = fullWidth ? 'w-full' : '';

  return (
    <button
      className={`${baseStyles} ${variants[variant]} ${sizes[size]} ${widthClass} ${className}`}
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

**Arquivo: `src/components/common/Button/Button.test.tsx`**

```typescript
import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { Button } from './Button';
import { Plus } from 'lucide-react';

describe('Button', () => {
  it('renderiza com texto', () => {
    render(<Button>Click me</Button>);
    expect(screen.getByText('Click me')).toBeInTheDocument();
  });

  it('executa onClick', () => {
    const onClick = vi.fn();
    render(<Button onClick={onClick}>Click</Button>);
    fireEvent.click(screen.getByText('Click'));
    expect(onClick).toHaveBeenCalledTimes(1);
  });

  it('mostra loading', () => {
    render(<Button isLoading>Submit</Button>);
    expect(screen.getByText('Carregando...')).toBeInTheDocument();
  });

  it('renderiza com ícone', () => {
    render(<Button icon={<Plus />}>Adicionar</Button>);
    expect(screen.getByText('Adicionar')).toBeInTheDocument();
  });
});
```

**Arquivo: `src/components/common/Button/index.ts`**

```typescript
export { Button } from './Button';
```

---

### Tarefa 3.2 - Input Component - OK

**Arquivo: `src/components/common/Input/Input.tsx`**

```typescript
import React, { forwardRef } from 'react';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  helperText?: string;
  icon?: React.ReactNode;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ label, error, helperText, icon, className = '', ...props }, ref) => {
    return (
      <div className="w-full">
        {label && (
          <label className="block text-sm font-medium text-gray-900 mb-1">
            {label}
            {props.required && <span className="text-gray-700 ml-1">*</span>}
          </label>
        )}
        <div className="relative">
          {icon && (
            <div className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400">
              {icon}
            </div>
          )}
          <input
            ref={ref}
            className={`
              w-full px-3 py-2 
              ${icon ? 'pl-10' : ''}
              border-2 border-gray-300 rounded-md
              text-gray-900 placeholder-gray-400
              focus:outline-none focus:ring-2 focus:ring-black focus:border-black
              disabled:bg-gray-100 disabled:cursor-not-allowed
              ${error ? 'border-gray-700 focus:border-gray-700 focus:ring-gray-700' : ''}
              ${className}
            `}
            {...props}
          />
        </div>
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

---

### Tarefa 3.3 - Select Component - OK

**Arquivo: `src/components/common/Select/Select.tsx`**

```typescript
import React, { forwardRef } from 'react';
import { SelectOption } from '@/types';

interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  error?: string;
  helperText?: string;
  options: SelectOption[];
  placeholder?: string;
}

export const Select = forwardRef<HTMLSelectElement, SelectProps>(
  ({ label, error, helperText, options, placeholder, className = '', ...props }, ref) => {
    return (
      <div className="w-full">
        {label && (
          <label className="block text-sm font-medium text-gray-900 mb-1">
            {label}
            {props.required && <span className="text-gray-700 ml-1">*</span>}
          </label>
        )}
        <select
          ref={ref}
          className={`
            w-full px-3 py-2 
            border-2 border-gray-300 rounded-md
            text-gray-900
            focus:outline-none focus:ring-2 focus:ring-black focus:border-black
            disabled:bg-gray-100 disabled:cursor-not-allowed
            ${error ? 'border-gray-700' : ''}
            ${className}
          `}
          {...props}
        >
          {placeholder && (
            <option value="">{placeholder}</option>
          )}
          {options.map((option) => (
            <option 
              key={option.value} 
              value={option.value}
              disabled={option.disabled}
            >
              {option.label}
            </option>
          ))}
        </select>
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

Select.displayName = 'Select';
```

---

### Tarefa 3.4 - Table Component - OK

**Arquivo: `src/components/common/Table/Table.tsx`**

```typescript
import React from 'react';

export interface Column<T> {
  key: string;
  header: string;
  render?: (item: T) => React.ReactNode;
  width?: string;
  align?: 'left' | 'center' | 'right';
}

interface TableProps<T> {
  data: T[];
  columns: Column<T>[];
  onRowClick?: (item: T) => void;
  isLoading?: boolean;
  emptyMessage?: string;
  keyExtractor?: (item: T) => string;
}

export function Table<T>({
  data,
  columns,
  onRowClick,
  isLoading,
  emptyMessage = 'Nenhum registro encontrado',
  keyExtractor,
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
      <div className="text-center py-12 text-gray-500 bg-white rounded-lg border border-gray-200">
        {emptyMessage}
      </div>
    );
  }

  const getAlignClass = (align?: 'left' | 'center' | 'right') => {
    switch (align) {
      case 'center': return 'text-center';
      case 'right': return 'text-right';
      default: return 'text-left';
    }
  };

  return (
    <div className="overflow-x-auto border border-gray-200 rounded-lg">
      <table className="min-w-full divide-y divide-gray-200">
        <thead className="bg-gray-100">
          <tr>
            {columns.map((column) => (
              <th
                key={column.key}
                className={`px-6 py-3 text-xs font-semibold text-gray-900 uppercase tracking-wider ${getAlignClass(column.align)}`}
                style={{ width: column.width }}
              >
                {column.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="bg-white divide-y divide-gray-200">
          {data.map((item, index) => {
            const key = keyExtractor ? keyExtractor(item) : `row-${index}`;
            return (
              <tr
                key={key}
                onClick={() => onRowClick?.(item)}
                className={onRowClick ? 'hover:bg-gray-50 cursor-pointer transition-colors' : ''}
              >
                {columns.map((column) => (
                  <td 
                    key={column.key} 
                    className={`px-6 py-4 text-sm text-gray-900 ${getAlignClass(column.align)}`}
                  >
                    {column.render ? column.render(item) : String((item as any)[column.key] ?? '-')}
                  </td>
                ))}
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}
```

---

### Tarefa 3.5 - Modal Component

**Arquivo: `src/components/common/Modal/Modal.tsx`**

```typescript
import React, { useEffect } from 'react';
import { X } from 'lucide-react';
import { Button } from '../Button';

interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  children: React.ReactNode;
  footer?: React.ReactNode;
  size?: 'sm' | 'md' | 'lg' | 'xl';
}

export const Modal: React.FC<ModalProps> = ({
  isOpen,
  onClose,
  title,
  children,
  footer,
  size = 'md',
}) => {
  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = 'unset';
    }
    return () => {
      document.body.style.overflow = 'unset';
    };
  }, [isOpen]);

  if (!isOpen) return null;

  const sizes = {
    sm: 'max-w-md',
    md: 'max-w-lg',
    lg: 'max-w-2xl',
    xl: 'max-w-4xl',
  };

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      {/* Backdrop */}
      <div 
        className="fixed inset-0 bg-black bg-opacity-50 transition-opacity"
        onClick={onClose}
      />
      
      {/* Modal */}
      <div className="flex min-h-full items-center justify-center p-4">
        <div 
          className={`relative bg-white rounded-lg shadow-xl border-2 border-black ${sizes[size]} w-full`}
          onClick={(e) => e.stopPropagation()}
        >
          {/* Header */}
          <div className="flex items-center justify-between p-6 border-b-2 border-gray-200">
            <h3 className="text-xl font-semibold text-gray-900">{title}</h3>
            <button
              onClick={onClose}
              className="text-gray-400 hover:text-gray-600 transition-colors"
            >
              <X className="h-6 w-6" />
            </button>
          </div>

          {/* Body */}
          <div className="p-6">
            {children}
          </div>

          {/* Footer */}
          {footer && (
            <div className="flex items-center justify-end gap-3 p-6 border-t-2 border-gray-200">
              {footer}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
```

---

### Tarefa 3.6 - Layout Components

**Arquivo: `src/components/layout/Header/Header.tsx`**

```typescript
import React from 'react';
import { LogOut, User } from 'lucide-react';
import { authStore } from '@/store/auth.store';
import { Button } from '@/components/common/Button';
import { authApi } from '@/api/endpoints/auth.api';

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

**Arquivo: `src/components/layout/Sidebar/Sidebar.tsx`**

```typescript
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
```

**Arquivo: `src/components/layout/Layout.tsx`**

```typescript
import React from 'react';
import { Outlet } from 'react-router-dom';
import { Header } from './Header/Header';
import { Sidebar } from './Sidebar/Sidebar';

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
    </div>
  );
};
```

---

## ✅ Critérios de Aceitação

- [ ] Todos os componentes implementados e funcionais
- [ ] Tema preto e branco aplicado consistentemente
- [ ] Componentes responsivos
- [ ] Props tipadas com TypeScript
- [ ] Testes básicos criados
- [ ] Documentação JSDoc nos componentes principais
- [ ] Acessibilidade básica (aria-labels, keyboard navigation)
- [ ] Storybook ou página de demonstração criada (opcional)

---

## 🧪 Testes de Validação

Criar página de teste: `src/pages/ComponentsShowcase.tsx`

```typescript
import React from 'react';
import { Button } from '@/components/common/Button';
import { Input } from '@/components/common/Input';
import { Plus, Search } from 'lucide-react';

export const ComponentsShowcase: React.FC = () => {
  return (
    <div className="p-8 space-y-8">
      <section>
        <h2 className="text-2xl font-bold mb-4">Buttons</h2>
        <div className="flex gap-4">
          <Button variant="primary">Primary</Button>
          <Button variant="secondary">Secondary</Button>
          <Button variant="outline">Outline</Button>
          <Button variant="ghost">Ghost</Button>
          <Button variant="danger">Danger</Button>
        </div>
      </section>

      <section>
        <h2 className="text-2xl font-bold mb-4">Inputs</h2>
        <div className="max-w-md space-y-4">
          <Input label="Nome" placeholder="Digite seu nome" />
          <Input 
            label="Email" 
            type="email" 
            icon={<Search />}
            helperText="Digite um email válido"
          />
          <Input 
            label="Senha" 
            type="password" 
            error="Senha muito curta"
          />
        </div>
      </section>
    </div>
  );
};
```

---

## 🔄 Próximo Entregável

**[Entregável 04 - Autenticação](./Frontend-Entregavel-04-Autenticacao.md)**
