# Implementação SQL Server com Entity Framework Core

## ✅ Status: COMPLETO

Implementação finalizada em 24/12/2025 - Sistema PartnerMesh com persistência em SQL Server usando Entity Framework Core 8.0.

---

## 📋 Resumo da Implementação

### 1. Pacotes NuGet Instalados

**Infraestructure.csproj:**
- `Microsoft.EntityFrameworkCore` 8.0.0
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.0
- `Microsoft.EntityFrameworkCore.Tools` 8.0.0
- `Microsoft.EntityFrameworkCore.Design` 8.0.0

**Api.csproj:**
- `Microsoft.EntityFrameworkCore.Design` 8.0.0

### 2. DbContext Criado

**Arquivo:** `Infraestructure/Data/PartnerMeshDbContext.cs`

**DbSets Configurados:**
- ✅ Users (com UserVetores)
- ✅ Vetores (com Partners e UserVetores)
- ✅ Partners (auto-referência para Recomendador)
- ✅ BusinessTypes
- ✅ Businesses (com Comissions 1:1)
- ✅ Comissions (com ComissionPayments 1:N)
- ✅ ComissionPayments
- ✅ AuditLogs (com índices em CreatedAt e Entity/EntityId)
- ✅ RefreshTokens (índice único em Token)

**Relacionamentos EF Core:**
```csharp
// User ↔ UserVetor ↔ Vetor (muitos-para-muitos)
User.UserVetores → UserVetor → Vetor.UserVetores

// Partner → Vetor (muitos-para-um)
Partner.Vetor → Vetor.Partners

// Partner → Partner (auto-referência)
Partner.Recommender → Partner.Recommended

// Business → BusinessType (muitos-para-um)
// Business → Partner (muitos-para-um)
// Business ↔ Comission (um-para-um)
Business.Comissao ↔ Comission.Bussiness

// Comission → ComissionPayment (um-para-muitos)
Comission.Pagamentos → ComissionPayment.Comission

// RefreshToken → User (muitos-para-um)
RefreshToken.User
```

### 3. Repositórios Migrados

Todos os 8 repositórios foram migrados de **in-memory** para **Entity Framework Core**:

| Repositório | Arquivo | Status |
|-------------|---------|--------|
| UserRepository | `Repositories/UserRepository.cs` | ✅ Migrado |
| VetorRepository | `Repositories/VetorRepository.cs` | ✅ Migrado |
| PartnerRepository | `Repositories/PartnerRepository.cs` | ✅ Migrado |
| BusinessTypeRepository | `Repositories/BusinessTypeRepository.cs` | ✅ Migrado |
| BusinessRepository | `Repositories/BusinessRepository.cs` | ✅ Migrado |
| CommissionRepository | `Repositories/CommissionRepository.cs` | ✅ Migrado |
| AuditLogRepository | `Repositories/AuditLogRepository.cs` | ✅ Migrado |
| RefreshTokenRepository | `Repositories/RefreshTokenRepository.cs` | ✅ Migrado |

**Recursos EF Core Implementados:**
- ✅ `Include()` e `ThenInclude()` para eager loading
- ✅ `AsQueryable()` para consultas LINQ
- ✅ `AddAsync()`, `Update()`, `Remove()` para operações CRUD
- ✅ `SaveChangesAsync()` para persistência
- ✅ `FirstOrDefaultAsync()`, `AnyAsync()`, `CountAsync()` para queries assíncronas
- ✅ Filtros complexos com `Where()`, `OrderBy()`, `Skip()`, `Take()`
- ✅ Eager loading de relacionamentos navegacionais

### 4. Connection Strings Configuradas

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PartnerMeshDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**appsettings.Development.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PartnerMeshDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

**Bancos de Dados:**
- **Produção:** `PartnerMeshDb`
- **Desenvolvimento:** `PartnerMeshDb_Dev`

**Autenticação:** Windows Authentication (`Trusted_Connection=True`)

### 5. Configuração no Program.cs

