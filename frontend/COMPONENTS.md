# Documentação de Componentes

Guia completo de todos os componentes reutilizáveis do sistema.

## 📚 Índice

- [Componentes de UI](#componentes-de-ui)
  - [Alert](#alert)
  - [Badge](#badge)
  - [Button](#button)
  - [Card](#card)
  - [Checkbox](#checkbox)
  - [Input](#input)
  - [Loading](#loading)
  - [Modal](#modal)
  - [Pagination](#pagination)
  - [Radio](#radio)
  - [Select](#select)
  - [Table](#table)
  - [Textarea](#textarea)
- [Componentes de Feedback](#componentes-de-feedback)
  - [Toast](#toast)
  - [ConfirmDialog](#confirmdialog)
  - [EmptyState](#emptystate)
  - [ErrorBoundary](#errorboundary)
- [Componentes Especializados](#componentes-especializados)
  - [PartnerTreeView](#partnertreeview)

---

## Componentes de UI

### Alert

Componente para exibir mensagens de feedback ao usuário.

**Localização:** `src/components/common/Alert/Alert.tsx`

**Props:**

```typescript
interface AlertProps {
  type?: 'info' | 'success' | 'warning' | 'error';
  title?: string;
  children: React.ReactNode;
  onClose?: () => void;
}
```

**Exemplo de Uso:**

```tsx
import { Alert } from '@/components/common/Alert';

// Alert de sucesso
<Alert type="success" title="Sucesso!">
  Operação realizada com sucesso.
</Alert>

// Alert de erro com botão fechar
<Alert 
  type="error" 
  title="Erro!"
  onClose={() => console.log('Fechado')}
>
  Ocorreu um erro ao processar sua solicitação.
</Alert>

// Alert de informação simples
<Alert type="info">
  Este é um aviso informativo.
</Alert>
```

**Variantes:**
- `info` (padrão): Fundo cinza claro, ícone Info
- `success`: Fundo cinza claro, ícone CheckCircle
- `warning`: Fundo cinza médio, ícone AlertCircle
- `error`: Fundo cinza escuro, texto branco, ícone XCircle

---

### Badge

Componente para exibir tags e labels coloridos.

**Localização:** `src/components/common/Badge/Badge.tsx`

**Props:**

```typescript
interface BadgeProps {
  variant?: 'primary' | 'success' | 'warning' | 'error' | 'info' | 'gray' | 'black';
  size?: 'sm' | 'md' | 'lg';
  children: React.ReactNode;
  className?: string;
}
```

**Exemplo de Uso:**

```tsx
import { Badge } from '@/components/common/Badge';

// Badge de status
<Badge variant="success">Ativo</Badge>
<Badge variant="error">Inativo</Badge>
<Badge variant="warning">Pendente</Badge>

// Badge com tamanhos
<Badge size="sm" variant="info">Pequeno</Badge>
<Badge size="md" variant="primary">Médio</Badge>
<Badge size="lg" variant="black">Grande</Badge>

// Badge personalizado
<Badge variant="gray" className="font-bold">
  Custom
</Badge>
```

**Variantes de Cor:**
- `primary`: Preto com texto branco
- `success`: Verde
- `warning`: Amarelo
- `error`: Vermelho
- `info`: Azul
- `gray`: Cinza
- `black`: Preto (padrão)

---

### Button

Componente de botão com múltiplas variantes e estados.

**Localização:** `src/components/common/Button/Button.tsx`

**Props:**

```typescript
interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger';
  size?: 'sm' | 'md' | 'lg';
  isLoading?: boolean;
  icon?: React.ReactNode;
  fullWidth?: boolean;
}
```

**Exemplo de Uso:**

```tsx
import { Button } from '@/components/common/Button';
import { Plus, Edit, Trash2 } from 'lucide-react';

// Botão primário
<Button variant="primary" onClick={handleSave}>
  Salvar
</Button>

// Botão com ícone
<Button variant="outline" icon={<Plus />}>
  Adicionar
</Button>

// Botão em estado de loading
<Button variant="primary" isLoading>
  Salvando...
</Button>

// Botão full width
<Button variant="secondary" fullWidth>
  Cancelar
</Button>

// Botões de diferentes tamanhos
<Button size="sm" variant="ghost">Pequeno</Button>
<Button size="md" variant="primary">Médio</Button>
<Button size="lg" variant="outline">Grande</Button>

// Botão de perigo
<Button variant="danger" icon={<Trash2 />}>
  Excluir
</Button>
```

**Variantes:**
- `primary`: Fundo preto, texto branco (padrão)
- `secondary`: Fundo cinza claro
- `outline`: Borda preta, sem fundo
- `ghost`: Sem borda, hover com fundo cinza
- `danger`: Fundo cinza escuro (para ações destrutivas)

---

### Card

Componente de card para agrupar conteúdo.

**Localização:** `src/components/common/Card/Card.tsx`

**Props:**

```typescript
interface CardProps {
  title?: string;
  subtitle?: string;
  children: React.ReactNode;
  actions?: React.ReactNode;
  className?: string;
  hoverable?: boolean;
  onClick?: () => void;
}
```

**Exemplo de Uso:**

```tsx
import { Card } from '@/components/common/Card';
import { Button } from '@/components/common/Button';

// Card simples
<Card title="Informações do Usuário">
  <p>Nome: João Silva</p>
  <p>Email: joao@email.com</p>
</Card>

// Card com subtítulo e ações
<Card
  title="Dashboard"
  subtitle="Visão geral do sistema"
  actions={
    <Button size="sm" variant="outline">Ver Mais</Button>
  }
>
  <div>Conteúdo do card...</div>
</Card>

// Card clicável com hover
<Card 
  title="Parceiro #001"
  hoverable
  onClick={() => navigate('/parceiros/1')}
>
  <p>Clique para ver detalhes</p>
</Card>
```

---

### Checkbox

Componente de checkbox customizado.

**Localização:** `src/components/common/Checkbox/Checkbox.tsx`

**Props:**

```typescript
interface CheckboxProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
}
```

**Exemplo de Uso:**

```tsx
import { Checkbox } from '@/components/common/Checkbox';

const [accepted, setAccepted] = useState(false);

// Checkbox simples
<Checkbox
  checked={accepted}
  onChange={(e) => setAccepted(e.target.checked)}
  label="Aceito os termos"
/>

// Checkbox com erro
<Checkbox
  label="Campo obrigatório"
  error="Você deve aceitar para continuar"
/>

// Checkbox com React Hook Form
<Checkbox
  {...register('acceptTerms')}
  label="Li e aceito os termos"
/>
```

---

### Input

Componente de input de texto com suporte a ícones e validação.

**Localização:** `src/components/common/Input/Input.tsx`

**Props:**

```typescript
interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  helperText?: string;
  icon?: React.ReactNode;
}
```

**Exemplo de Uso:**

```tsx
import { Input } from '@/components/common/Input';
import { Search, Mail, Lock } from 'lucide-react';

// Input simples com label
<Input
  label="Nome"
  placeholder="Digite seu nome"
  required
/>

// Input com ícone
<Input
  label="Email"
  type="email"
  icon={<Mail className="w-4 h-4" />}
  placeholder="seu@email.com"
/>

// Input com erro
<Input
  label="Senha"
  type="password"
  icon={<Lock className="w-4 h-4" />}
  error="Senha deve ter no mínimo 6 caracteres"
/>

// Input com helper text
<Input
  label="Buscar"
  icon={<Search className="w-4 h-4" />}
  helperText="Digite para buscar parceiros"
/>

// Input com React Hook Form
<Input
  {...register('email')}
  label="Email"
  error={errors.email?.message}
/>
```

---

### Loading

Componente de loading spinner.

**Localização:** `src/components/common/Loading/Loading.tsx`

**Props:**

```typescript
interface LoadingProps {
  size?: 'sm' | 'md' | 'lg';
  text?: string;
  fullScreen?: boolean;
}
```

**Exemplo de Uso:**

```tsx
import { Loading } from '@/components/common/Loading';

// Loading simples
<Loading />

// Loading com texto
<Loading text="Carregando dados..." />

// Loading pequeno (inline)
<Loading size="sm" />

// Loading grande
<Loading size="lg" text="Processando..." />

// Loading em tela cheia (overlay)
<Loading fullScreen text="Aguarde..." />

// Uso em páginas
if (isLoading) {
  return <Loading text="Carregando usuários..." />;
}
```

---

### Modal

Componente de modal/dialog genérico.

**Localização:** `src/components/common/Modal/Modal.tsx`

**Props:**

```typescript
interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title?: string;
  children: React.ReactNode;
  footer?: React.ReactNode;
  size?: 'sm' | 'md' | 'lg' | 'xl';
}
```

**Exemplo de Uso:**

```tsx
import { Modal } from '@/components/common/Modal';
import { Button } from '@/components/common/Button';

const [isOpen, setIsOpen] = useState(false);

// Modal simples
<Modal
  isOpen={isOpen}
  onClose={() => setIsOpen(false)}
  title="Título do Modal"
>
  <p>Conteúdo do modal...</p>
</Modal>

// Modal com footer personalizado
<Modal
  isOpen={isOpen}
  onClose={() => setIsOpen(false)}
  title="Confirmar Ação"
  footer={
    <div className="flex gap-3">
      <Button variant="outline" onClick={() => setIsOpen(false)}>
        Cancelar
      </Button>
      <Button variant="primary" onClick={handleConfirm}>
        Confirmar
      </Button>
    </div>
  }
>
  <p>Tem certeza que deseja continuar?</p>
</Modal>

// Modal grande
<Modal
  isOpen={isOpen}
  onClose={() => setIsOpen(false)}
  title="Detalhes Completos"
  size="xl"
>
  <div>Conteúdo extenso...</div>
</Modal>
```

---

### Pagination

Componente de paginação para listas.

**Localização:** `src/components/common/Pagination/Pagination.tsx`

**Props:**

```typescript
interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}
```

**Exemplo de Uso:**

```tsx
import { Pagination } from '@/components/common/Pagination';

const [page, setPage] = useState(1);
const totalPages = Math.ceil(totalItems / pageSize);

<Pagination
  currentPage={page}
  totalPages={totalPages}
  onPageChange={setPage}
/>

// Uso completo em página de lista
function UsersListPage() {
  const [page, setPage] = useState(1);
  
  const { data } = useQuery({
    queryKey: ['users', page],
    queryFn: () => usersApi.list({ page, pageSize: 20 }),
  });
  
  return (
    <div>
      <Table data={data?.items} />
      <Pagination
        currentPage={page}
        totalPages={Math.ceil(data?.totalItems / 20)}
        onPageChange={setPage}
      />
    </div>
  );
}
```

---

### Radio

Componente de radio button customizado.

**Localização:** `src/components/common/Radio/Radio.tsx`

**Props:**

```typescript
interface RadioProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
}
```

**Exemplo de Uso:**

```tsx
import { Radio } from '@/components/common/Radio';

const [selected, setSelected] = useState('option1');

// Radio buttons em grupo
<div className="space-y-2">
  <Radio
    name="options"
    value="option1"
    checked={selected === 'option1'}
    onChange={(e) => setSelected(e.target.value)}
    label="Opção 1"
  />
  <Radio
    name="options"
    value="option2"
    checked={selected === 'option2'}
    onChange={(e) => setSelected(e.target.value)}
    label="Opção 2"
  />
</div>

// Radio com React Hook Form
<Radio
  {...register('paymentMethod')}
  value="credit"
  label="Cartão de Crédito"
/>
```

---

### Select

Componente de select dropdown customizado.

**Localização:** `src/components/common/Select/Select.tsx`

**Props:**

```typescript
interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  error?: string;
  helperText?: string;
  options: Array<{ value: string; label: string }>;
}
```

**Exemplo de Uso:**

```tsx
import { Select } from '@/components/common/Select';

const statusOptions = [
  { value: 'all', label: 'Todos' },
  { value: 'active', label: 'Ativos' },
  { value: 'inactive', label: 'Inativos' },
];

// Select simples
<Select
  label="Status"
  options={statusOptions}
  value={status}
  onChange={(e) => setStatus(e.target.value)}
/>

// Select com erro
<Select
  label="Perfil"
  options={profileOptions}
  error="Campo obrigatório"
  required
/>

// Select com React Hook Form
<Select
  {...register('vectorId')}
  label="Vetor"
  options={vectors.map(v => ({
    value: v.id,
    label: v.name
  }))}
  error={errors.vectorId?.message}
/>
```

---

### Table

Componente de tabela responsiva com ordenação.

**Localização:** `src/components/common/Table/Table.tsx`

**Props:**

```typescript
interface Column<T> {
  header: string;
  accessor: keyof T | ((row: T) => React.ReactNode);
  sortable?: boolean;
  className?: string;
}

interface TableProps<T> {
  data: T[];
  columns: Column<T>[];
  isLoading?: boolean;
  onRowClick?: (row: T) => void;
  emptyMessage?: string;
}
```

**Exemplo de Uso:**

```tsx
import { Table } from '@/components/common/Table';
import { Badge } from '@/components/common/Badge';

const columns = [
  {
    header: 'Nome',
    accessor: 'name',
    sortable: true,
  },
  {
    header: 'Email',
    accessor: 'email',
  },
  {
    header: 'Status',
    accessor: (row) => (
      <Badge variant={row.isActive ? 'success' : 'error'}>
        {row.isActive ? 'Ativo' : 'Inativo'}
      </Badge>
    ),
  },
  {
    header: 'Ações',
    accessor: (row) => (
      <Button size="sm" variant="outline">
        Editar
      </Button>
    ),
  },
];

// Tabela com dados
<Table
  data={users}
  columns={columns}
  onRowClick={(user) => navigate(`/usuarios/${user.id}`)}
/>

// Tabela em loading
<Table
  data={[]}
  columns={columns}
  isLoading={true}
/>

// Tabela vazia
<Table
  data={[]}
  columns={columns}
  emptyMessage="Nenhum usuário encontrado"
/>
```

---

### Textarea

Componente de textarea com validação.

**Localização:** `src/components/common/Textarea/Textarea.tsx`

**Props:**

```typescript
interface TextareaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
  error?: string;
  helperText?: string;
  maxLength?: number;
}
```

**Exemplo de Uso:**

```tsx
import { Textarea } from '@/components/common/Textarea';

// Textarea simples
<Textarea
  label="Observações"
  placeholder="Digite suas observações..."
  rows={4}
/>

// Textarea com limite de caracteres
<Textarea
  label="Descrição"
  maxLength={500}
  helperText="Máximo 500 caracteres"
/>

// Textarea com erro
<Textarea
  label="Comentário"
  error="Campo obrigatório"
  required
/>

// Textarea com React Hook Form
<Textarea
  {...register('observations')}
  label="Observações"
  error={errors.observations?.message}
  rows={6}
/>
```

---

## Componentes de Feedback

### Toast

Sistema de notificações toast com contexto.

**Localização:** `src/components/common/Toast/`

**Uso com Hook:**

```tsx
import { useToast } from '@/hooks/useToast';

function MyComponent() {
  const { showToast } = useToast();
  
  // Toast de sucesso
  const handleSuccess = () => {
    showToast('success', 'Operação realizada com sucesso!');
  };
  
  // Toast de erro
  const handleError = () => {
    showToast('error', 'Erro ao processar solicitação');
  };
  
  // Toast de warning
  const handleWarning = () => {
    showToast('warning', 'Atenção: verifique os dados');
  };
  
  // Toast de info
  const handleInfo = () => {
    showToast('info', 'Nova atualização disponível');
  };
  
  return (
    <Button onClick={handleSuccess}>Salvar</Button>
  );
}
```

**Tipos de Toast:**
- `success`: Verde, ícone CheckCircle
- `error`: Vermelho, ícone XCircle
- `warning`: Amarelo, ícone AlertTriangle
- `info`: Azul, ícone Info

**Configuração:**
- Duração: 3 segundos (auto-dismiss)
- Posição: Bottom-right
- Máximo: 3 toasts simultâneos

---

### ConfirmDialog

Componente de diálogo de confirmação para ações críticas.

**Localização:** `src/components/common/ConfirmDialog/ConfirmDialog.tsx`

**Props:**

```typescript
interface ConfirmDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  variant?: 'danger' | 'warning' | 'info';
  isLoading?: boolean;
}
```

**Exemplo de Uso:**

```tsx
import { ConfirmDialog } from '@/components/common/ConfirmDialog';

const [confirmOpen, setConfirmOpen] = useState(false);

// Diálogo de exclusão (danger)
<ConfirmDialog
  isOpen={confirmOpen}
  onClose={() => setConfirmOpen(false)}
  onConfirm={handleDelete}
  title="Confirmar Exclusão"
  message="Tem certeza que deseja excluir este usuário?"
  confirmText="Sim, excluir"
  cancelText="Cancelar"
  variant="danger"
/>

// Diálogo de desativação (warning)
<ConfirmDialog
  isOpen={confirmOpen}
  onClose={() => setConfirmOpen(false)}
  onConfirm={handleDeactivate}
  title="Desativar Parceiro"
  message="O parceiro será desativado. Deseja continuar?"
  variant="warning"
/>

// Diálogo com loading
<ConfirmDialog
  isOpen={confirmOpen}
  onClose={() => setConfirmOpen(false)}
  onConfirm={handleConfirm}
  title="Processar Pagamento"
  message="Confirma o processamento deste pagamento?"
  isLoading={mutation.isPending}
/>
```

---

### EmptyState

Componente para exibir estado vazio em listas.

**Localização:** `src/components/common/EmptyState/EmptyState.tsx`

**Props:**

```typescript
interface EmptyStateProps {
  icon?: React.ReactNode;
  title?: string;
  message?: string;
  action?: React.ReactNode;
}
```

**Exemplo de Uso:**

```tsx
import { EmptyState } from '@/components/common/EmptyState';
import { Users, FileText } from 'lucide-react';
import { Button } from '@/components/common/Button';

// Empty state simples
<EmptyState
  icon={<Users className="w-12 h-12" />}
  title="Nenhum usuário encontrado"
  message="Comece adicionando seu primeiro usuário"
/>

// Empty state com ação
<EmptyState
  icon={<FileText className="w-12 h-12" />}
  title="Nenhum relatório disponível"
  message="Não há dados para exibir no momento"
  action={
    <Button variant="primary" onClick={() => navigate('/negocios/novo')}>
      Criar Negócio
    </Button>
  }
/>

// Uso em listas
if (data?.items.length === 0) {
  return (
    <EmptyState
      title="Lista vazia"
      message="Nenhum item encontrado com os filtros aplicados"
    />
  );
}
```

---

### ErrorBoundary

Componente de captura de erros React.

**Localização:** `src/components/common/ErrorBoundary/ErrorBoundary.tsx`

**Props:**

```typescript
interface ErrorBoundaryProps {
  children: React.ReactNode;
}
```

**Exemplo de Uso:**

```tsx
import { ErrorBoundary } from '@/components/common/ErrorBoundary';

// Wrap na raiz da aplicação (App.tsx)
function App() {
  return (
    <ErrorBoundary>
      <QueryClientProvider client={queryClient}>
        <ToastProvider>
          <RouterProvider router={router} />
        </ToastProvider>
      </QueryClientProvider>
    </ErrorBoundary>
  );
}

// Wrap em componente específico
<ErrorBoundary>
  <ComplexComponent />
</ErrorBoundary>
```

**Recursos:**
- Captura erros em toda a árvore de componentes
- Exibe UI amigável com ícone AlertTriangle
- Botão "Tentar Novamente" para resetar
- Botão "Voltar ao Início" para navegação
- Details collapsible com mensagem de erro técnica
- Previne crash da aplicação inteira

---

## Componentes Especializados

### PartnerTreeView

Componente para visualizar hierarquia de parceiros em árvore.

**Localização:** `src/components/common/PartnerTreeView/PartnerTreeView.tsx`

**Props:**

```typescript
interface PartnerNode {
  id: string;
  name: string;
  cpf: string;
  isActive: boolean;
  childPartners?: PartnerNode[];
}

interface PartnerTreeViewProps {
  data: PartnerNode[];
  onNodeClick?: (partner: PartnerNode) => void;
}
```

**Exemplo de Uso:**

```tsx
import { PartnerTreeView } from '@/components/common/PartnerTreeView';

const { data } = useQuery({
  queryKey: ['partner-tree'],
  queryFn: partnersApi.getTree,
});

// Árvore simples
<PartnerTreeView data={data} />

// Árvore com callback de clique
<PartnerTreeView
  data={data}
  onNodeClick={(partner) => {
    navigate(`/parceiros/${partner.id}`);
  }}
/>
```

**Recursos:**
- Hierarquia recursiva (suporta n níveis)
- Auto-expand dos primeiros 2 níveis
- Botão de expand/collapse por nó
- Badge de status (Ativo/Inativo)
- Transições suaves
- Hover states
- Responsivo

**Estrutura Visual:**
```
└─ Parceiro Raiz
   ├─ Nível 1 - Filho 1
   │  ├─ Nível 2 - Neto 1
   │  └─ Nível 2 - Neto 2
   └─ Nível 1 - Filho 2
```

---

## Boas Práticas

### Importação
```tsx
// ✅ Importe de forma individual
import { Button } from '@/components/common/Button';
import { Input } from '@/components/common/Input';

// ❌ Evite import de index (não existe)
import { Button, Input } from '@/components/common';
```

### Composição
```tsx
// ✅ Componha componentes para criar interfaces complexas
<Card title="Formulário de Usuário">
  <form onSubmit={handleSubmit}>
    <Input label="Nome" {...register('name')} />
    <Input label="Email" {...register('email')} />
    <div className="flex gap-2">
      <Button variant="outline" onClick={onCancel}>
        Cancelar
      </Button>
      <Button variant="primary" type="submit">
        Salvar
      </Button>
    </div>
  </form>
</Card>
```

### Tipagem
```tsx
// ✅ Sempre use TypeScript para props
interface MyComponentProps {
  title: string;
  isActive?: boolean;
  onSave: () => void;
}

export const MyComponent: React.FC<MyComponentProps> = ({
  title,
  isActive = false,
  onSave,
}) => {
  // ...
};
```

### Consistência de Tema
```tsx
// ✅ Use as variantes padronizadas
<Button variant="primary">Salvar</Button>
<Badge variant="success">Ativo</Badge>
<Alert type="error">Erro!</Alert>

// ❌ Evite cores customizadas inline
<div className="bg-blue-500">Custom</div>
```

---

## Referência Rápida

### Botões
```tsx
<Button variant="primary">Primário</Button>
<Button variant="secondary">Secundário</Button>
<Button variant="outline">Outline</Button>
<Button variant="ghost">Ghost</Button>
<Button variant="danger">Perigo</Button>
```

### Feedback
```tsx
showToast('success', 'Sucesso!');
showToast('error', 'Erro!');
<Alert type="warning">Atenção</Alert>
<Badge variant="info">Info</Badge>
```

### Formulários
```tsx
<Input label="Campo" {...register('field')} />
<Textarea label="Texto" rows={4} />
<Select label="Opção" options={options} />
<Checkbox label="Aceito" />
<Radio label="Opção A" />
```

### Layout
```tsx
<Card title="Título">Conteúdo</Card>
<Modal isOpen={open} onClose={close}>Conteúdo</Modal>
<Table data={items} columns={columns} />
<Pagination currentPage={1} totalPages={10} />
```

---

## Suporte

Para dúvidas ou sugestões sobre componentes:
1. Consulte os arquivos TypeScript para definições completas de props
2. Veja exemplos de uso nas páginas existentes (`src/pages/`)
3. Abra uma issue no repositório

**Última atualização:** Dezembro 2024
