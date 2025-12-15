<!-- Baseado no documento ANALISE-TODOS-TIPIFICACAO.md, e na arquitetura do projeto, implemente o ponto -->

# **ANÁLISE DE CÓDIGO - TODOs E TIPIFICAÇÃO**

## 📋 **TODOs Identificados**

### **1. Autorização e Segurança - OK**
| Arquivo | Linha | TODO | Prioridade |
|---------|-------|------|------------|
| `Api/Controllers/AuditQueryController.cs` | 14 | Implementar verificação específica de Admin Global | **ALTA** |
| `Api/Controllers/AuditQueryController.cs` | 35, 71 | Verificar se usuário é Admin Global | **ALTA** |
| `Api/Controllers/BusinessController.cs` | 291, 353 | Extrair userId do token JWT | **ALTA** |
| `Api/Controllers/FinancialController.cs` | 48 | Extrair userId do token JWT | **ALTA** |
| `Api/Controllers/PaymentsController.cs` | 58, 102 | Extrair userId do token JWT | **ALTA** |
| `Api/Controllers/ReportsController.cs` | 44 | Extrair userId do token JWT | **ALTA** |

### **2. Implementações Futuras - OK**
| Arquivo | Linha | TODO | Prioridade |
|---------|-------|------|------------|
| `Application/UseCases/PartnersReport/PartnersReportUseCase.cs` | 34 | Implementar validação de acesso por vetor | **MÉDIA** |
| `Application/UseCases/GetBusinessPayments/GetBusinessPaymentsUseCase.cs` | 47 | Implementar validação de acesso por vetor | **MÉDIA** |
| `Application/UseCases/DeactivateBusinessType/DeactivateBusinessTypeUseCase.cs` | 52 | Verificar negócios ativos antes da desativação | **MÉDIA** |

### **3. Relacionamentos e Dados - OK**
| Arquivo | Linha | TODO | Prioridade |
|---------|-------|------|------------|
| `Infraestructure/Repositories/CommissionRepository.cs` | 93 | Implementar filtro por vetorId | **BAIXA** |
| `Application/UseCases/ListPayments/ListPaymentsUseCase.cs` | 92, 93 | Implementar relação direta com vetor | **BAIXA** |
| `Application/UseCases/GetVetorById/GetVetorByIdUseCase.cs` | 108, 111 | Implementar contadores de partners e businesses | **BAIXA** |

---

## 🏷️ **OPORTUNIDADES DE TIPIFICAÇÃO**

### **1. STATUS - CRIAÇÃO DE ENUMS - OK**

#### **StatusEntity (Para Entidades Gerais)**
```csharp
public enum StatusEntity
{
    Ativo = 1,
    Inativo = 2
}
```
**Uso atual:** Strings "ativo"/"inativo" em múltiplas entidades
**Arquivos afetados:** User.Active (bool), Partner.Active (bool), Vetor.Active (bool)

#### **BusinessStatus (Para Negócios)**
```csharp
public enum BusinessStatus
{
    Ativo = 1,
    Cancelado = 2
}
```
**Uso atual:** String em `Domain/Entities/Bussiness.cs` linha 15
**Implementação:** Substituir `string Status` por `BusinessStatus Status`

#### **PaymentStatus (Para Pagamentos de Comissão)**
```csharp
public enum PaymentStatus
{
    APagar = 1,
    Pago = 2,
    Cancelado = 3
}
```
**Uso atual:** Strings hardcoded em `Domain/ValueObjects/ComissionPayment.cs`
**Constantes atuais:**
- `APagar = "a_pagar"`
- `Pago = "pago"`
- `Cancelado = "cancelado"`

### **2. TIPOS DE PAGAMENTO - CRIAÇÃO DE ENUMS - OK**

#### **PaymentType (Para Tipos de Pagamento)**
```csharp
public enum PaymentType
{
    Vetor = 1,
    Recomendador = 2,
    Participante = 3,
    Intermediario = 4
}
```
**Uso atual:** Strings em `Domain/ValueObjects/ComissionPayment.cs`
**Constantes atuais:**
- `VetorPagamento = "vetor"`
- `RecomendadorPagamento = "recomendador"`
- `ParticipantePagamento = "participante"`
- `IntermediarioPagamento = "intermediario"`

### **3. AUDITORIA - JÁ TIPIFICADA (✅) - OK**

As constantes de auditoria já estão bem estruturadas em:
- `Application/UseCases/LogAudit/DTO/LogAuditResult.cs`
- **AuditActions:** LOGIN, LOGOUT, CREATE, UPDATE, DELETE, etc.
- **AuditEntities:** User, Partner, Business, Commission, etc.

**Recomendação:** Converter para enums para melhor tipo-segurança:

#### **AuditAction (Enum para Ações)**
```csharp
public enum AuditAction
{
    Login,
    Logout,
    RefreshToken,
    PasswordChange,
    Create,
    Update,
    Delete,
    Activate,
    Deactivate,
    BusinessCreate,
    BusinessUpdate,
    BusinessCancel,
    CommissionPayment,
    ReportPartners,
    ReportFinancial,
    ReportBusiness,
    ViewSensitiveData,
    ExportData
}
```

