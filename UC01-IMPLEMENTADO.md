# UC-01 - Autenticar Usuário - Implementado

## 🎉 Implementação Concluída

O Use Case UC-01 (Autenticar Usuário) foi implementado com sucesso seguindo a **Arquitetura Hexagonal** e respeitando a estrutura existente do projeto.

### 📋 O que foi implementado:

#### **1. Domain Layer**
- ✅ Enum `PermissionEnum` com os perfis do sistema
- ✅ Entidade `User` com validação de senha via BCrypt
- ✅ Value Object `UserVetor` atualizado com propriedade `Active`

#### **2. Application Layer**
- ✅ Interface `IAuthenticateUserUseCase`
- ✅ DTOs: `AuthenticationRequest`, `AuthenticationResult`, `UserInfo`
- ✅ Use Case `AuthenticateUserUseCase` com todas as validações
- ✅ Interface `IUserRepository` 
- ✅ Interface `ITokenService`

#### **3. Infrastructure Layer**
- ✅ Implementação `TokenService` com geração de JWT e Refresh Token
- ✅ Implementação temporária `UserRepository` (em memória para testes)
- ✅ Configuração de serviços JWT

#### **4. API Layer**
- ✅ `AuthController` atualizado com endpoint `/api/auth/login`
- ✅ Configuração JWT no `Program.cs`
- ✅ Swagger configurado com autorização JWT
- ✅ Configurações JWT no `appsettings.json`

---

## 🧪 **Testando a API**

### **Usuários de Teste Disponíveis:**

1. **Admin Global**
   - Email: `admin@partnermesh.com`
   - Senha: `123456`
   - Perfil: AdminGlobal

2. **Admin Vetor**
   - Email: `adminvetor@partnermesh.com`
   - Senha: `123456` 
   - Perfil: AdminVetor

3. **Operador**
   - Email: `operador@partnermesh.com`
   - Senha: `123456`
   - Perfil: Operador

### **Endpoint de Login:**

**POST** `http://localhost:5251/api/auth/login`

**Request Body:**
```json
{
  "email": "admin@partnermesh.com",
  "password": "123456"
}
```

**Response de Sucesso:**
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

**Response de Erro:**
```json
{
  "isSuccess": false,
  "token": null,
  "refreshToken": null,
  "message": "Credenciais inválidas.",
  "user": null
}
```

---

## 🔒 **Regras de Validação Implementadas**

✅ **Email e senha obrigatórios**
✅ **Validação de usuário existente**
✅ **Verificação de usuário ativo**
✅ **Validação de senha com BCrypt**
✅ **Verificação de vetor ativo** (exceto para Admin Global)
✅ **Geração segura de JWT e Refresh Token**
✅ **Claims incluem**: UserId, Name, Email, Permission, VetorIds

---

## 📊 **Swagger/OpenAPI**

- **URL:** `http://localhost:5251`
- **Documentação interativa** com suporte a JWT
- **Botão "Authorize"** para testar endpoints autenticados

---

## ⚡ **Próximos Passos Sugeridos**

1. **UC-02** - Renovar Token (Refresh Token)
2. **UC-03** - Logout
3. **Integração com Entity Framework + PostgreSQL**
4. **Implementação dos próximos Use Cases (UC-10 a UC-15)**

---

## 🛠 **Tecnologias Utilizadas**

- **.NET 8** com Minimal APIs
- **JWT Bearer Authentication** 
- **BCrypt** para hash de senhas
- **Swagger/OpenAPI** para documentação
- **Arquitetura Hexagonal/Clean Architecture**

---

### 🔧 **Para Executar:**

```bash
cd c:\sdk\partnerMesh\Api
dotnet run
```

A API ficará disponível em: `http://localhost:5251`