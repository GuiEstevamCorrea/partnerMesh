# PartnerMesh

Sistema de gestão de rede de parceiros com controle de negócios e distribuição automática de comissões.

## 📋 Sobre o Projeto

PartnerMesh é uma plataforma completa para gerenciamento de redes multinível de parceiros, permitindo o controle de negócios fechados entre parceiros e a distribuição automática de comissões através de uma estrutura hierárquica de até 3 níveis.

### Conceitos Principais

- **Vetor**: Entidade proprietária da rede de parceiros. Cada vetor possui sua própria rede independente.
- **Parceiro**: Membro da rede que pode fechar negócios e recomendar novos parceiros.
- **Negócio**: Transação entre dois parceiros da rede, gerando comissões automaticamente.
- **Comissão**: Valor calculado automaticamente (10% do negócio) e distribuído para até 3 níveis acima dos parceiros envolvidos.

## 🏗️ Arquitetura

### Backend (.NET 8)

```
Api/                      # Adaptador Primário - Controllers e endpoints REST
Application/              # Camada de Aplicação - Casos de uso (Ports)
  ├── UseCases/          # Casos de uso implementados
  ├── Interfaces/        # Portas (Interfaces de Repositórios e Serviços)
  └── DTOs/              # Objetos de transferência de dados
Domain/                   # Núcleo do Hexágono - Entidades e regras de negócio
  ├── Entities/          # Modelos de domínio
  ├── ValueObjects/      # Objetos de valor
  ├── ValueTypes/        # Enums e configurações
  └── Extensions/        # Extensões de domínio
Infrastructure/           # Adaptador Secundário - Implementações externas
  ├── Data/              # Contexto EF Core
  ├── Repositories/      # Implementação das Portas de Repositório
  └── Services/          # Implementação das Portas de Serviços Externos
```

**Arquitetura**: Hexagonal (Ports and Adapters)
- **Núcleo (Domain)**: Regras de negócio isoladas e independentes
- **Portas (Application/Interfaces)**: Contratos que definem comunicação
- **Adaptadores**: 
  - **Primários (Api)**: Controladores REST que recebem requisições
  - **Secundários (Infrastructure)**: Implementações de persistência e serviços externos
- **Princípios**:
  - Separação clara de responsabilidades
  - Domain-Driven Design (DDD)
  - Inversão de dependência (DIP)
  - Repository Pattern com Entity Framework Core
  - CQRS para queries complexas

### Frontend (React + TypeScript)

```
src/
  ├── api/               # Configuração Axios e endpoints
  ├── components/        # Componentes reutilizáveis
  ├── pages/             # Páginas da aplicação
  ├── hooks/             # Custom hooks
  ├── types/             # TypeScript types
  ├── utils/             # Utilitários
  └── contexts/          # Contexts React
```

**Stack Frontend**:
- React 18 com TypeScript
- Vite para build
- React Router para navegação
- React Query (TanStack Query) para gerenciamento de estado
- Tailwind CSS para estilização
- Zod para validação de formulários

## 🚀 Funcionalidades

### Gestão de Parceiros
- ✅ Cadastro de parceiros com estrutura hierárquica
- ✅ Recomendação de novos parceiros (até 3 níveis)
- ✅ Ativação/desativação de parceiros
- ✅ Visualização da árvore de recomendações

### Gestão de Negócios
- ✅ Registro de negócios entre dois parceiros
- ✅ Cálculo automático de comissões (10% do valor)
- ✅ Distribuição inteligente para até 3 níveis acima
- ✅ Validação para evitar negócios entre parceiros inválidos
- ✅ Cancelamento de negócios
- ✅ Histórico completo de transações

### Distribuição de Comissões

**Regra de Distribuição:**
- Parceiros envolvidos no negócio **NÃO recebem comissão**
- Comissão distribuída apenas para níveis acima (máximo 3 níveis)
- Percentuais de distribuição:
  - **Nível 1** (recomendador direto): 50%
  - **Nível 2** (segundo nível): 30%
  - **Nível 3** (terceiro nível): 20%
  - **Vetor**: Recebe o saldo restante (quando há menos de 3 níveis)

**Exemplos:**

```
Negócio de R$ 1.000,00 → Comissão total: R$ 100,00

Cenário 1: Parceiro com 3 níveis acima
├─ Nível 1: R$ 50,00 (50%)
├─ Nível 2: R$ 30,00 (30%)
└─ Nível 3: R$ 20,00 (20%)

Cenário 2: Parceiro com 1 nível acima
├─ Nível 1: R$ 50,00 (50%)
└─ Vetor: R$ 50,00 (50%)

Cenário 3: Parceiro sem recomendador
└─ Vetor: R$ 100,00 (100%)
```

