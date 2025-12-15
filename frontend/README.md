# Sistema de Rede de Credenciamento - Frontend

Frontend React para gerenciamento de rede de parceiros e comissões.

## 🚀 Tecnologias

- **React 18** + TypeScript
- **Vite** como bundler
- **Tailwind CSS** (Tema Preto e Branco)
- **React Router v6** para navegação
- **React Query (TanStack Query)** para gerenciamento de estado do servidor
- **Zustand** para estado global
- **Axios** para requisições HTTP
- **React Hook Form + Zod** para formulários e validação
- **Lucide React** para ícones

## 📋 Pré-requisitos

- Node.js 18+ 
- npm ou yarn

## 🔧 Instalação

```bash
# Instalar dependências
npm install

# Copiar arquivo de ambiente
cp .env.example .env.local
```

## 🏃 Executar

```bash
# Desenvolvimento
npm run dev

# Build
npm run build

# Preview da build
npm run preview

# Testes
npm run test
```

## 📁 Estrutura

```
src/
├── api/          # Configuração Axios e endpoints
├── components/   # Componentes React
│   ├── common/       # Componentes reutilizáveis
│   ├── layout/       # Layout (Header, Sidebar, Footer)
│   └── features/     # Componentes específicos por feature
├── hooks/        # Hooks customizados
├── pages/        # Páginas/telas
├── routes/       # Configuração de rotas
├── store/        # Estado global (Zustand)
├── styles/       # Estilos globais
├── types/        # Tipos TypeScript
└── utils/        # Utilitários
```

## 🎨 Paleta de Cores

- **Preto:** #000000
- **Branco:** #FFFFFF
- **Cinzas:** 50-900 (Tailwind)

## 🔐 Variáveis de Ambiente

Crie um arquivo `.env.local` baseado no `.env.example`:

```env
VITE_API_BASE_URL=http://localhost:5000/api
VITE_APP_NAME=Sistema de Rede de Credenciamento
```

## 📚 Documentação

- [Documentação Completa](../docs/frontend/Frontend-React-Documentation.md)
- [Guia de Entregáveis](../docs/frontend/Frontend-Entregaveis-Indice.md)

## 🧪 Testes

```bash
# Executar testes
npm run test

# Executar testes em watch mode
npm run test:watch

# Cobertura de testes
npm run test:coverage
```

## 🚢 Deploy

```bash
# Build de produção
npm run build

# Preview local
npm run preview
```

## 📝 Padrões de Código

- Use TypeScript com tipagem estrita
- Siga o padrão de nomenclatura em inglês
- Use componentes funcionais com hooks
- Mantenha componentes pequenos e focados
- Use path aliases (@/) para imports

## 🎯 Funcionalidades

- ✅ Autenticação com JWT
- ✅ Controle de permissões por perfil
- ✅ CRUD de Vetores, Usuários, Parceiros
- ✅ Gestão de Negócios e Comissões
- ✅ Relatórios financeiros
- ✅ Auditoria de ações
- ✅ Tema preto e branco consistente

## 👥 Perfis de Usuário

- **Admin Global:** Acesso total ao sistema
- **Admin de Vetor:** Gerencia seu próprio vetor
- **Operador:** Gerencia parceiros e negócios
- **Parceiro:** Visualiza suas comissões (futuro)

## 📞 Suporte

Para dúvidas sobre regras de negócio, consulte [Projeto.md](../Projeto.md)
