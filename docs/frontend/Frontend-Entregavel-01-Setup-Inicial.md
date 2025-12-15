# Entregável 01 - Setup Inicial

## 📋 Informações Gerais

- **Duração estimada:** 1-2 dias
- **Prioridade:** Crítica
- **Dependências:** Nenhuma
- **Desenvolvedor(es):** 1 desenvolvedor

---

## 🎯 Objetivo

Configurar o ambiente de desenvolvimento completo com todas as ferramentas, dependências e estrutura base do projeto React com tema preto e branco.

---

## 📦 Entregáveis

### 1. Projeto Vite + React + TypeScript
- [ ] Projeto criado com template TypeScript
- [ ] Build funcionando sem erros
- [ ] Dev server rodando em http://localhost:5173

### 2. Dependências Instaladas
- [ ] Todas as dependências de produção instaladas
- [ ] Todas as dependências de desenvolvimento instaladas
- [ ] `package.json` e `package-lock.json` commitados

### 3. Tailwind CSS Configurado
- [ ] Tailwind instalado e configurado
- [ ] Paleta de cores preto e branco implementada
- [ ] Arquivo de estilos globais criado
- [ ] Fontes configuradas (Inter ou similar)

### 4. Estrutura de Pastas
- [ ] Todas as pastas criadas conforme arquitetura
- [ ] Arquivos `.gitkeep` ou `index.ts` em pastas vazias

### 5. Configurações
- [ ] `.env.example` criado com variáveis de ambiente
- [ ] `.gitignore` configurado
- [ ] `tsconfig.json` otimizado
- [ ] `vite.config.ts` com configurações necessárias

---

## 🔧 Tarefas Detalhadas

### Tarefa 1.1 - Criar Projeto

```bash
# Criar projeto
npm create vite@latest frontend -- --template react-ts

# Entrar na pasta
cd frontend

# Instalar dependências base
npm install
```

**Verificação:** `npm run dev` deve iniciar o servidor sem erros

---

### Tarefa 1.2 - Instalar Dependências Principais

```bash
# Navegação e roteamento
npm install react-router-dom

# HTTP client
npm install axios

# Gerenciamento de estado
npm install @tanstack/react-query zustand

# Formulários e validação
npm install react-hook-form @hookform/resolvers zod

# Utilidades
npm install lucide-react date-fns

# Tailwind CSS
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p

# Types
npm install -D @types/node
```

**Verificação:** Verificar `package.json` contém todas as dependências

---

### Tarefa 1.3 - Configurar Tailwind CSS

**1. Atualizar `tailwind.config.js`:**

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

**2. Criar `src/styles/global.css`:**

```css
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');

@tailwind base;
@tailwind components;
@tailwind utilities;

@layer base {
  * {
    @apply border-gray-200;
  }
  
  html {
    @apply text-gray-900;
  }
  
  body {
    @apply bg-gray-50 font-sans antialiased;
  }
}

@layer components {
  .btn {
    @apply inline-flex items-center justify-center font-medium transition-colors 
           focus:outline-none focus:ring-2 focus:ring-offset-2 
           disabled:opacity-50 disabled:cursor-not-allowed;
  }
  
  .input {
    @apply w-full px-3 py-2 border-2 border-gray-300 rounded-md
           text-gray-900 placeholder-gray-400
           focus:outline-none focus:ring-2 focus:ring-black focus:border-black
           disabled:bg-gray-100 disabled:cursor-not-allowed;
  }
  
  .card {
    @apply bg-white rounded-lg shadow border border-gray-200 p-6;
  }
}
```

**3. Importar no `src/main.tsx`:**

```typescript
import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import './styles/global.css'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
)
```

**Verificação:** Estilos do Tailwind devem ser aplicados

---

### Tarefa 1.4 - Criar Estrutura de Pastas