**Registro do DbContext:**
```csharp
builder.Services.AddDbContext<PartnerMeshDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

**Data Seeding Automático:**
```csharp
using (var scope = app.Services.CreateScope())
{
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}
```

### 6. Migration Inicial

**Comando Executado:**
```bash
dotnet ef migrations add InitialCreate --project Infraestructure --startup-project Api
```

**Arquivos Gerados:**
- `Infraestructure/Migrations/xxxxxx_InitialCreate.cs`
- `Infraestructure/Migrations/PartnerMeshDbContextModelSnapshot.cs`

**Status:** ✅ Migration criada com sucesso

### 7. Data Seeder

**Arquivo:** `Infraestructure/Data/DatabaseSeeder.cs`

**Dados Iniciais Populados:**

| Tipo | Nome | Email | Senha | Permissão |
|------|------|-------|-------|-----------|
| Usuário | Admin Global | admin@partnermesh.com | 123456 | AdminGlobal |
| Usuário | Admin Vetor | adminvetor@partnermesh.com | 123456 | AdminVetor |
| Usuário | Operador Sistema | operador@partnermesh.com | 123456 | Operador |
| Vetor | Vetor Principal | vetor@partnermesh.com | - | - |
| Vetor | Vetor Secundário | vetor2@partnermesh.com | - | - |

**Funcionalidades:**
- ✅ Aplica migrations automaticamente (`Database.MigrateAsync()`)
- ✅ Verifica se dados já existem (evita duplicação)
- ✅ Popula vetores antes de usuários (respeita FK)
- ✅ Cria usuários com perfis diferentes
- ✅ Associa usuários aos vetores via `UserVetor`
- ✅ Loga credenciais no console durante inicialização
- ✅ Tratamento de erros com logs

---

## 🚀 Como Usar

### 1. Pré-requisitos

- ✅ SQL Server instalado e rodando (LocalDB, Express ou Full)
- ✅ .NET 8.0 SDK instalado
- ✅ EF Core CLI Tools instalado:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### 2. Aplicar Migrations

**Opção A: Automático (na inicialização da API)**
```bash
cd Api
dotnet run
```
O `DatabaseSeeder` aplicará as migrations automaticamente.

**Opção B: Manual (via CLI)**
```bash
cd Api
dotnet ef database update --project ../Infraestructure --startup-project .
```

### 3. Verificar Banco de Dados

**SQL Server Management Studio (SSMS):**
```sql
-- Conectar ao servidor: localhost
USE PartnerMeshDb_Dev;

-- Verificar tabelas criadas
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';

-- Verificar dados iniciais
SELECT * FROM Users;
SELECT * FROM Vetores;
```

**Azure Data Studio:**
```sql
-- Mesmas queries acima
```

### 4. Testar API

**Swagger UI:**
```
http://localhost:5000/swagger
```

**Endpoint de Login:**
```bash
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "admin@partnermesh.com",
  "password": "123456"
}
```

**Resposta Esperada:**
```json
{
  "userId": "guid",
  "name": "Admin Global",
  "email": "admin@partnermesh.com",
  "accessToken": "eyJhbGci...",
  "refreshToken": "refresh-token-guid",
  "permission": "AdminGlobal",
  "vetorId": null
}
```

---

## 📊 Estrutura do Banco de Dados

### Tabelas Criadas

| Tabela | Chave Primária | Relacionamentos |
|--------|---------------|-----------------|
| Users | Id (Guid) | → UserVetores (1:N) |
| UserVetores | UserId + VetorId | ← Users, Vetores (N:N) |
| Vetores | Id (Guid) | → UserVetores, Partners (1:N) |
| Partners | Id (Guid) | → Vetor, Recommender, Recommended (N:1, self-ref) |
| BusinessTypes | Id (Guid) | → Businesses (1:N) |
| Businesses | Id (Guid) | → Partner, BusinessType, Comissao (N:1, 1:1) |
| Comissions | Id (Guid) | → Bussiness, ComissionPayments (1:1, 1:N) |
| ComissionPayments | Id (Guid) | → Comission (N:1) |
| AuditLogs | Id (Guid) | Nenhum (tabela de log) |
| RefreshTokens | Id (Guid) | → User (N:1) |

### Índices Criados

| Tabela | Índice | Tipo |
|--------|--------|------|
| Users | Email | Único |
| RefreshTokens | Token | Único |
| AuditLogs | CreatedAt | Não-único |
| AuditLogs | Entity + EntityId | Composto |

### Constraints

- ✅ `DeleteBehavior.Cascade` em UserVetores ↔ Users/Vetores
- ✅ `DeleteBehavior.Restrict` em Partners → Vetor/Recommender
- ✅ `DeleteBehavior.Cascade` em Comissions ↔ Businesses
- ✅ `DeleteBehavior.Cascade` em ComissionPayments → Comissions
- ✅ `DeleteBehavior.Cascade` em RefreshTokens → Users

---

## 🔧 Comandos Úteis do EF Core

### Migrations

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration --project Infraestructure --startup-project Api

# Aplicar todas migrations
dotnet ef database update --project Infraestructure --startup-project Api

# Reverter última migration
dotnet ef migrations remove --project Infraestructure --startup-project Api

# Reverter para migration específica
dotnet ef database update NomeDaMigration --project Infraestructure --startup-project Api

# Gerar script SQL da migration
dotnet ef migrations script --project Infraestructure --startup-project Api --output migration.sql

# Listar migrations
dotnet ef migrations list --project Infraestructure --startup-project Api
```

### Database

```bash
# Criar banco de dados
dotnet ef database update --project Infraestructure --startup-project Api

# Deletar banco de dados
dotnet ef database drop --project Infraestructure --startup-project Api --force

# Verificar conexão
dotnet ef dbcontext info --project Infraestructure --startup-project Api
```

### DbContext