#### **AuditEntityType (Enum para Entidades)**
```csharp
public enum AuditEntityType
{
    User,
    Vetor,
    Partner,
    BusinessType,
    Business,
    Commission,
    CommissionPayment,
    System,
    Report
}
```

### **4. CAMPOS STRING QUE DEVERIAM SER TIPIFICADOS - OK**

#### **Ordenação e Filtros**
**Arquivo:** `Infraestructure/Repositories/PartnerRepository.cs`
**Problema:** Strings hardcoded para ordenação: `"name"`, `"createdat"`, `"email"`

**Solução:** Criar enum SortField
```csharp
public enum PartnerSortField
{
    Name,
    CreatedAt,
    Email,
    Active
}
```

#### **Direção de Ordenação**
**Problema:** Strings "ASC"/"DESC" em múltiplos repositórios
**Solução:** 
```csharp
public enum SortDirection
{
    Ascending,
    Descending
}
```

### **5. VALIDAÇÕES E REGRAS DE NEGÓCIO - OK**

#### **Níveis de Recomendação**
**Arquivo:** Múltiplos (lógica de 3 níveis de parceiros)
**Problema:** Valores hardcoded para níveis (1, 2, 3)

**Solução:**
```csharp
public enum RecommendationLevel
{
    Level1 = 1, // Direto
    Level2 = 2, // Segundo nível  
    Level3 = 3  // Terceiro nível
}
```

#### **Percentuais de Comissão**
**Problema:** Valores hardcoded (10%, distribuição)
**Solução:** Criar classe de configuração tipada

```csharp
public class CommissionSettings
{
    public decimal TotalPercentage { get; } = 0.10m; // 10%
    public decimal Level1Percentage { get; } = 0.05m; // 5%
    public decimal Level2Percentage { get; } = 0.03m; // 3%
    public decimal Level3Percentage { get; } = 0.02m; // 2%
}
```

---

## 🎯 **PRIORIZAÇÃO DE IMPLEMENTAÇÃO**

### **PRIORIDADE ALTA** (Implementar primeiro)
1. **Extrair userId do token JWT** - Segurança crítica
2. **Implementar verificação Admin Global** - Autorização
3. **BusinessStatus Enum** - Tipagem de status de negócios
4. **PaymentStatus Enum** - Tipagem de status de pagamentos

### **PRIORIDADE MÉDIA** (Implementar depois)
1. **PaymentType Enum** - Tipos de pagamento de comissão
2. **AuditAction/AuditEntityType Enums** - Melhor tipagem de auditoria
3. **SortField/SortDirection Enums** - Tipagem de ordenação

### **PRIORIDADE BAIXA** (Futuras melhorias)
1. **StatusEntity Enum** - Substituir bool Active por enum
2. **RecommendationLevel Enum** - Níveis de recomendação
3. **CommissionSettings Class** - Configurações de comissão tipadas
4. **Validações de acesso por vetor** - Implementações futuras

---

## 📝 **IMPACTO DAS MUDANÇAS**

### **Breaking Changes**
- Mudança de `string Status` para enums em entidades
- Mudança de constantes string para enums em auditoria
- Alteração de assinaturas de métodos de repositório

### **Benefícios**
1. **Tipo-segurança** em tempo de compilação
2. **IntelliSense** melhorado
3. **Menos erros** por typos em strings
4. **Código mais maintível** e legível
5. **Validação automática** de valores

### **Estratégia de Migração**
1. Criar enums novos sem quebrar código existente
2. Criar métodos de extensão para conversão
3. Migrar gradualmente uso por uso
4. Deprecar constantes antigas
5. Remover constantes após migração completa

---

## ✅ **CONCLUSÃO**

O projeto tem uma base sólida, mas pode se beneficiar significativamente de:

1. **Melhor tipificação** de status e tipos de pagamento
2. **Resolução dos TODOs de segurança** (extração de userId)
3. **Implementação de validações de autorização** pendentes
4. **Conversão de strings hardcoded** para enums tipados

A implementação desses pontos aumentará a **robustez**, **maintibilidade** e **segurança** do sistema.



Arrume a regra de negócio pois posso ter niveis infinitos e a rede de comissionamento vai andar como uma corrente, 
então, dos 10% do valor do negócio fechado, de um certo nivel o vetor sempre fica com 10%, por exemplo:

1. Vetor -> Finder 1 -> "Recomendação do Finder 1 que fechou negócio"
1. 50% / 50%

2. Vetor -> Finder 1 -> Finder 2 -> "Recomendação do Finder 2 que fechou negócio"
2. 15% / 35% / 50%

3. Vetor -> Finder 1 -> Finder 2 -> Finder 3 -> "Recomendação do Finder 3 que fechou negócio"
3. 10% / 15% / 25% / 50%

4. Vetor -> Finder 1 -> Finder 2 -> Finder 3 -> Finder 4 -> "Recomendação do Finder  que fechou negócio"
4. 10% / 0% / 15% / 25% / 50%

4. Vetor -> Finder 1 -> Finder 2 -> Finder 3 -> Finder 4 -> Finder 5 -> "Recomendação do Finder  que fechou negócio"
4. 10% / 0% / 0% / 15% / 25% / 50%