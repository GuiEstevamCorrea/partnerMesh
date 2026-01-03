# PartnerMesh

Sistema de gestão de rede de parceiros com controle de negócios e distribuição automática de comissões.

## 🚀 Setup Completo do Projeto

### Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) - Para banco de dados
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) - Para o backend
- [Node.js 18+](https://nodejs.org/) - Para o frontend
- [Git](https://git-scm.com/) - Para controle de versão

### 📋 Opções de Instalação

Escolha uma das opções abaixo:

## 🐳 Opção 1: Setup com Docker (Recomendado)

Esta é a forma mais rápida e confiável de executar o projeto completo.

### 1️⃣ Clone e prepare o projeto

```bash
# Clone o repositório
git clone <URL_DO_SEU_REPOSITORIO>
cd partnerMesh

# Verifique se o Docker está rodando
docker --version
```

### 2️⃣ Configure o ambiente

```bash
# Crie as variáveis de ambiente (opcional)
cp Api/appsettings.Development.json.example Api/appsettings.Development.json
```

### 3️⃣ Execute com Docker Compose

```bash
# Execute todos os serviços (banco + backend + frontend)
docker-compose up -d

# Verifique se os containers estão rodando
docker-compose ps
```

### 4️⃣ Configure o banco de dados

```bash
# Execute as migrações do banco
docker-compose exec backend dotnet ef database update

# OU se preferir fazer localmente:
cd Api
dotnet ef database update
```

### 5️⃣ Acesse a aplicação

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000/api
- **Swagger**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433 (sa/PartnerMesh@2026)

---

## 🔧 Opção 2: Setup Manual (Desenvolvimento)

### 1️⃣ Banco de Dados

**Opção A: SQL Server com Docker**
```bash
# Execute apenas o SQL Server
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=PartnerMesh@2026" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

**Opção B: SQL Server LocalDB (Windows)**
```bash
# Instale o SQL Server Express LocalDB
# Connection String: Server=(localdb)\\mssqllocaldb;Database=PartnerMeshDb;Trusted_Connection=true;
```

### 2️⃣ Backend (.NET 8)

```bash
# Navegue para a pasta da API
cd Api

# Restaure as dependências
dotnet restore

# Configure a connection string em appsettings.Development.json
# "DefaultConnection": "Server=localhost,1433;Database=PartnerMeshDb;User Id=sa;Password=PartnerMesh@2026;TrustServerCertificate=true;"

# Execute as migrações
dotnet ef database update

# Execute o backend (porta 5000)
dotnet run
```

### 3️⃣ Frontend (React + TypeScript)

```bash
# Em um novo terminal, navegue para a pasta frontend
cd frontend

# Instale as dependências
npm install

# Execute o servidor de desenvolvimento (porta 5173)
npm run dev
```

### 4️⃣ Acesse a aplicação

- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:5000/api
- **Swagger**: http://localhost:5000/swagger

---

## 👤 Credenciais Padrão

```
Email: admin@partnermesh.com
Senha: Admin@123
```

---

## 🐳 Docker Compose - Serviços

O `docker-compose.yml` inclui:

### SQL Server
- **Container**: `sqlserver`
- **Porta**: 1433
- **Usuário**: sa
- **Senha**: PartnerMesh@2026
- **Volume**: Dados persistidos em `./data/mssql`

### Backend (.NET 8)
- **Container**: `backend`
- **Porta**: 5000
- **Health Check**: Endpoint `/health`
- **Dependências**: SQL Server

### Frontend (React)
- **Container**: `frontend`  
- **Porta**: 3000
- **Build**: Vite production build
- **Dependências**: Backend

---

## 🛠️ Comandos Úteis

### Docker

```bash
# Ver logs dos serviços
docker-compose logs -f

# Parar todos os serviços
docker-compose down

# Rebuilt completo
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d

# Executar comandos no backend
docker-compose exec backend dotnet ef migrations add NomeDaMigração
docker-compose exec backend dotnet ef database update

# Acessar o SQL Server
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P PartnerMesh@2026
```

### Desenvolvimento

```bash
# Backend - Adicionar migração
cd Api
dotnet ef migrations add <NomeDaMigração>
dotnet ef database update

# Frontend - Build de produção
cd frontend
npm run build

# Frontend - Executar testes
npm test

# Frontend - Executar linter
npm run lint
```

---

## 🔧 Configurações Avançadas

### Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto:

```env
# Banco de dados
DB_SERVER=localhost,1433
DB_NAME=PartnerMeshDb
DB_USER=sa
DB_PASSWORD=PartnerMesh@2026

# JWT
JWT_SECRET=SuaChaveSecretaAqui
JWT_EXPIRY=24

# API
API_URL=http://localhost:5000
FRONTEND_URL=http://localhost:3000

# Email (opcional)
SMTP_HOST=
SMTP_PORT=587
SMTP_USER=
SMTP_PASSWORD=
```

### appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=PartnerMeshDb;User Id=sa;Password=PartnerMesh@2026;TrustServerCertificate=true;"
  },
  "JwtSettings": {
    "Key": "SuaChaveSecretaAqui",
    "Issuer": "PartnerMesh",
    "Audience": "PartnerMesh",
    "ExpireMinutes": 1440
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

---

## 🚀 Deploy em Produção

### 1. Preparar ambiente
```bash
# Gerar build de produção
cd frontend
npm run build

# Publicar backend
cd ../Api
dotnet publish -c Release -o ./publish
```

### 2. Docker para produção
```bash
# Build das imagens
docker-compose -f docker-compose.prod.yml build

# Deploy
docker-compose -f docker-compose.prod.yml up -d
```

---

## 🛠️ Troubleshooting

### Problemas Comuns

#### Docker
```bash
# Erro de permissão (Linux/Mac)
sudo docker-compose up -d

# Limpar cache Docker
docker system prune -a

# Rebuild forçado
docker-compose build --no-cache

# Ver logs detalhados
docker-compose logs backend
docker-compose logs frontend
```

#### SQL Server
```bash
# Conectar ao SQL Server
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P PartnerMesh@2026

# Verificar se o banco foi criado
SELECT name FROM sys.databases;

# Backup do banco
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P PartnerMesh@2026 -Q "BACKUP DATABASE [PartnerMeshDb] TO DISK = '/var/opt/mssql/backups/partnermesh.bak'"
```

#### Backend (.NET)
```bash
# Verificar logs do backend
docker-compose logs -f backend

# Executar migração manualmente
cd Api
dotnet ef database update -v

# Limpar e rebuild
dotnet clean
dotnet build
```

#### Frontend (React)
```bash
# Limpar cache npm
npm cache clean --force

# Reinstalar dependências
rm -rf node_modules package-lock.json
npm install

# Build local
npm run build
```

### Portas em Uso
Se as portas padrão estiverem ocupadas, modifique no `docker-compose.yml`:

```yaml
services:
  frontend:
    ports:
      - "3001:3000"  # Mude para porta disponível
  backend:
    ports:
      - "5001:5000"  # Mude para porta disponível
  sqlserver:
    ports:
      - "1434:1433"  # Mude para porta disponível
```

---

## 👥 Desenvolvimento

### Git Flow
Este projeto usa Git Flow automático com GitHub Actions. Consulte [GITFLOW.md](GITFLOW.md) para detalhes.

### Branches
- `main` - Produção
- `develop` - Desenvolvimento
- `feature/*` - Novas funcionalidades
- `release/*` - Preparação para produção
- `hotfix/*` - Correções urgentes

### Comandos de Desenvolvimento

```bash
# Criar nova feature
git flow feature start nova-funcionalidade
git flow feature finish nova-funcionalidade

# Criar release
git flow release start v1.0.0
git flow release finish v1.0.0

# Hotfix
git flow hotfix start correcao-urgente
git flow hotfix finish correcao-urgente
```

### Estrutura do Banco

#### Principais Entidades
- `Users` - Usuários do sistema
- `Vectors` - Vetores (donos da rede)
- `Partners` - Parceiros da rede
- `Business` - Negócios fechados
- `ComissionPayments` - Comissões calculadas
- `AuditLogs` - Log de auditoria

### API Endpoints

#### Autenticação
- `POST /api/auth/login` - Login
- `POST /api/auth/refresh` - Renovar token

#### Parceiros
- `GET /api/partners` - Listar parceiros
- `POST /api/partners` - Criar parceiro
- `PUT /api/partners/{id}` - Atualizar parceiro

#### Negócios
- `GET /api/business` - Listar negócios
- `POST /api/business` - Criar negócio
- `PUT /api/business/{id}` - Atualizar negócio

#### Pagamentos
- `GET /api/payments` - Listar pagamentos
- `POST /api/payments/process` - Processar pagamentos

---

## 📊 Monitoramento

### Health Checks
- Backend: http://localhost:5000/health
- SQL Server: Query `SELECT 1`

### Logs
```bash
# Todos os serviços
docker-compose logs -f

# Apenas backend
docker-compose logs -f backend

# Apenas frontend
docker-compose logs -f frontend
```

### Métricas
- Acompanhe os logs do Entity Framework para queries lentas
- Monitore uso de memória dos containers
- Verifique espaço em disco para o SQL Server

---

## 🔒 Segurança

### Produção
- Altere todas as senhas padrão
- Use HTTPS com certificados SSL
- Configure firewall adequadamente
- Mantenha backups regulares
- Use variáveis de ambiente para secrets

### Variáveis Sensíveis
```env
DB_PASSWORD=SuaSenhaSuperSegura123!
JWT_SECRET=ChaveJWTSuperSecretaComPeloMenos256Bits
```

---

## 📚 Documentação Adicional

- [Git Flow Guide](GITFLOW.md) - Guia completo do Git Flow
- [GitHub Actions](/.github/workflows/README.md) - Workflows automatizados
- [API Documentation](http://localhost:5000/swagger) - Documentação da API
- [Components Guide](frontend/COMPONENTS.md) - Guia dos componentes

---

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma feature branch (`git flow feature start amazing-feature`)
3. Commit suas mudanças (`git commit -m 'feat: add amazing feature'`)
4. Push para a branch (`git push origin feature/amazing-feature`)
5. Abra um Pull Request

---

## 📄 Scripts de Setup

### Linux/Mac
```bash
chmod +x setup.sh
./setup.sh
```

### Windows
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
.\setup.ps1
```

---

---

### 1️⃣ Backend Setup

```bash
# Navigate to Api folder
cd Api

# Restore dependencies
dotnet restore

# Update database (creates schema and migrations)
dotnet ef database update

# Run the API (port 5000)
dotnet run
```

### 2️⃣ Frontend Setup

```bash
# Navigate to frontend folder
cd frontend

# Install dependencies
npm install

# Run development server (port 5173)
npm run dev
```

### 3️⃣ Access the Application

- Frontend: http://localhost:5173
- Backend API: http://localhost:5000/api

**Default Login:**
- Email: admin@partnermesh.com
- Password: Admin@123

---

## �📋 About the Project

PartnerMesh is a complete platform for managing multi-level partner networks, allowing control of deals closed between partners and automatic distribution of commissions through a hierarchical structure of up to 3 levels.

### Core Concepts

- **Vector**: Entity that owns the partner network. Each vector has its own independent network.
- **Partner**: Network member who can close deals and recommend new partners.
- **Business**: Transaction between two network partners, automatically generating commissions.
- **Commission**: Value automatically calculated (10% of the business) and distributed up to 3 levels above the involved partners.

## 🏗️ Architecture

### Backend (.NET 8)

```
Api/                      # Primary Adapter - REST Controllers and endpoints
Application/              # Application Layer - Use Cases (Ports)
  ├── UseCases/          # Implemented use cases
  ├── Interfaces/        # Ports (Repository and Service Interfaces)
  └── DTOs/              # Data Transfer Objects
Domain/                   # Hexagon Core - Entities and business rules
  ├── Entities/          # Domain models
  ├── ValueObjects/      # Value objects
  ├── ValueTypes/        # Enums and configurations
  └── Extensions/        # Domain extensions
Infrastructure/           # Secondary Adapter - External implementations
  ├── Data/              # EF Core Context
  ├── Repositories/      # Repository Port implementations
  └── Services/          # External Service Port implementations
```

**Architecture**: Hexagonal (Ports and Adapters)
- **Core (Domain)**: Isolated and independent business rules
- **Ports (Application/Interfaces)**: Contracts defining communication
- **Adapters**: 
  - **Primary (Api)**: REST controllers receiving requests
  - **Secondary (Infrastructure)**: Persistence and external service implementations
- **Principles**:
  - Clear separation of concerns
  - Domain-Driven Design (DDD)
  - Dependency Inversion Principle (DIP)
  - Repository Pattern with Entity Framework Core
  - CQRS for complex queries

### Frontend (React + TypeScript)

```
src/
  ├── api/               # Axios configuration and endpoints
  ├── components/        # Reusable components
  ├── pages/             # Application pages
  ├── hooks/             # Custom hooks
  ├── types/             # TypeScript types
  ├── utils/             # Utilities
  └── contexts/          # React contexts
```

**Frontend Stack**:
- React 18 with TypeScript
- Vite for build
- React Router for navigation
- React Query (TanStack Query) for state management
- Tailwind CSS for styling
- Zod for form validation

## 🛠️ Technologies

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
- Lucide React (icons)

### Tools
- Docker & Docker Compose
- Git

---

**PartnerMesh** - Partner Network Management System © 2026