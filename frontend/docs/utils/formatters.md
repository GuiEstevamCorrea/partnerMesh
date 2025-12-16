# 🛠️ Utilitários - Formatters

Funções utilitárias para formatação de dados no frontend.

## 📁 Localização

`src/utils/formatters.ts`

## 📋 Funções Disponíveis

### formatCurrency(value: number): string

Formata um número para o formato de moeda brasileira (BRL).

**Exemplo:**
```typescript
formatCurrency(1234.56);
// Retorna: "R$ 1.234,56"

formatCurrency(0.99);
// Retorna: "R$ 0,99"
```

---

### formatNumber(value: number): string

Formata um número com separadores de milhares no padrão brasileiro.

**Exemplo:**
```typescript
formatNumber(1234567);
// Retorna: "1.234.567"

formatNumber(42);
// Retorna: "42"
```

---

### formatDate(date: string | Date): string

Formata uma data para o formato brasileiro (dd/MM/yyyy).

**Exemplo:**
```typescript
formatDate('2025-12-16T10:30:00Z');
// Retorna: "16/12/2025"

formatDate(new Date(2025, 11, 16));
// Retorna: "16/12/2025"
```

---

### formatDateTime(date: string | Date): string

Formata uma data com hora para o formato brasileiro (dd/MM/yyyy HH:mm).

**Exemplo:**
```typescript
formatDateTime('2025-12-16T10:30:00Z');
// Retorna: "16/12/2025 07:30"

formatDateTime(new Date());
// Retorna: "16/12/2025 15:45"
```

---

### formatRelativeDate(date: string | Date): string

Formata uma data de forma relativa (ex: "há 2 horas", "ontem").

**Exemplo:**
```typescript
// Supondo que agora seja 16/12/2025 15:00

formatRelativeDate('2025-12-16T15:00:00Z');
// Retorna: "agora"

formatRelativeDate('2025-12-16T14:30:00Z');
// Retorna: "há 30 min"

formatRelativeDate('2025-12-16T10:00:00Z');
// Retorna: "há 5h"

formatRelativeDate('2025-12-15T15:00:00Z');
// Retorna: "ontem"

formatRelativeDate('2025-12-10T15:00:00Z');
// Retorna: "há 6 dias"

formatRelativeDate('2025-11-16T15:00:00Z');
// Retorna: "16/11/2025"
```

---

### truncate(text: string, maxLength: number): string

Trunca um texto com ellipsis se exceder o comprimento máximo.

**Exemplo:**
```typescript
truncate('Este é um texto muito longo', 15);
// Retorna: "Este é um texto..."

truncate('Texto curto', 20);
// Retorna: "Texto curto"
```

---

### formatPercent(value: number, decimals?: number): string

Formata um percentual com casas decimais opcionais (padrão: 1).

**Exemplo:**
```typescript
formatPercent(75.5);
// Retorna: "75.5%"

formatPercent(75.567, 2);
// Retorna: "75.57%"

formatPercent(100, 0);
// Retorna: "100%"
```

---

## 🎨 Uso no Dashboard

```typescript
import {
  formatCurrency,
  formatNumber,
  formatDate,
  formatRelativeDate,
} from '@/utils/formatters';

// Card de estatística
<p className="text-3xl font-bold">
  {formatNumber(stats.totalPartners)}
</p>

// Valor monetário
<p className="text-2xl font-bold text-green-600">
  {formatCurrency(stats.paidCommissionsThisMonth)}
</p>

// Data de negócio
<p className="text-sm text-gray-600">
  {formatDate(business.createdAt)}
</p>

// Data relativa em notificações
<p className="text-xs text-gray-500">
  {formatRelativeDate(notification.createdAt)}
</p>
```

## 📝 Notas Importantes

1. **Locale**: Todas as funções usam o locale `pt-BR` (português do Brasil)
2. **Timezone**: As datas são formatadas considerando o timezone local do navegador
3. **Performance**: Usa `Intl.NumberFormat` e `Intl.DateTimeFormat` do JavaScript nativo (otimizado)
4. **Acessibilidade**: Considere usar `aria-label` com valores não formatados para leitores de tela

## 🔄 Extensibilidade

Para adicionar novos formatadores, siga o padrão:

```typescript
/**
 * Descrição da função
 */
export const formatNome = (parametro: tipo): string => {
  // Implementação
  return resultado;
};
```

## 🧪 Testes

Para testar os formatadores:

```typescript
import { formatCurrency } from '@/utils/formatters';

describe('formatCurrency', () => {
  it('formata valores corretamente', () => {
    expect(formatCurrency(1234.56)).toBe('R$ 1.234,56');
  });
});
```
