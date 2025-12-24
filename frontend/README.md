# Sistema de Rede de Credenciamento - Frontend

Frontend React para gerenciamento de rede de parceiros e comissões multi-nível.

## 🚀 Tecnologias

- **React 18.2** + TypeScript 5
- **Vite 5** como bundler e dev server
- **Tailwind CSS** (Tema Preto e Branco)
- **React Router v6** para navegação
- **React Query (TanStack Query) v5** para gerenciamento de estado do servidor
- **Zustand 4** para estado global
- **Axios 1.6** para requisições HTTP
- **React Hook Form 7 + Zod 3** para formulários e validação
- **Lucide React** para ícones consistentes
- **date-fns** para manipulação de datas

## 📋 Pré-requisitos

- Node.js 18+ 
- npm 9+ ou yarn
- Backend da API rodando (porta 5000 por padrão)

## 🔧 Instalação

```bash
# Clone o repositório
git clone [url-do-repositorio]

# Navegue até a pasta do frontend
cd frontend

# Instale as dependências
npm install

# Copie o arquivo de ambiente
cp .env.example .env.local

# Configure a URL da API no .env.local
# VITE_API_BASE_URL=http://localhost:5000/api
```

## 🏃 Como Executar

### Modo Desenvolvimento
```bash
npm run dev
# Acesse: http://localhost:5173
```

### Build de Produção
```bash
npm run build
# Arquivos gerados em: dist/
```

### Preview da Build
```bash
npm run preview
# Acesse: http://localhost:4173
```

### Testes
```bash
npm run test
```

### Lint
```bash
npm run lint
```

## 📁 Estrutura de Pastas

```
src/
├── api/                      # Configuração Axios e endpoints da API
│   ├── axios.config.ts       # Instância Axios com interceptors
│   ├── auth.api.ts           # Endpoints de autenticação
│   ├── users.api.ts          # Endpoints de usuários
│   ├── vectors.api.ts        # Endpoints de vetores
│   ├── partners.api.ts       # Endpoints de parceiros
│   ├── businessTypes.api.ts  # Endpoints de tipos de negócio
│   ├── business.api.ts       # Endpoints de negócios
│   ├── payments.api.ts       # Endpoints de pagamentos
│   ├── reports.api.ts        # Endpoints de relatórios
│   └── audit.api.ts          # Endpoints de auditoria
│
├── components/
│   ├── common/               # Componentes reutilizáveis (18)
│   │   ├── Alert/            # Alertas de feedback
│   │   ├── Badge/            # Tags e badges coloridos
│   │   ├── Button/           # Botões primário/secondary/outline/ghost
│   │   ├── Card/             # Cards para conteúdo
│   │   ├── Checkbox/         # Checkboxes customizados
│   │   ├── ConfirmDialog/    # Diálogos de confirmação
│   │   ├── EmptyState/       # Estados vazios
│   │   ├── ErrorBoundary/    # Captura de erros React
│   │   ├── Input/            # Inputs de texto
│   │   ├── Loading/          # Spinners de carregamento
│   │   ├── Modal/            # Modais genéricos
│   │   ├── Pagination/       # Paginação de listas
│   │   ├── PartnerTreeView/  # Visualização em árvore
│   │   ├── Radio/            # Radio buttons
│   │   ├── Select/           # Selects customizados
│   │   ├── Table/            # Tabelas responsivas
│   │   ├── Textarea/         # Text areas
│   │   └── Toast/            # Notificações toast
│   │
│   ├── layout/               # Componentes de layout
│   │   ├── Layout.tsx        # Layout principal (Header + Sidebar)
│   │   ├── PublicLayout.tsx  # Layout público (login)
│   │   ├── Header.tsx        # Cabeçalho com perfil e logout
│   │   └── Sidebar.tsx       # Menu lateral com navegação
│   │
│   └── routes/               # Componentes de rota
│       ├── PrivateRoute.tsx      # HOC para rotas autenticadas
│       └── PermissionRoute.tsx   # HOC para controle de permissão
│
├── hooks/                    # Hooks customizados
│   └── useToast.tsx         # Hook para notificações toast
│
├── pages/                    # Páginas da aplicação (24 páginas)
│   ├── auth/                 # Autenticação
│   │   └── LoginPage.tsx
│   │
│   ├── DashboardPage.tsx     # Dashboard principal
│   │
│   ├── Users/                # Gestão de Usuários
│   │   ├── UsersListPage.tsx
│   │   └── UserFormPage.tsx
│   │
│   ├── Vectors/              # Gestão de Vetores
│   │   ├── VectorsListPage.tsx
│   │   └── VectorFormPage.tsx
│   │
│   ├── Partners/             # Gestão de Parceiros
│   │   ├── PartnersListPage.tsx
│   │   ├── PartnerFormPage.tsx
│   │   └── PartnerTreePage.tsx
│   │
│   ├── BusinessTypes/        # Tipos de Negócio
│   │   ├── BusinessTypesListPage.tsx
│   │   └── BusinessTypeFormPage.tsx
│   │
│   ├── Business/             # Gestão de Negócios
│   │   ├── BusinessListPage.tsx
│   │   ├── BusinessFormPage.tsx
│   │   └── BusinessDetailPage.tsx
│   │
│   ├── Payments/             # Pagamentos
│   │   └── PaymentsListPage.tsx
│   │
│   ├── Reports/              # Relatórios
│   │   ├── PartnersReportPage.tsx
│   │   ├── FinancialReportPage.tsx
│   │   └── BusinessReportPage.tsx
│   │
│   └── Audit/                # Auditoria
│       ├── AuditLogsPage.tsx
│       └── AuditTimelinePage.tsx
│
├── routes/
│   └── router.tsx            # Configuração de rotas React Router
│
├── store/                    # Estado global Zustand
│   └── authStore.ts          # Store de autenticação
│
├── styles/
│   └── index.css             # Estilos globais e Tailwind imports
│
├── types/                    # Tipos TypeScript globais
│   ├── auth.types.ts         # User, LoginRequest, AuthResponse
│   ├── user.types.ts         # UserListItem, UserDetail, CreateUserRequest
│   ├── vector.types.ts       # Vector, CreateVectorRequest
│   ├── partner.types.ts      # Partner, CreatePartnerRequest
│   ├── businessType.types.ts # BusinessType
│   ├── business.types.ts     # Business, Commission
│   ├── payment.types.ts      # Payment
│   ├── report.types.ts       # PartnersReport, FinancialReport
│   ├── audit.types.ts        # AuditLog
│   ├── common.types.ts       # PaginatedResponse, ApiError
│   └── enums.ts              # Permission, Status, ActionType
│
├── utils/                    # Funções utilitárias
│   ├── formatters.ts         # formatCurrency, formatDate, formatCPF
│   ├── validators.ts         # validateCPF, validateEmail
│   └── permissions.ts        # hasPermission, canAccess
│
├── App.tsx                   # Componente raiz
├── main.tsx                  # Entry point
└── vite-env.d.ts            # Type definitions do Vite
```