```bash
# No diretório src/, criar estrutura:
mkdir -p src/@types
mkdir -p src/api/endpoints
mkdir -p src/assets/images
mkdir -p src/components/common/Button
mkdir -p src/components/common/Input
mkdir -p src/components/common/Select
mkdir -p src/components/common/Table
mkdir -p src/components/common/Modal
mkdir -p src/components/common/Card
mkdir -p src/components/common/Loading
mkdir -p src/components/common/Alert
mkdir -p src/components/common/Pagination
mkdir -p src/components/layout/Header
mkdir -p src/components/layout/Sidebar
mkdir -p src/components/layout/Footer
mkdir -p src/components/features/auth
mkdir -p src/components/features/users
mkdir -p src/components/features/vectors
mkdir -p src/components/features/partners
mkdir -p src/components/features/businessTypes
mkdir -p src/components/features/business
mkdir -p src/components/features/payments
mkdir -p src/components/features/reports
mkdir -p src/components/features/audit
mkdir -p src/hooks
mkdir -p src/pages/Users
mkdir -p src/pages/Vectors
mkdir -p src/pages/Partners
mkdir -p src/pages/BusinessTypes
mkdir -p src/pages/Business
mkdir -p src/pages/Payments
mkdir -p src/pages/Reports
mkdir -p src/pages/Audit
mkdir -p src/routes
mkdir -p src/services
mkdir -p src/store
mkdir -p src/styles
mkdir -p src/types
mkdir -p src/utils
```

**Criar arquivo `index.ts` vazio em cada pasta para Git rastrear:**

```bash
# Windows PowerShell
Get-ChildItem -Path src -Recurse -Directory | ForEach-Object { New-Item -Path "$($_.FullName)\index.ts" -ItemType File -Force }
```

**Verificação:** Estrutura de pastas completa criada

---

### Tarefa 1.5 - Configurar Variáveis de Ambiente

**Criar `.env.example`:**

```env
# API Configuration
VITE_API_BASE_URL=http://localhost:5000/api

# Application
VITE_APP_NAME=Sistema de Rede de Credenciamento
VITE_APP_VERSION=1.0.0

# Environment
VITE_ENV=development
```

**Criar `.env.local` (não commitado):**

```env
VITE_API_BASE_URL=http://localhost:5000/api
VITE_APP_NAME=Sistema de Rede de Credenciamento
VITE_APP_VERSION=1.0.0
VITE_ENV=development
```

**Atualizar `.gitignore`:**

```gitignore
# Logs
logs
*.log
npm-debug.log*
yarn-debug.log*
yarn-error.log*
pnpm-debug.log*
lerna-debug.log*

node_modules
dist
dist-ssr
*.local

# Editor directories and files
.vscode/*
!.vscode/extensions.json
.idea
.DS_Store
*.suo
*.ntvs*
*.njsproj
*.sln
*.sw?

# Environment variables
.env.local
.env.*.local
```

**Verificação:** Variáveis acessíveis via `import.meta.env`

---

### Tarefa 1.6 - Configurar TypeScript

**Atualizar `tsconfig.json`:**

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "useDefineForClassFields": true,
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "module": "ESNext",
    "skipLibCheck": true,

    /* Bundler mode */
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "jsx": "react-jsx",

    /* Linting */
    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true,

    /* Path mapping */
    "baseUrl": ".",
    "paths": {
      "@/*": ["./src/*"],
      "@/components/*": ["./src/components/*"],
      "@/types/*": ["./src/types/*"],
      "@/utils/*": ["./src/utils/*"],
      "@/hooks/*": ["./src/hooks/*"],
      "@/api/*": ["./src/api/*"],
      "@/store/*": ["./src/store/*"]
    }
  },
  "include": ["src"],
  "references": [{ "path": "./tsconfig.node.json" }]
}
```

**Atualizar `vite.config.ts`:**

```typescript
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      '@/components': path.resolve(__dirname, './src/components'),
      '@/types': path.resolve(__dirname, './src/types'),
      '@/utils': path.resolve(__dirname, './src/utils'),
      '@/hooks': path.resolve(__dirname, './src/hooks'),
      '@/api': path.resolve(__dirname, './src/api'),
      '@/store': path.resolve(__dirname, './src/store'),
    },
  },
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
    },
  },
})
```

**Verificação:** Imports com alias devem funcionar

---

### Tarefa 1.7 - Criar App Base

**Atualizar `src/App.tsx`:**

```typescript
import React from 'react'

