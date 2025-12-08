# UC-02 - Renovar Token - Implementado

## 🎉 Implementação Concluída

O Use Case UC-02 (Renovar Token) foi implementado com sucesso seguindo a **Arquitetura Hexagonal** e integrado à estrutura existente do projeto.

### 📋 O que foi implementado:

#### **1. Domain Layer**
- ✅ Entidade `RefreshToken` com validações e métodos de controle
- ✅ Propriedades: Token, UserId, ExpiresAt, IsRevoked, IsUsed
- ✅ Métodos: `IsValid()`, `MarkAsUsed()`, `Revoke()`

#### **2. Application Layer**
- ✅ Interface `IRefreshTokenUseCase`
- ✅ DTOs: `RefreshTokenRequest`, `RefreshTokenResult`
- ✅ Use Case `RefreshTokenUseCase` com todas as validações
- ✅ Interface `IRefreshTokenRepository`
- ✅ Atualização do `IUserRepository` com método `GetByIdAsync()`

#### **3. Infrastructure Layer**
- ✅ Implementação `RefreshTokenRepository` (em memória para testes)
- ✅ Métodos: `GetByTokenAsync()`, `SaveAsync()`, `RevokeAllByUserIdAsync()`
- ✅ Atualização do `UserRepository` com busca por ID

#### **4. API Layer**
- ✅ Endpoint `POST /api/auth/refresh` no `AuthController`
- ✅ Documentação Swagger atualizada
- ✅ Integração com o login existente (UC-01)

#### **5. Integração com UC-01**
- ✅ `AuthenticateUserUseCase` agora salva refresh tokens
- ✅ Dependências registradas no `Program.cs`

---

## 🧪 **Testando o Refresh Token**

### **Fluxo Completo de Teste:**

#### **1. Fazer Login (UC-01)**
**POST** `http://localhost:5251/api/auth/login`

```json
{
  "email": "admin@partnermesh.com",
  "password": "123456"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "8F2E4A1B5D6C7E9F...",
  "message": null,
  "user": {
    "id": "12345678-1234-1234-1234-123456789abc",
    "name": "Admin Global",
    "email": "admin@partnermesh.com",
    "permission": "AdminGlobal",
    "vetorIds": []
  }
}
```

#### **2. Renovar Token (UC-02)**
**POST** `http://localhost:5251/api/auth/refresh`

```json
{
  "refreshToken": "8F2E4A1B5D6C7E9F..."
}
```

**Response de Sucesso:**
```json
{
  "isSuccess": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...", // NOVO TOKEN
  "refreshToken": "9G3F5B2C6E8D0A1E...", // NOVO REFRESH TOKEN
  "message": null,
  "user": {
    "id": "12345678-1234-1234-1234-123456789abc",
    "name": "Admin Global",
    "email": "admin@partnermesh.com",
    "permission": "AdminGlobal",
    "vetorIds": []
  }
}
```

**Response de Erro:**
```json
{
  "isSuccess": false,
  "token": null,
  "refreshToken": null,
  "message": "Refresh token inválido.",
  "user": null
}
```

---

## 🔒 **Regras de Validação Implementadas**

✅ **Refresh token obrigatório**
✅ **Verificação de existência do refresh token**
✅ **Validação de expiração** (30 dias)
✅ **Verificação se não foi revogado**
✅ **Verificação se não foi usado** (one-time use)
✅ **Validação de usuário ativo**
✅ **Verificação de vetor ativo** (exceto Admin Global)
✅ **Invalidação do refresh token usado**
✅ **Geração de novos tokens** (JWT + Refresh)

---

## 🔄 **Ciclo de Vida do Refresh Token**

1. **Criação**: Gerado durante login (UC-01)
2. **Armazenamento**: Salvo com validade de 30 dias
3. **Uso**: Utilizado uma única vez para renovar tokens
4. **Invalidação**: Marcado como usado após renovação
5. **Expiração**: Automática após 30 dias

---

## ⚡ **Recursos Implementados**

### **Segurança:**
- ✅ One-time use para refresh tokens
- ✅ Expiração automática (30 dias)
- ✅ Revogação manual possível
- ✅ Validação de usuário ativo

### **Flexibilidade:**
- ✅ Suporte a múltiplos vetores
- ✅ Diferentes perfis de usuário
- ✅ Regeneração automática de tokens

### **Integração:**
- ✅ Funciona com UC-01 existente
- ✅ Pronto para UC-03 (Logout)
- ✅ Estrutura preparada para Entity Framework

---

## 📊 **Swagger/OpenAPI**

- **URL:** `http://localhost:5251`
- **Novo endpoint documentado**: `POST /api/auth/refresh`
- **Suporte a teste interativo**

---

## ⚡ **Próximos Passos Sugeridos**

1. **UC-03** - Logout (Revogar Refresh Token)
2. **UC-10 a UC-15** - Gestão de Usuários
3. **UC-20 a UC-24** - Gestão de Vetores
4. **Integração com Entity Framework + PostgreSQL**

---

### 🔧 **Para Executar:**

```bash
cd c:\sdk\partnerMesh\Api
dotnet run
```

A API ficará disponível em: `http://localhost:5251`

### 🧪 **Teste Rápido:**

1. Fazer login → pegar refresh token
2. Usar refresh token → receber novos tokens
3. Usar refresh token novamente → erro (já foi usado)

---

**✅ UC-02 implementado com sucesso!** O sistema agora suporta renovação segura de tokens JWT.