## 🎨 Design System

### Paleta de Cores (Tema Preto e Branco)
- **Primária:** `#000000` (Preto)
- **Secundária:** `#FFFFFF` (Branco)
- **Cinzas Tailwind:** `gray-50` até `gray-900`
- **Estados:**
  - Success: `green-600`
  - Error: `red-600`
  - Warning: `yellow-600`
  - Info: `blue-600`

### Componentes Base
Todos os componentes seguem o design system preto e branco:
- Botões com variantes: `primary`, `secondary`, `outline`, `ghost`
- Inputs com borda preta `border-2 border-black`
- Focus ring preto `focus:ring-2 focus:ring-black`
- Badges coloridos para status
- Animações suaves com Tailwind transitions

## 🔐 Variáveis de Ambiente

Crie um arquivo `.env.local` baseado no `.env.example`:

```env
# API Configuration (obrigatório)
VITE_API_BASE_URL=http://localhost:5000/api

# Application
VITE_APP_NAME=Sistema de Rede de Credenciamento
VITE_APP_VERSION=1.0.0

# Environment
VITE_ENV=development
```

### Variáveis de Produção
```env
VITE_API_BASE_URL=https://api.production.com/api
VITE_ENV=production
```

**⚠️ Importante:** Todas as variáveis devem começar com `VITE_` para serem expostas ao cliente.

## 📚 Scripts Disponíveis

| Script | Comando | Descrição |
|--------|---------|-----------|
| **dev** | `npm run dev` | Inicia servidor de desenvolvimento (porta 5173) |
| **build** | `npm run build` | Compila TypeScript + build Vite para produção |
| **preview** | `npm run preview` | Preview da build de produção localmente |
| **lint** | `npm run lint` | Executa ESLint para verificar código |
| **test** | `npm run test` | Executa testes com Vitest |

## 🔑 Autenticação e Permissões

### Fluxo de Autenticação
1. Usuário faz login em `/login`
2. Backend retorna `accessToken` e `refreshToken`
3. Tokens são armazenados no Zustand store
4. `accessToken` é enviado em cada requisição via header `Authorization`
5. Quando `401` é recebido, axios interceptor tenta refresh automático
6. Se refresh falhar, redireciona para login

### Sistema de Permissões

| Perfil | Código | Acesso |
|--------|--------|--------|
| **Admin Global** | `AdminGlobal` | Acesso total ao sistema |
| **Admin Vetor** | `AdminVetor` | Gerencia apenas seu vetor |
| **Operador** | `Operador` | Parceiros, negócios e pagamentos |
| **Parceiro** | `Parceiro` | Visualiza suas comissões (futuro) |

### Controle nas Rotas
```typescript
// PrivateRoute - verifica autenticação
<PrivateRoute>
  <Layout />
</PrivateRoute>

// PermissionRoute - verifica permissão específica
<PermissionRoute allowedPermissions={[Permission.AdminGlobal]}>
  <SensitivePage />
</PermissionRoute>
```

