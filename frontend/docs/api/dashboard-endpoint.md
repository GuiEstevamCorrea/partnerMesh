# 📊 Dashboard API

## Endpoint: GET /api/dashboard

Retorna dados consolidados para exibição no dashboard principal.

### Resposta de Sucesso (200)

```json
{
  "stats": {
    "totalActiveUsers": 45,
    "totalActiveVectors": 8,
    "totalPartners": 120,
    "totalBusinessThisMonth": 23,
    "pendingCommissionsAmount": 15750.50,
    "paidCommissionsThisMonth": 32400.00
  },
  "recentBusiness": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "partnerName": "João Silva",
      "businessTypeName": "Venda de Produto",
      "value": 5000.00,
      "totalCommission": 500.00,
      "createdAt": "2025-12-16T10:30:00Z",
      "status": "active"
    },
    {
      "id": "550e8400-e29b-41d4-a716-446655440002",
      "partnerName": "Maria Santos",
      "businessTypeName": "Prestação de Serviço",
      "value": 3000.00,
      "totalCommission": 300.00,
      "createdAt": "2025-12-15T14:20:00Z",
      "status": "active"
    }
  ],
  "pendingPayments": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440003",
      "recipientName": "João Silva",
      "businessId": "550e8400-e29b-41d4-a716-446655440001",
      "amount": 500.00,
      "level": 1,
      "createdAt": "2025-12-16T10:30:00Z"
    },
    {
      "id": "660e8400-e29b-41d4-a716-446655440004",
      "recipientName": "Carlos Oliveira",
      "businessId": "550e8400-e29b-41d4-a716-446655440001",
      "amount": 250.00,
      "level": 2,
      "createdAt": "2025-12-16T10:30:00Z"
    }
  ]
}
```

### Regras de Negócio

1. **Estatísticas (stats)**:
   - `totalActiveUsers`: Conta apenas usuários com `isActive = true`
   - `totalActiveVectors`: Conta apenas vetores com `isActive = true`
   - `totalPartners`: Total de parceiros (ativos e inativos) visíveis conforme permissão do usuário
   - `totalBusinessThisMonth`: Negócios criados no mês atual (status ativo)
   - `pendingCommissionsAmount`: Soma de todos os pagamentos com status "a pagar"
   - `paidCommissionsThisMonth`: Soma de pagamentos com status "pago" no mês atual

2. **Negócios Recentes (recentBusiness)**:
   - Retorna os últimos 10 negócios ordenados por data de criação (DESC)
   - Filtra por vetor do usuário (exceto AdminGlobal que vê todos)
   - Inclui negócios ativos e cancelados

3. **Pagamentos Pendentes (pendingPayments)**:
   - Retorna os próximos 5 pagamentos pendentes ordenados por data de criação (ASC)
   - Filtra por vetor do usuário (exceto AdminGlobal)
   - Apenas pagamentos com status "a pagar"

### Permissões

- **AdminGlobal**: Vê dados consolidados de todos os vetores
- **AdminVetor**: Vê apenas dados do seu vetor
- **Operador**: Vê apenas dados do seu vetor

### Códigos de Erro

- **401 Unauthorized**: Token inválido ou expirado
- **403 Forbidden**: Usuário sem permissão
- **500 Internal Server Error**: Erro interno do servidor

### Exemplo de Uso (Frontend)

```typescript
import { useQuery } from '@tanstack/react-query';
import { dashboardApi } from '@/api/endpoints';

const { data, isLoading, error } = useQuery({
  queryKey: ['dashboard'],
  queryFn: dashboardApi.getDashboardData,
  // Atualiza a cada 5 minutos
  refetchInterval: 5 * 60 * 1000,
});
```

### Cache e Performance

- Os dados do dashboard são cacheados por 5 minutos no React Query
- O backend deve otimizar as queries usando índices apropriados
- Considerar cache no Redis para AdminGlobal (dados globais)

### Observações

- Valores monetários são retornados em número (decimal)
- Datas seguem o formato ISO 8601 (UTC)
- IDs são UUIDs v4
- O nível de comissão varia de 1 a 3
