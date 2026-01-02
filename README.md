# PartnerMesh

Partner network management system with business control and automatic commission distribution.

## 📋 About the Project

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

## 🚀 Features

### Partner Management
- ✅ Partner registration with hierarchical structure
- ✅ New partner recommendations (up to 3 levels)
- ✅ Partner activation/deactivation
- ✅ Recommendation tree visualization

### Business Management
- ✅ Deal registration between two partners
- ✅ Automatic commission calculation (10% of value)
- ✅ Intelligent distribution up to 3 levels above
- ✅ Validation to prevent deals between invalid partners
- ✅ Business cancellation
- ✅ Complete transaction history

### Commission Distribution

**Distribution Rule:**
- Partners involved in the deal **DO NOT receive commission**
- Commission distributed only to levels above (maximum 3 levels)
- Distribution percentages:
  - **Level 1** (direct recommender): 50%
  - **Level 2** (second level): 30%
  - **Level 3** (third level): 20%
  - **Vector**: Receives remaining balance (when there are fewer than 3 levels)

**Examples:**

```
$1,000.00 Deal → Total Commission: $100.00

Scenario 1: Partner with 3 levels above
├─ Level 1: $50.00 (50%)
├─ Level 2: $30.00 (30%)
└─ Level 3: $20.00 (20%)

Scenario 2: Partner with 1 level above
├─ Level 1: $50.00 (50%)
└─ Vector: $50.00 (50%)

Scenario 3: Partner without recommender
└─ Vector: $100.00 (100%)
```

### Reports and Dashboards
- ✅ Financial report with advanced filters
- ✅ Partners report by level
- ✅ Real-time metrics dashboard
- ✅ Data export

### Audit
- ✅ Complete log of all operations
- ✅ User change tracking
- ✅ Action history query

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

## 📦 Installation and Execution

### Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm
- SQL Server (or Docker)
- Git

### 1. Clone the repository

```bash
git clone <repository-url>
cd partnerMesh
```

### 2. Configure Backend

```bash
cd Api

# Configure connection string in appsettings.json
# "Server=localhost;Database=PartnerMeshDb;Trusted_Connection=True;TrustServerCertificate=True;"

# Run migrations
dotnet ef database update --project ../Infraestructure

# Run API
dotnet run
```

The API will be available at: `http://localhost:5000`

### 3. Configure Frontend

```bash
cd frontend

# Install dependencies
npm install

# Run in development mode
npm run dev
```

The frontend will be available at: `http://localhost:5174`

### 4. Using Docker (Optional)

```bash
# At project root
docker-compose up -d
```

This will start:
- SQL Server on port 1433
- API on port 5000
- Frontend on port 5174

## 🗄️ Database Structure

### Main Tables

- **Users**: System users
- **Vetores**: Network owner entities
- **Partners**: Network partners
- **BusinessTypes**: Business types
- **Businesses**: Closed deals
- **Comissions**: Generated commissions
- **ComissionPayments**: Individual commission payments
- **AuditLogs**: Audit logs

## 🔐 Authentication

The system uses JWT (JSON Web Tokens) for authentication:

1. Login with credentials
2. Receive JWT token
3. Token sent in all requests via `Authorization: Bearer <token>` header
4. Role-based permissions (Vector, Partner)

## 📊 Implemented Use Cases

### Authentication (UC01)
- User login
- Password change

### Partner Management (UC02-UC15)
- Create partner
- Update partner
- List partners with filters
- Get partner details
- Activate/deactivate partner

### Business Management (UC20-UC35)
- Create business
- List businesses
- Get business details
- Cancel business
- List commission payments

### Business Types (UC40-UC45)
- Create business type
- List types
- Deactivate type

### Reports (UC50-UC60)
- Financial report
- Partners report
- Business report

### Audit (UC70-UC75)
- Query audit logs
- Filter by user, entity, period

## 🧪 Testing

```bash
# Backend (if tests are implemented)
cd Api
dotnet test

# Frontend
cd frontend
npm run test
```

## 📝 Code Conventions

### Backend
- English naming for code
- Portuguese for documentation comments
- Use cases follow CQRS pattern
- Entities follow DDD

### Frontend
- Components in PascalCase
- Custom hooks start with `use`
- TypeScript types exported from `types/`
- CSS with Tailwind (avoid custom CSS)

## 🤝 Contributing

1. Fork the project
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is proprietary and confidential.

## 👥 Team

- Backend Development: .NET Team
- Frontend Development: React Team
- Architecture: Solution Architects

## 📞 Support

For support and questions:
- Email: support@partnermesh.com
- Documentation: [Link to docs]
- Issues: [Link to project issues]

---

**PartnerMesh** - Partner Network Management System © 2026
