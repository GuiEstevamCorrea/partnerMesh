# PartnerMesh

Partner network management system with business control and automatic commission distribution.

## � Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- SQL Server (LocalDB or Express)

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