```bash
# Gerar diagrama do modelo
dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer --project Infraestructure
```

---

## ⚙️ Configurações Avançadas

### Connection String para Docker/Azure

**Docker SQL Server:**
```json
"DefaultConnection": "Server=localhost,1433;Database=PartnerMeshDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
```

**Azure SQL Database:**
```json
"DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=PartnerMeshDb;Persist Security Info=False;User ID=admin;Password=password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

### Habilitar Logging SQL

**appsettings.Development.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information",
      "Microsoft.EntityFrameworkCore.Infrastructure": "Information"
    }
  }
}
```

### Pool de Conexões

**Program.cs:**
```csharp
builder.Services.AddDbContext<PartnerMeshDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => 
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(60);
        }));
```

---

## 🐛 Troubleshooting

### Erro: "Cannot open database"

**Solução:**
```bash
# Verificar se SQL Server está rodando
Get-Service MSSQLSERVER

# Criar banco manualmente
sqlcmd -S localhost -Q "CREATE DATABASE PartnerMeshDb_Dev"

# Aplicar migrations
dotnet ef database update
```

### Erro: "Login failed for user"

**Solução:**
- Verificar connection string
- Usar Windows Authentication: `Trusted_Connection=True`
- Ou criar SQL Login:
  ```sql
  CREATE LOGIN partnermesh WITH PASSWORD = 'Strong@Pass123';
  CREATE USER partnermesh FOR LOGIN partnermesh;
  ALTER ROLE db_owner ADD MEMBER partnermesh;
  ```

### Erro: "A connection was successfully established, but then an error occurred"

**Solução:**
- Adicionar `TrustServerCertificate=True` na connection string
- Ou instalar certificado SSL no SQL Server

### Banco não atualiza após mudanças

**Solução:**
```bash
# Remover migration
dotnet ef migrations remove

# Recriar migration
dotnet ef migrations add NovoNome

# Aplicar
dotnet ef database update
```

---

## 📈 Próximos Passos (Opcional)

### 1. Performance

- [ ] Adicionar índices compostos em queries frequentes
- [ ] Implementar `AsNoTracking()` em queries read-only
- [ ] Configurar lazy loading com proxies
- [ ] Implementar paginação com `Skip()` e `Take()`
- [ ] Cache de queries com Redis

### 2. Segurança

- [ ] Migrar de Windows Auth para SQL Auth em produção
- [ ] Implementar Row-Level Security
- [ ] Criptografar dados sensíveis (senhas já estão com BCrypt)
- [ ] Adicionar auditoria automática com EF Core Interceptors

### 3. Monitoramento

- [ ] Integrar com Application Insights
- [ ] Adicionar logging de slow queries
- [ ] Configurar alertas de deadlock
- [ ] Implementar health checks do banco

### 4. Testes

- [ ] Criar testes de integração com TestContainers
- [ ] Implementar testes de carga com k6/JMeter
- [ ] Validar migrations com snapshots
- [ ] Testar rollback de migrations

---

## 📚 Recursos

### Documentação Oficial
- [EF Core 8.0 Docs](https://learn.microsoft.com/en-us/ef/core/)
- [SQL Server Docs](https://learn.microsoft.com/en-us/sql/sql-server/)
- [Connection Strings](https://www.connectionstrings.com/sql-server/)

### Ferramentas
- **SQL Server Management Studio (SSMS):** https://aka.ms/ssmsfullsetup
- **Azure Data Studio:** https://aka.ms/azuredatastudio
- **EF Core Power Tools:** VS Extension para scaffolding

### Comandos Rápidos
```bash
# Verificar versão EF CLI
dotnet ef --version

# Instalar/Atualizar EF CLI
dotnet tool update --global dotnet-ef

# Listar providers disponíveis
dotnet ef dbcontext list
```

---

## ✅ Checklist Final

- [x] Pacotes NuGet instalados
- [x] DbContext criado com todos os DbSets
- [x] Configurações de entidades (Fluent API)
- [x] Connection strings configuradas
- [x] DbContext registrado no DI
- [x] 8 repositórios migrados para EF Core
- [x] Migration inicial criada
- [x] DataSeeder implementado
- [x] Seeding automático no Program.cs
- [x] Build com sucesso (0 erros)
- [x] Documentação completa
- [x] Credenciais de teste documentadas

---

## 🎉 Conclusão

✅ **Implementação SQL Server + Entity Framework Core COMPLETA!**

**Status:** Pronto para testes e desenvolvimento

**Próximo Passo:** Rodar a aplicação e testar endpoints via Swagger

**Comando para iniciar:**
```bash
cd c:\sdk\partnerMesh\Api
dotnet run
```

**Acesse:** http://localhost:5000/swagger

---

**Desenvolvido em:** 24/12/2025  
**Tecnologias:** .NET 8.0, EF Core 8.0, SQL Server 2019+  
**Arquitetura:** Clean Architecture (Hexagonal)
