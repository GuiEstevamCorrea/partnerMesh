# Guia Rápido de Migração i18n

Este guia mostra como migrar componentes existentes para usar o sistema de internacionalização.

## 🔄 Passo a Passo de Migração

### 1. Importar o Hook

```tsx
// Antes
import { formatCurrency } from '@/utils/formatters';

// Depois
import { useI18n } from '@/hooks/useI18n';

function MyComponent() {
  const { t, formatCurrency } = useI18n();
  // ...
}
```

### 2. Substituir Textos Hardcoded

```tsx
// ❌ Antes
<h1>Parceiros</h1>
<button>Adicionar Parceiro</button>
<p>Nome é obrigatório</p>

// ✅ Depois
<h1>{t('partners.title')}</h1>
<button>{t('partners.addPartner')}</button>
<p>{t('partners.validation.nameRequired')}</p>
```

### 3. Migrar Schemas de Validação

```tsx
// ❌ Antes
const schema = z.object({
  name: z.string().min(1, 'Nome é obrigatório'),
  email: z.string().email('Email inválido')
});

// ✅ Depois
const createSchema = (t: any) => z.object({
  name: z.string().min(1, t('partners.validation.nameRequired')),
  email: z.string().email(t('partners.validation.emailInvalid'))
});

// No componente
const { t } = useI18n();
const schema = createSchema(t);
```

### 4. Atualizar Formatações

```tsx
// ❌ Antes
import { formatCurrency, formatDate } from '@/utils/formatters';
{formatCurrency(value)}
{formatDate(date)}

// ✅ Depois
const { formatCurrency, formatDate } = useI18n();
{formatCurrency(value)} // Adapta ao idioma
{formatDate(date)}      // Adapta ao idioma
```

### 5. Status Dinâmicos

```tsx
// ❌ Antes
const getStatusLabel = (status: string) => {
  switch(status) {
    case 'pending': return 'Pendente';
    case 'paid': return 'Pago';
    default: return status;
  }
};

// ✅ Depois
{t(`payments.status.${status.toLowerCase()}`)}
```

## 📋 Checklist de Migração

Para cada componente/página:

- [ ] Importar `useI18n`
- [ ] Substituir títulos e labels
- [ ] Migrar mensagens de validação
- [ ] Atualizar botões e ações
- [ ] Traduzir placeholders
- [ ] Migrar mensagens de toast/alert
- [ ] Atualizar formatações de moeda/data
- [ ] Traduzir status e badges
- [ ] Revisar textos de confirmação

## 🎯 Exemplos Práticos

### Exemplo 1: Botões

```tsx
// Antes
<Button>Salvar</Button>
<Button>Cancelar</Button>
<Button>Voltar</Button>

// Depois
<Button>{t('common.save')}</Button>
<Button>{t('common.cancel')}</Button>
<Button>{t('common.back')}</Button>
```

### Exemplo 2: Toast Messages

```tsx
// Antes
showToast('success', 'Parceiro criado com sucesso!');
showToast('error', 'Erro ao criar parceiro');

// Depois
showToast('success', t('partners.partnerCreated'));
showToast('error', t('partners.partnerError'));
```

### Exemplo 3: Tabelas

```tsx
// Antes
<th>Nome</th>
<th>Email</th>
<th>Status</th>
<th>Ações</th>

// Depois
<th>{t('common.name')}</th>
<th>{t('common.email')}</th>
<th>{t('common.status')}</th>
<th>{t('common.actions')}</th>
```

### Exemplo 4: Inputs com Labels

```tsx
// Antes
<Input 
  label="Nome" 
  placeholder="Digite o nome"
  error="Nome é obrigatório"
/>

// Depois
<Input 
  label={t('common.name')}
  placeholder={t('partners.form.namePlaceholder')}
  error={errors.name?.message}
/>
```

### Exemplo 5: Modal de Confirmação

```tsx
// Antes
<ConfirmDialog
  title="Confirmar Exclusão"
  message="Tem certeza que deseja excluir este parceiro?"
  confirmText="Sim, excluir"
  cancelText="Cancelar"
/>

// Depois
<ConfirmDialog
  title={t('common.confirmDelete')}
  message={t('partners.deleteConfirmation')}
  confirmText={t('common.yes')}
  cancelText={t('common.cancel')}
/>
```

## 🔑 Chaves Comuns Já Disponíveis

```typescript
// Ações gerais
t('common.save')
t('common.cancel')
t('common.edit')
t('common.delete')
t('common.back')
t('common.loading')

// Status
t('common.active')
t('common.inactive')

// Navegação
t('navigation.dashboard')
t('navigation.partners')
t('navigation.business')
t('navigation.payments')

// Formulários
t('common.name')
t('common.email')
t('common.status')
t('common.date')
t('common.value')
```

## ⚠️ Armadilhas Comuns

### 1. Usar formatCurrency do utils ao invés do hook

```tsx
// ❌ Errado - sempre mostra em Real
import { formatCurrency } from '@/utils/formatters';
{formatCurrency(100)}

// ✅ Correto - adapta ao idioma
const { formatCurrency } = useI18n();
{formatCurrency(100)}
```

### 2. Esquecer de traduzir mensagens de erro

```tsx
// ❌ Errado
catch (error) {
  showToast('error', 'Erro ao salvar');
}

// ✅ Correto
catch (error) {
  showToast('error', t('errors.generic'));
}
```

### 3. Não usar schema factory para validações

```tsx
// ❌ Errado - mensagens não atualizam com idioma
const schema = z.object({
  name: z.string().min(1, 'Required')
});

// ✅ Correto
const createSchema = (t: any) => z.object({
  name: z.string().min(1, t('validation.required'))
});
```

## 📝 Template de Componente Migrado

```tsx
import { useI18n } from '@/hooks/useI18n';
import { z } from 'zod';

const createSchema = (t: any) => z.object({
  name: z.string().min(1, t('module.validation.nameRequired')),
});

export function MyComponent() {
  const { t, formatCurrency, formatDate } = useI18n();
  
  const schema = createSchema(t);
  
  return (
    <div>
      <h1>{t('module.title')}</h1>
      
      <Button onClick={handleSave}>
        {t('common.save')}
      </Button>
      
      <p>{formatCurrency(100)}</p>
      <p>{formatDate(new Date())}</p>
    </div>
  );
}
```

## 🚀 Ordem Recomendada de Migração

1. ✅ **Componentes de Layout** (Sidebar, Header) - **CONCLUÍDO**
2. ✅ **Páginas de Autenticação** (Login) - **CONCLUÍDO**
3. ⏳ **Dashboard** - **PARCIALMENTE CONCLUÍDO**
4. ⏳ Páginas CRUD (Partners, Business, etc.)
5. ⏳ Componentes comuns (Button, Input, etc.)
6. ⏳ Modais e Dialogs
7. ⏳ Mensagens de Toast

## 🆘 Precisa de Ajuda?

Consulte:
- [I18N.md](./I18N.md) - Documentação completa
- Exemplos já migrados: LoginPage, Sidebar, Header, DashboardPage (parcial)

---

**Nota**: Após migrar um componente, sempre teste em todos os idiomas (pt-BR, en, es) para garantir que todas as chaves estão corretas!