function App() {
  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center">
      <div className="text-center">
        <h1 className="text-4xl font-bold text-black mb-4">
          Sistema de Rede de Credenciamento
        </h1>
        <p className="text-gray-600 text-lg">
          Frontend React - Setup Completo ✓
        </p>
        <div className="mt-8 p-6 bg-white rounded-lg shadow-lg border-2 border-black max-w-md mx-auto">
          <h2 className="text-2xl font-semibold mb-4">Tecnologias</h2>
          <ul className="text-left space-y-2 text-gray-700">
            <li>✓ React 18 + TypeScript</li>
            <li>✓ Vite</li>
            <li>✓ Tailwind CSS (Preto e Branco)</li>
            <li>✓ React Router</li>
            <li>✓ React Query</li>
            <li>✓ Zustand</li>
            <li>✓ Axios</li>
          </ul>
        </div>
      </div>
    </div>
  )
}

export default App
```

**Verificação:** Página inicial deve exibir corretamente com estilos

---

### Tarefa 1.8 - Documentação Inicial

**Criar `README.md` na pasta frontend:**

```markdown
# Sistema de Rede de Credenciamento - Frontend

Frontend React para gerenciamento de rede de parceiros e comissões.

## 🚀 Tecnologias

- React 18 + TypeScript
- Vite
- Tailwind CSS (Tema Preto e Branco)
- React Router v6
- React Query (TanStack Query)
- Zustand
- Axios
- React Hook Form + Zod

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
```

## 📁 Estrutura

```
src/
├── api/          # Configuração Axios e endpoints
├── components/   # Componentes React
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

## 📚 Documentação

Ver [Frontend-React-Documentation.md](../Frontend-React-Documentation.md)
```

**Verificação:** README claro e informativo

---

## ✅ Critérios de Aceitação

- [ ] Projeto criado e rodando sem erros
- [ ] Todas as dependências instaladas e funcionando
- [ ] Tailwind CSS configurado com tema preto e branco
- [ ] Estrutura de pastas completa
- [ ] Variáveis de ambiente configuradas
- [ ] TypeScript com path aliases funcionando
- [ ] Página inicial exibindo corretamente
- [ ] README.md criado
- [ ] Git inicializado com `.gitignore` correto
- [ ] Primeiro commit realizado

---

## 🧪 Testes de Validação

1. **Build Test:**
   ```bash
   npm run build
   # Deve completar sem erros
   ```

2. **Dev Server Test:**
   ```bash
   npm run dev
   # Deve abrir em http://localhost:5173 sem erros no console
   ```

3. **TypeScript Test:**
   ```bash
   npx tsc --noEmit
   # Não deve ter erros de tipo
   ```

4. **Import Alias Test:**
   Criar arquivo `src/test.ts`:
   ```typescript
   import type { User } from '@/types/auth.types'
   // Não deve dar erro de import
   ```

---

## 📝 Notas Importantes

- Sempre use `npm` ao invés de `yarn` para manter consistência
- Não commite o arquivo `.env.local`
- Mantenha a paleta de cores preto e branco consistente
- Use os path aliases configurados para imports mais limpos

---

## 🔄 Próximo Entregável

**[Entregável 02 - Configuração Base](./Frontend-Entregavel-02-Configuracao-Base.md)**

---

## 📞 Suporte

Para dúvidas sobre a arquitetura, consulte a documentação completa ou o arquivo `Projeto.md`.