## 🎯 Funcionalidades Principais

### Entregável 05 - Gestão de Usuários e Vetores
- ✅ CRUD de Vetores (AdminGlobal)
- ✅ CRUD de Usuários com 4 perfis
- ✅ Ativar/Inativar usuários e vetores
- ✅ Filtros por perfil, vetor e status

### Entregável 06 - Parceiros e Tipos de Negócio
- ✅ CRUD de Parceiros com recomendador
- ✅ Visualização em árvore hierárquica
- ✅ CRUD de Tipos de Negócio
- ✅ Ativar/Inativar parceiros

### Entregável 07 - Negócios e Comissões
- ✅ Criar negócios com cálculo de comissões
- ✅ Visualizar detalhes e comissões por nível
- ✅ Cancelar negócios (com confirmação)
- ✅ Editar observações

### Entregável 08 - Pagamentos e Relatórios
- ✅ Listar comissões pendentes/pagas
- ✅ Processar pagamentos em lote
- ✅ Relatório de Parceiros (com filtros)
- ✅ Relatório Financeiro (resumos)
- ✅ Relatório de Negócios (status)

### Entregável 09 - Auditoria e Logs
- ✅ Lista de logs com 6 filtros
- ✅ Timeline de eventos por entidade
- ✅ Detalhes de mudanças (diff old → new)
- ✅ Acesso restrito (AdminGlobal)

## 📝 Convenções de Código

### Nomenclatura
- **Componentes:** PascalCase (`UserFormPage.tsx`)
- **Funções/Variáveis:** camelCase (`getUserById`)
- **Constantes:** UPPER_SNAKE_CASE (`API_BASE_URL`)
- **Tipos/Interfaces:** PascalCase (`User`, `CreateUserRequest`)

### Imports
Use path aliases para imports limpos:
```typescript
// ✅ Bom
import { Button } from '@/components/common/Button';
import { authApi } from '@/api/auth.api';
import { useAuthStore } from '@/store/authStore';

// ❌ Evite
import { Button } from '../../../components/common/Button';
```

### Componentes
- Use componentes funcionais com hooks
- Mantenha componentes pequenos (< 300 linhas)
- Extraia lógica complexa para hooks customizados
- Use TypeScript para todas as props

### Estado e Requisições
```typescript
// React Query para dados do servidor
const { data, isLoading } = useQuery({
  queryKey: ['users'],
  queryFn: usersApi.list,
});

// Zustand para estado global (ex: auth)
const { user, setAuth } = useAuthStore();

// useState para estado local do componente
const [page, setPage] = useState(1);
```

### Validação de Formulários
```typescript
// Sempre use Zod + React Hook Form
const schema = z.object({
  name: z.string().min(3),
  email: z.string().email(),
});

const { register, handleSubmit } = useForm({
  resolver: zodResolver(schema),
});
```

## 🧪 Testes

```bash
# Executar todos os testes
npm run test

# Watch mode
npm run test -- --watch

# Cobertura
npm run test -- --coverage
```

### Estrutura de Testes
```
src/
├── components/
│   └── Button/
│       ├── Button.tsx
│       └── Button.test.tsx
```

## 🚢 Deploy

Veja [DEPLOY.md](./DEPLOY.md) para guia completo de deploy em produção.

### Build Rápido
```bash
# Build otimizado
npm run build

# Arquivos gerados em dist/
# Servir com nginx, Apache, ou CDN
```

## 🐛 Troubleshooting

### Erro de conexão com API
```bash
# Verifique se backend está rodando
curl http://localhost:5000/api/health

# Verifique variável de ambiente
echo $VITE_API_BASE_URL
```

### Problemas com dependências
```bash
# Limpar node_modules e reinstalar
rm -rf node_modules package-lock.json
npm install
```

### Build falhando
```bash
# Verificar erros TypeScript
npx tsc --noEmit

# Verificar erros ESLint
npm run lint
```

## 📖 Documentação Adicional

- [Documentação de Componentes](./COMPONENTS.md) - Guia completo de todos os componentes
- [Guia de Deploy](./DEPLOY.md) - Instruções de deploy em produção
- [Projeto.md](../Projeto.md) - Regras de negócio e requisitos
- [MVP Plano](../docs/frontend/MVP-Plano-Entregaveis.md) - Plano de entregáveis

## 👥 Perfis de Teste

Para testes, use os seguintes perfis:

| Usuário | Email | Senha | Perfil |
|---------|-------|-------|--------|
| Admin | admin@test.com | admin123 | AdminGlobal |
| Vetor A | vetor@test.com | vetor123 | AdminVetor |
| Operador | operador@test.com | oper123 | Operador |

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📞 Suporte

Para dúvidas:
- Consulte [Projeto.md](../Projeto.md) para regras de negócio
- Veja [COMPONENTS.md](./COMPONENTS.md) para uso de componentes
- Abra uma issue no repositório

## 📄 Licença

Este projeto é propriedade privada. Todos os direitos reservados.

---

**Desenvolvido com ❤️ usando React + TypeScript**
