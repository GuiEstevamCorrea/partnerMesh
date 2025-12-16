# 📍 Estrutura de Rotas

## Visão Geral

Este diretório contém a configuração de rotas do sistema usando React Router v6 com `createBrowserRouter`.

## Arquivos

- **`router.tsx`**: Configuração principal das rotas
- **`PrivateRoute.tsx`**: HOC para proteção de rotas autenticadas
- **`PermissionRoute.tsx`**: HOC para proteção baseada em permissões
- **`index.ts`**: Exportação centralizada

## Estrutura de Rotas

### Rotas Públicas

```
/ (PublicLayout)
  ├── / → redirect para /login
  └── /login → LoginPage
```

### Rotas Protegidas

```
/ (PrivateRoute + Layout)
  ├── /dashboard → DashboardPage
  │
  ├── /usuarios (Entregável 05)
  │   ├── /usuarios → Lista
  │   ├── /usuarios/novo → Criar
  │   └── /usuarios/:id/editar → Editar
  │
  ├── /vetores (Entregável 05)
  │   ├── /vetores → Lista
  │   ├── /vetores/novo → Criar
  │   └── /vetores/:id/editar → Editar
  │
  ├── /parceiros (Entregável 06)
  │   ├── /parceiros → Lista
  │   ├── /parceiros/novo → Criar
  │   ├── /parceiros/:id/editar → Editar
  │   └── /parceiros/arvore → Árvore Hierárquica
  │
  ├── /tipos-negocio (Entregável 06)
  │   ├── /tipos-negocio → Lista
  │   ├── /tipos-negocio/novo → Criar
  │   └── /tipos-negocio/:id/editar → Editar
  │
  ├── /negocios (Entregável 07)
  │   ├── /negocios → Lista
  │   ├── /negocios/novo → Criar
  │   ├── /negocios/:id → Detalhes
  │   └── /negocios/:id/editar → Editar
  │
  ├── /pagamentos (Entregável 07)
  │   └── /pagamentos → Lista e Processamento
  │
  ├── /relatorios (Entregável 08)
  │   ├── /relatorios/parceiros → Relatório de Parceiros
  │   ├── /relatorios/financeiro → Relatório Financeiro
  │   └── /relatorios/negocios → Relatório de Negócios
  │
  └── /auditoria (Entregável 09)
      ├── /auditoria → Lista de Logs
      └── /auditoria/timeline/:entityType/:entityId → Timeline
```

### Wildcard

```
* → redirect para /login
```

## Proteção de Rotas

### PrivateRoute

Protege rotas que requerem autenticação:

```tsx
<PrivateRoute>
  <Layout />
</PrivateRoute>
```

**Comportamento:**
- Verifica se usuário está autenticado (token existe)
- Se não autenticado: redireciona para `/login`
- Se autenticado: renderiza o componente filho

### PermissionRoute

Protege rotas baseadas em permissões específicas:

```tsx
<PermissionRoute requiredPermission="users.create">
  <UserFormPage />
</PermissionRoute>
```

**Comportamento:**
- Verifica se usuário tem a permissão requerida
- Se não tiver: exibe página de acesso negado
- Se tiver: renderiza o componente

## Layouts

### PublicLayout

Layout minimalista para páginas públicas:
- Header com logo e título
- Conteúdo centralizado
- Footer com copyright

### Layout (Protegido)

Layout completo para área autenticada:
- Header com menu de usuário
- Sidebar com navegação
- Conteúdo principal
- Footer

## Como Adicionar Novas Rotas

### 1. Criar a Página

```tsx
// src/pages/exemplo/ExemploPage.tsx
export const ExemploPage = () => {
  return <div>Exemplo</div>;
};
```

### 2. Adicionar ao Router

```tsx
// src/routes/router.tsx
import { ExemploPage } from '@/pages/exemplo/ExemploPage';

// Dentro do array de children das rotas protegidas:
{
  path: 'exemplo',
  element: <ExemploPage />,
}
```

### 3. Adicionar Link no Menu

```tsx
// src/components/layout/Sidebar/Sidebar.tsx
<Link to="/exemplo">Exemplo</Link>
```

## Navegação Programática

### Usando useNavigate

```tsx
import { useNavigate } from 'react-router-dom';

const navigate = useNavigate();

// Navegar para rota
navigate('/dashboard');

// Navegar com replace (sem adicionar ao histórico)
navigate('/login', { replace: true });

// Voltar
navigate(-1);
```

### Usando Link

```tsx
import { Link } from 'react-router-dom';

<Link to="/usuarios">Usuários</Link>
```

## Estado da Implementação

| Entregável | Status | Rotas |
|-----------|--------|-------|
| 04 - Autenticação | ✅ Completo | `/`, `/login`, `/dashboard` |
| 05 - Usuários/Vetores | 🚧 Planejado | `/usuarios/*`, `/vetores/*` |
| 06 - Parceiros/Tipos | 🚧 Planejado | `/parceiros/*`, `/tipos-negocio/*` |
| 07 - Negócios/Pagamentos | 🚧 Planejado | `/negocios/*`, `/pagamentos` |
| 08 - Relatórios | 🚧 Planejado | `/relatorios/*` |
| 09 - Auditoria | 🚧 Planejado | `/auditoria/*` |

## Referências

- [React Router v6 Docs](https://reactrouter.com/)
- [createBrowserRouter](https://reactrouter.com/en/main/routers/create-browser-router)
- [Layout Routes](https://reactrouter.com/en/main/start/concepts#layout-routes)