### Relatórios e Dashboards
- ✅ Relatório financeiro com filtros avançados
- ✅ Relatório de parceiros por nível
- ✅ Dashboard com métricas em tempo real
- ✅ Exportação de dados

### Auditoria
- ✅ Log completo de todas as operações
- ✅ Rastreamento de alterações por usuário
- ✅ Consulta de histórico de ações

## 🛠️ Tecnologias

### Backend
- .NET 8.0
- ASP.NET Core Web API
- Entity Framework Core 8.0
- SQL Server
- JWT Authentication
- Swagger/OpenAPI

### Frontend
- React 18
- TypeScript
- Vite
- TanStack Query (React Query)
- React Router DOM
- Tailwind CSS
- Axios
- Zod
- Lucide React (ícones)

### Ferramentas
- Docker & Docker Compose
- Git

## 📦 Instalação e Execução

### Pré-requisitos
- .NET 8 SDK
- Node.js 18+ e npm
- SQL Server (ou Docker)
- Git

### 1. Clonar o repositório

```bash
git clone <repository-url>
cd partnerMesh
```

### 2. Configurar o Backend

```bash
cd Api

# Configurar connection string no appsettings.json
# "Server=localhost;Database=PartnerMeshDb;Trusted_Connection=True;TrustServerCertificate=True;"

# Executar migrations
dotnet ef database update --project ../Infraestructure

# Executar a API
dotnet run
```

A API estará disponível em: `http://localhost:5000`

### 3. Configurar o Frontend

```bash
cd frontend

# Instalar dependências
npm install

# Executar em modo desenvolvimento
npm run dev
```

O frontend estará disponível em: `http://localhost:5174`

### 4. Usando Docker (Opcional)

```bash
# Na raiz do projeto
docker-compose up -d
```

Isso iniciará:
- SQL Server na porta 1433
- API na porta 5000
- Frontend na porta 5174

## 🗄️ Estrutura do Banco de Dados

### Principais Tabelas

- **Users**: Usuários do sistema
- **Vetores**: Entidades proprietárias de redes
- **Partners**: Parceiros da rede
- **BusinessTypes**: Tipos de negócios
- **Businesses**: Negócios fechados
- **Comissions**: Comissões geradas
- **ComissionPayments**: Pagamentos individuais de comissão
- **AuditLogs**: Logs de auditoria

## 🔐 Autenticação

O sistema utiliza JWT (JSON Web Tokens) para autenticação:

1. Login com credenciais
2. Recebimento de token JWT
3. Token enviado em todas as requisições via header `Authorization: Bearer <token>`
4. Permissões baseadas em roles (Vetor, Parceiro)

## 📊 Casos de Uso Implementados

### Autenticação (UC01)
- Login de usuário
- Alteração de senha

### Gestão de Parceiros (UC02-UC15)
- Criar parceiro
- Atualizar parceiro
- Listar parceiros com filtros
- Obter detalhes de parceiro
- Ativar/desativar parceiro

### Gestão de Negócios (UC20-UC35)
- Criar negócio
- Listar negócios
- Obter detalhes de negócio
- Cancelar negócio
- Listar pagamentos de comissão

### Tipos de Negócio (UC40-UC45)
- Criar tipo de negócio
- Listar tipos
- Desativar tipo

### Relatórios (UC50-UC60)
- Relatório financeiro
- Relatório de parceiros
- Relatório de negócios

### Auditoria (UC70-UC75)
- Consultar logs de auditoria
- Filtrar por usuário, entidade, período

## 🧪 Testes

```bash
# Backend (se houver testes implementados)
cd Api
dotnet test

# Frontend
cd frontend
npm run test
```

## 📝 Convenções de Código

### Backend
- Nomenclatura em inglês para código
- Português para comentários de documentação
- Use cases seguem padrão CQRS
- Entidades seguem DDD

### Frontend
- Componentes em PascalCase
- Hooks customizados começam com `use`
- Tipos TypeScript exportados de `types/`
- CSS com Tailwind (evitar CSS customizado)

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto é proprietário e confidencial.

## 👥 Equipe

- Desenvolvimento Backend: .NET Team
- Desenvolvimento Frontend: React Team
- Arquitetura: Solution Architects

## 📞 Suporte

Para suporte e dúvidas:
- Email: support@partnermesh.com
- Documentação: [Link para docs]
- Issues: [Link para issues do projeto]

---

**PartnerMesh** - Sistema de Gestão de Redes de Parceiros © 2026
