<!-- Baseado no MVP-Plano-Entregãveis, Implemente o -->

# 📋 MVP - Plano de Entregáveis Frontend
## Sistema de Rede de Credenciamento / Vetores

**Data:** 15 de dezembro de 2025  
**Versão:** 1.0  
**Status:** Documentação de Implementação

---

## 📊 Visão Geral do MVP

Este documento define o plano de implementação do frontend React para todos os use cases do sistema, conectado ao backend .NET já existente. O objetivo é entregar um **MVP (Minimum Viable Product)** funcional que permita o gerenciamento completo da rede de parceiros, negócios e comissões.

### ✅ Base Já Implementada

- **Entregável 01**: Setup Inicial (100%)
- **Entregável 02**: Configuração Base (100%)
- **Entregável 03**: Componentes Comuns (100%)

### 🎯 Escopo do MVP

O MVP cobrirá **35 use cases** organizados em **8 blocos funcionais**, totalizando **7 novos entregáveis** (04 a 10).

---

## 📦 Estrutura de Entregáveis

### Entregável 04 - Autenticação e Área Pública
**Prioridade:** 🔴 CRÍTICA  
**Tempo Estimado:** 8 horas  
**Use Cases:** UC-01, UC-02, UC-03

### Entregável 05 - Gestão de Usuários e Vetores
**Prioridade:** 🔴 CRÍTICA  
**Tempo Estimado:** 16 horas  
**Use Cases:** UC-10 a UC-24

### Entregável 06 - Gestão de Parceiros e Tipos de Negócio
**Prioridade:** 🔴 CRÍTICA  
**Tempo Estimado:** 12 horas  
**Use Cases:** UC-30 a UC-44

### Entregável 07 - Gestão de Negócios e Comissões
**Prioridade:** 🔴 CRÍTICA  
**Tempo Estimado:** 16 horas  
**Use Cases:** UC-50 a UC-62

### Entregável 08 - Relatórios e Dashboard
**Prioridade:** 🟡 ALTA  
**Tempo Estimado:** 12 horas  
**Use Cases:** UC-70, UC-71, UC-72

### Entregável 09 - Auditoria e Logs
**Prioridade:** 🟢 MÉDIA  
**Tempo Estimado:** 6 horas  
**Use Cases:** UC-80, UC-81

### Entregável 10 - Refinamentos e Integração Final
**Prioridade:** 🟡 ALTA  
**Tempo Estimado:** 10 horas  
**Objetivo:** Polish, testes, correções

---

## 🔥 Entregável 04 - Autenticação e Área Pública

### Objetivo
Implementar o fluxo completo de autenticação, permitindo login, renovação de token e logout com integração ao backend.

### Use Cases Cobertos
- **UC-01**: Autenticar Usuário
- **UC-02**: Renovar Token
- **UC-03**: Logout

### Páginas a Criar

#### 4.1. Login Page (`/login`)  - OK
**Arquivo:** `src/pages/auth/LoginPage.tsx`

**Funcionalidades:**
- Formulário com email e senha
- Validação com React Hook Form + Zod
- Chamada ao endpoint `POST /api/auth/login`
- Armazenamento do token e refresh token no authStore
- Redirecionamento para dashboard após sucesso
- Exibição de erros de autenticação

**Componentes Utilizados:**
- `Input` (email, senha)
- `Button` (submit, loading state)
- `Alert` (erros)
- `Card` (container do formulário)

**Validações:**
- Email obrigatório e formato válido
- Senha obrigatória (mínimo 6 caracteres)

**Fluxo:**
```
1. Usuário preenche credenciais
2. Submit do formulário
3. Loading state ativado
4. Chamada à API de login
5. Se sucesso:
   - Salvar token/refresh no authStore
   - Redirecionar para /dashboard
6. Se erro:
   - Exibir Alert com mensagem
```

#### 4.2. Layout Público - OK
**Arquivo:** `src/components/layout/PublicLayout.tsx`

**Funcionalidades:**
- Layout minimalista para páginas públicas
- Sem header/sidebar
- Logo e título centralizado
- Footer opcional

#### 4.3. Atualização do Router - OK
**Arquivo:** `src/routes/router.tsx`

**Estrutura:**
```tsx
/ (público)
  /login → LoginPage
  
/ (protegido - PrivateRoute)
  /dashboard → DashboardPage
  /usuarios → (Entregável 05)
  /vetores → (Entregável 05)
  /parceiros → (Entregável 06)
  /tipos-negocio → (Entregável 06)
  /negocios → (Entregável 07)
  /pagamentos → (Entregável 07)
  /relatorios → (Entregável 08)
  /auditoria → (Entregável 09)
```

### Critérios de Aceitação
- [x] Usuário consegue fazer login com credenciais válidas
- [x] Token é armazenado e usado nas requisições
- [x] Refresh token renova automaticamente quando expira
- [x] Logout limpa o estado e redireciona para login
- [x] Erros de autenticação são exibidos claramente
- [x] Rotas protegidas redirecionam para login se não autenticado

### Arquivos a Criar/Modificar
```
src/
  pages/
    auth/
      LoginPage.tsx (NOVO)
  components/
    layout/
      PublicLayout.tsx (NOVO)
  routes/
    index.tsx (MODIFICAR)
  App.tsx (MODIFICAR - adicionar Router)
```

---

## 🔥 Entregável 05 - Gestão de Usuários e Vetores

### Objetivo
Implementar o CRUD completo de usuários e vetores com controle de permissões e validações de negócio.

### Use Cases Cobertos
- **UC-10 a UC-15**: Gestão de Usuários
- **UC-20 a UC-24**: Gestão de Vetores

### Páginas a Criar

#### 5.1. Dashboard Principal - OK
**Arquivo:** `src/pages/DashboardPage.tsx`

**Funcionalidades:**
- Visão geral do sistema
- Cards com estatísticas principais:
  - Total de usuários ativos
  - Total de vetores ativos
  - Total de parceiros
  - Total de negócios no mês
  - Comissões pendentes
  - Comissões pagas no mês
- Seção de negócios recentes (últimos 10)
- Seção de pagamentos pendentes
- Ações rápidas (criar negócio, parceiro, ver árvore, relatórios)
- Integração com API via React Query

**Componentes:**
- `Card` (estatísticas)
- `Badge` (status)
- `Button` (ações rápidas)
- `Loading` (carregamento)
- `Alert` (erros)
- Ícones do Lucide React

#### 5.2. Lista de Usuários - OK
**Arquivo:** `src/pages/Users/UsersListPage.tsx`

**Funcionalidades:**
- Tabela com todos os usuários
- Colunas: Nome, Email, Perfil, Vetor, Status
- Filtros: Nome (busca), Perfil, Vetor (AdminGlobal), Status
- Paginação (20 por página)
- Botão "Novo Usuário"
- Ações por linha: Editar, Ativar/Inativar
- ConfirmDialog para ativar/inativar
- Toast de feedback
- Estados de loading e erro
- Estado vazio

**Componentes:**
- `Table<User>` (com render customizado)
- `Input` (busca com ícone)
- `select` nativo (filtros)
- `Button` (novo, ações)
- `Badge` (status, perfil)
- `Pagination`
- `ConfirmDialog`
- `Loading` e `Alert`

#### 5.3. Formulário de Usuário (Create/Edit) - OK
**Arquivo:** `src/pages/users/UserFormPage.tsx`

**Funcionalidades:** ✅
- Modo criação e edição (mesma página)
- Campos:
  - Nome (obrigatório)
  - Email (obrigatório, único)
  - Senha (obrigatório apenas em criação, opcional em edição)
  - Perfil (select: AdminGlobal, AdminVetor, Operador)
  - Vetor (select, obrigatório exceto AdminGlobal)
  - Status (checkbox ativo/inativo)

**Validações:** ✅
- Email único
- AdminGlobal não pode ter vetor (validação Zod refine)
- Outros perfis devem ter vetor (validação Zod refine)
- Senha mínimo 6 caracteres (validação Zod refine)
- Senha obrigatória apenas em criação (schemas diferentes)

**Componentes:** ✅
- `Input` (nome, email, senha com label condicional)
- `select` nativo (perfil com Permission enum, vetor com loading)
- `checkbox` nativo (ativo)
- `Button` (salvar com loading, cancelar, voltar)
- `Alert` (erros, avisos sobre permissões - info/warning)
- `Card` (container do formulário)
- `Loading` (carregamento de usuário/vetores)

**Recursos Implementados:**
- React Hook Form + Zod com schemas separados (create/edit)
- React Query para carregar usuário e vetores
- Mutations separadas para create e update
- Toast de feedback (success/error)
- Redirect automático após sucesso
- Campo vetor oculto para AdminGlobal
- Campo vetor readonly para AdminVetor/Operador (mostra vetor atual)
- Limpeza automática de vectorId ao selecionar AdminGlobal
- Descrições contextuais por perfil
- Avisos sobre regras de AdminGlobal e AdminVetor

#### 5.4. Confirmação de Alterações Críticas
**Componente:** `ConfirmDialog`

**Uso:**
- Inativar usuário
- Alterar perfil de AdminVetor (validar se é único)
- Resetar senha

#### 5.5. Lista de Vetores
**Arquivo:** `src/pages/vectors/VectorsListPage.tsx`

**Funcionalidades:**
- Tabela com todos os vetores
- Colunas: Nome, Email, Status, Qtd Parceiros
- Filtros: Nome, Status
- Paginação
- Botão "Novo Vetor"
- Ações: Editar, Ativar/Inativar, Ver Árvore

**Componentes:**
- `Table<Vector>`
- `Input` (filtros)
- `Badge` (status)
- `Pagination`
- `Button`

#### 5.6. Formulário de Vetor
**Arquivo:** `src/pages/vectors/VectorFormPage.tsx`

**Funcionalidades:**
- Criar/Editar vetor
- Campos:
  - Nome (obrigatório, único)
  - Email (obrigatório, único)
  - Status (checkbox)
- Ao criar: associar ou criar admin do vetor

**Validações:**
- Nome e email únicos
- Deve ter ao menos 1 admin ativo

**Componentes:**
- `Input` (nome, email)
- `Checkbox` (ativo)
- `Button`

### Controle de Permissões

**AdminGlobal:**
- Acesso total a usuários e vetores
- Pode criar/editar/inativar qualquer entidade

**AdminVetor:**
- Vê apenas seu próprio vetor
- Pode gerenciar usuários do seu vetor
- Não pode criar novos vetores

**Operador:**
- Não tem acesso a usuários e vetores

### Critérios de Aceitação
- [ ] Lista de usuários exibe apenas usuários permitidos conforme perfil
- [ ] Formulário de usuário valida todas as regras de negócio
- [ ] AdminGlobal consegue criar e gerenciar vetores
- [ ] AdminVetor não vê outros vetores
- [ ] Inativação de usuário/vetor exige confirmação
- [ ] Filtros e paginação funcionam corretamente
- [ ] Toast exibe sucesso/erro em todas as operações

### Arquivos a Criar
```
src/
  pages/
    DashboardPage.tsx
    users/
      UsersListPage.tsx
      UserFormPage.tsx
    vectors/
      VectorsListPage.tsx
      VectorFormPage.tsx
```

---

## 🔥 Entregável 06 - Gestão de Parceiros e Tipos de Negócio

### Objetivo
Implementar CRUD de parceiros com árvore hierárquica e gestão de tipos de negócio.

### Use Cases Cobertos
- **UC-30 a UC-35**: Gestão de Parceiros
- **UC-40 a UC-44**: Gestão de Tipos de Negócio

### Páginas a Criar

#### 6.1. Lista de Parceiros
**Arquivo:** `src/pages/partners/PartnersListPage.tsx`

**Funcionalidades:**
- Tabela com todos os parceiros do vetor
- Colunas: Nome, Contato, Recomendador, Status, Data Cadastro
- Filtros: Nome, Status, Recomendador
- Paginação
- Botão "Novo Parceiro"
- Botão "Ver Árvore"
- Ações: Editar, Ativar/Inativar

**Componentes:**
- `Table<Partner>`
- `Input` (filtros)
- `Select` (status)
- `Badge` (status)
- `Pagination`
- `Button`

#### 6.2. Formulário de Parceiro
**Arquivo:** `src/pages/partners/PartnerFormPage.tsx`

**Funcionalidades:**
- Criar/Editar parceiro
- Campos:
  - Nome (obrigatório)
  - Contato (email/telefone)
  - Recomendador (select: Vetor ou outro Parceiro)
  - Status (checkbox)

**Validações:**
- Recomendador deve pertencer ao mesmo vetor
- Recomendador deve estar ativo
- Não pode criar ciclo na árvore
- Nome obrigatório

**Regras Especiais:**
- Se não selecionar recomendador → Vetor é o recomendador
- Exibir hierarquia do recomendador selecionado

**Componentes:**
- `Input` (nome, contato)
- `Select` (recomendador - busca com filtro)
- `Checkbox` (ativo)
- `Alert` (aviso sobre recomendador)
- `Button`

#### 6.3. Visualização de Árvore de Parceiros
**Arquivo:** `src/pages/partners/PartnerTreePage.tsx`

**Funcionalidades:**
- Exibição hierárquica da árvore de parceiros
- Componente de árvore visual (tree view)
- Níveis identificados (1, 2, 3, 3+)
- Informações por nó:
  - Nome
  - Quantidade de recomendados
  - Status
- Filtro por parceiro específico (mostrar apenas sua sub-árvore)
- Expansão/colapso de nós

**Componente Especial:**
- `PartnerTreeView` (novo componente de árvore)

**Layout:**
```
Vetor
  ├── Parceiro A (Nível 1)
  │   ├── Parceiro B (Nível 2)
  │   │   └── Parceiro C (Nível 3)
  │   └── Parceiro D (Nível 2)
  └── Parceiro E (Nível 1)
```

#### 6.4. Lista de Tipos de Negócio
**Arquivo:** `src/pages/business-types/BusinessTypesListPage.tsx`

**Funcionalidades:**
- Tabela com todos os tipos
- Colunas: Nome, Descrição, Status
- Paginação
- Botão "Novo Tipo"
- Ações: Editar, Ativar/Inativar

**Componentes:**
- `Table<BusinessType>`
- `Badge` (status)
- `Button`
- `Pagination`

#### 6.5. Formulário de Tipo de Negócio
**Arquivo:** `src/pages/business-types/BusinessTypeFormPage.tsx`

**Funcionalidades:**
- Criar/Editar tipo de negócio
- Campos:
  - Nome (obrigatório, único)
  - Descrição (opcional, textarea)
  - Status (checkbox)

**Componentes:**
- `Input` (nome)
- `Textarea` (descrição)
- `Checkbox` (ativo)
- `Button`

### Controle de Permissões

**AdminGlobal:**
- Vê parceiros de todos os vetores
- Pode criar/editar tipos de negócio globais

**AdminVetor / Operador:**
- Vê apenas parceiros do seu vetor
- Pode criar/editar tipos de negócio

### Critérios de Aceitação
- [ ] Lista de parceiros filtra por vetor do usuário
- [ ] Formulário valida regras de recomendador
- [ ] Árvore de parceiros exibe hierarquia corretamente
- [ ] Não permite criar ciclos na árvore
- [ ] Parceiro inativo não pode ser recomendador
- [ ] CRUD de tipos de negócio funciona completamente
- [ ] Todos os toasts e confirmações implementados

### Arquivos a Criar
```
src/
  pages/
    partners/
      PartnersListPage.tsx
      PartnerFormPage.tsx
      PartnerTreePage.tsx
    business-types/
      BusinessTypesListPage.tsx
      BusinessTypeFormPage.tsx
  components/
    common/
      PartnerTreeView/ (novo componente especial)
        PartnerTreeView.tsx
        index.ts
```

---

## 🔥 Entregável 07 - Gestão de Negócios e Comissões

### Objetivo
Implementar o core do sistema: cadastro de negócios com cálculo automático de comissões e gestão de pagamentos.

### Use Cases Cobertos
- **UC-50 a UC-54**: Gestão de Negócios
- **UC-60 a UC-62**: Gestão de Pagamentos

### Páginas a Criar

#### 7.1. Lista de Negócios
**Arquivo:** `src/pages/business/BusinessListPage.tsx`

**Funcionalidades:**
- Tabela com todos os negócios do vetor
- Colunas: ID, Parceiro, Tipo, Valor, Data, Comissão Total, Status
- Filtros:
  - Parceiro (select com busca)
  - Tipo de Negócio (select)
  - Data Início/Fim (date range)
  - Status (Ativo/Cancelado)
  - Valor mínimo/máximo
- Ordenação por data, valor
- Paginação
- Botão "Novo Negócio"
- Ações por linha:
  - Ver Detalhes
  - Ver Pagamentos
  - Editar
  - Cancelar

**Componentes:**
- `Table<Business>`
- `Input` (filtros de data, valor)
- `Select` (parceiro, tipo, status)
- `Badge` (status - ativo em verde, cancelado em vermelho)
- `Pagination`
- `Button`

**Destaque Visual:**
- Negócios cancelados em cor diferente (texto cinza)
- Valor e comissão formatados como moeda

#### 7.2. Formulário de Negócio
**Arquivo:** `src/pages/business/BusinessFormPage.tsx`

**Funcionalidades:**
- Criar/Editar negócio
- **Modo Criação:**
  - Parceiro (select obrigatório)
  - Tipo de Negócio (select obrigatório)
  - Valor (input number, obrigatório, > 0)
  - Data (date picker, default hoje)
  - Observações (textarea opcional)

- **Modo Edição:**
  - Apenas Observações editável
  - Demais campos readonly
  - Exibir aviso: "Comissão já calculada, não pode alterar valores críticos"

**Cálculo Automático:**
- Exibir preview da comissão (10% do valor)
- Após criar: sistema calcula e cria pagamentos automaticamente

**Validações:**
- Parceiro ativo e do mesmo vetor
- Tipo de negócio ativo
- Valor > 0

**Fluxo de Criação:**
```
1. Preencher formulário
2. Exibir preview: "Comissão Total: R$ XXX"
3. Submit
4. Backend cria negócio
5. Backend calcula e cria pagamentos
6. Redirecionar para lista de pagamentos do negócio
7. Toast de sucesso
```

**Componentes:**
- `Input` (valor, data)
- `Select` (parceiro, tipo)
- `Textarea` (observações)
- `Card` (preview da comissão)
- `Alert` (avisos)
- `Button`

#### 7.3. Detalhes do Negócio
**Arquivo:** `src/pages/business/BusinessDetailPage.tsx`

**Funcionalidades:**
- Exibir todos os dados do negócio
- Seção: Dados do Negócio
  - ID, Data, Parceiro, Tipo, Valor, Status
- Seção: Comissões Geradas
  - Tabela com todos os pagamentos
  - Colunas: Destinatário, Nível, Valor, Status, Data Pagamento
  - Total Pago / Total Pendente
- Botões:
  - Editar (se ativo)
  - Cancelar (se ativo)
  - Voltar

**Destaque:**
- Card separado para cada seção
- Resumo financeiro destacado

**Componentes:**
- `Card` (seções)
- `Table<Payment>` (pagamentos)
- `Badge` (status)
- `Button`
- `ConfirmDialog` (cancelar)

#### 7.4. Lista de Pagamentos
**Arquivo:** `src/pages/payments/PaymentsListPage.tsx`

**Funcionalidades:**
- Tabela com todos os pagamentos
- Colunas: ID, Destinatário, Negócio ID, Valor, Nível, Status, Data Pagamento
- Filtros:
  - Destinatário (input busca)
  - Status (Pendente/Pago)
  - Nível (1/2/3)
  - Vetor (apenas AdminGlobal)
  - Data Início/Fim
  - Valor mínimo/máximo
- Seleção múltipla de pagamentos pendentes
- Botão "Pagar Selecionados"
- Paginação
- Ordenação por data, valor

**Resumo no Topo:**
- Total Pendente: R$ XXX
- Total Pago: R$ XXX
- Qtd Pendente: XX
- Qtd Pago: XX

**Componentes:**
- `Card` (resumo financeiro)
- `Table<Payment>` (com checkbox de seleção)
- `Input` (filtros)
- `Select` (status, nível)
- `Badge` (status, nível)
- `Pagination`
- `Button` (pagar selecionados)
- `ConfirmDialog` (confirmação de pagamento)

#### 7.5. Confirmação de Pagamento
**Componente:** `ConfirmDialog`

**Uso:**
- Ao clicar em "Pagar Selecionados"
- Exibir:
  - Quantidade de pagamentos
  - Valor total
  - Lista de destinatários
- Confirmar para processar
- Loading durante processamento
- Toast de sucesso/erro

### Controle de Permissões

**AdminGlobal:**
- Vê negócios e pagamentos de todos os vetores

**AdminVetor / Operador:**
- Vê apenas negócios e pagamentos do seu vetor
- Pode criar negócios
- Pode processar pagamentos

### Regras de Negócio Críticas

1. **Criação de Negócio:**
   - Backend calcula automaticamente os 3 níveis
   - Frontend apenas valida e envia dados básicos

2. **Cancelamento de Negócio:**
   - Marca negócio como cancelado
   - Cancela pagamentos pendentes
   - Não cancela pagamentos já pagos
   - Exibe confirmação clara

3. **Processamento de Pagamento:**
   - Apenas pagamentos com status "a pagar"
   - Registra data e usuário responsável
   - Não permite estorno (não implementar no MVP)
   - Gera log de auditoria automaticamente

4. **Filtros e Performance:**
   - Paginação obrigatória (máximo 50 por página)
   - Índices no backend para queries rápidas

### Critérios de Aceitação
- [ ] Formulário de negócio cria e calcula comissões automaticamente
- [ ] Lista de negócios exibe status e permite filtros
- [ ] Detalhes do negócio mostram todas as comissões geradas
- [ ] Cancelamento de negócio cancela pagamentos pendentes
- [ ] Lista de pagamentos permite filtros e seleção múltipla
- [ ] Pagamento múltiplo processa corretamente e exibe confirmação
- [ ] Resumo financeiro sempre atualizado
- [ ] Todas as operações geram toast de feedback
- [ ] Validações impedem operações inválidas

### Arquivos a Criar
```
src/
  pages/
    business/
      BusinessListPage.tsx
      BusinessFormPage.tsx
      BusinessDetailPage.tsx
    payments/
      PaymentsListPage.tsx
```

---

## 🔥 Entregável 08 - Relatórios e Dashboard

### Objetivo
Implementar relatórios analíticos e dashboard com indicadores principais do sistema.

### Use Cases Cobertos
- **UC-70**: Relatório de Parceiros
- **UC-71**: Relatório Financeiro
- **UC-72**: Relatório de Negócios

### Páginas a Criar

#### 8.1. Dashboard Aprimorado
**Arquivo:** `src/pages/DashboardPage.tsx` (ATUALIZAR)

**Funcionalidades:**
- **Seção: Visão Geral**
  - Cards com métricas principais:
    - Total de Parceiros Ativos
    - Total de Negócios (mês atual)
    - Total em Comissões Pendentes
    - Total em Comissões Pagas (mês atual)

- **Seção: Negócios Recentes**
  - Tabela com últimos 10 negócios
  - Link para "Ver Todos"

- **Seção: Pagamentos Pendentes**
  - Tabela com próximos pagamentos
  - Ação rápida de pagar

- **Seção: Árvore de Parceiros** (se AdminVetor/Operador)
  - Indicador de níveis
  - Link para árvore completa

**Componentes:**
- `Card` (métricas)
- `Table` (negócios, pagamentos)
- `Badge`
- `Button`

#### 8.2. Relatório de Parceiros
**Arquivo:** `src/pages/reports/PartnersReportPage.tsx`

**Funcionalidades:**
- **Filtros:**
  - Vetor (apenas AdminGlobal)
  - Status (Ativo/Inativo/Todos)
  - Data Cadastro (Início/Fim)

- **Exibição:**
  - Tabela com parceiros
  - Colunas:
    - Nome
    - Nível na Árvore
    - Qtd de Recomendados
    - Total Recebido (histórico)
    - Total a Receber (pendente)
    - Status
  - Ordenação por qualquer coluna
  - Paginação

- **Resumo no Topo:**
  - Total de Parceiros
  - Total Ativos
  - Total Inativos
  - Total de Recomendações

- **Exportação:**
  - Botão "Exportar CSV" (opcional MVP)

**Componentes:**
- `Card` (resumo)
- `Table<PartnerReportData>`
- `Input` (filtros de data)
- `Select` (vetor, status)
- `Button` (exportar)
- `Pagination`

#### 8.3. Relatório Financeiro
**Arquivo:** `src/pages/reports/FinancialReportPage.tsx`

**Funcionalidades:**
- **Filtros:**
  - Vetor (apenas AdminGlobal)
  - Período (Início/Fim)
  - Status (Pago/Pendente/Todos)
  - Nível (1/2/3/Todos)
  - Parceiro (select com busca)

- **Resumo Geral:**
  - Total Pago no Período
  - Total Pendente
  - Total por Nível:
    - Nível 1: R$ XXX
    - Nível 2: R$ XXX
    - Nível 3: R$ XXX
  - Total por Vetor (se AdminGlobal)

- **Gráficos (Opcional MVP):**
  - Pizza: Distribuição por Nível
  - Barras: Evolução Mensal

- **Tabela Detalhada:**
  - Colunas:
    - Data
    - Destinatário
    - Negócio ID
    - Valor
    - Nível
    - Status
  - Paginação
  - Ordenação

**Componentes:**
- `Card` (resumos)
- `Table<PaymentReportData>`
- `Input` (filtros de data)
- `Select` (filtros)
- `Badge` (status, nível)
- `Button` (exportar)
- `Pagination`

#### 8.4. Relatório de Negócios
**Arquivo:** `src/pages/reports/BusinessReportPage.tsx`

**Funcionalidades:**
- **Filtros:**
  - Vetor (apenas AdminGlobal)
  - Período (Início/Fim)
  - Tipo de Negócio (select)
  - Parceiro (select com busca)
  - Status (Ativo/Cancelado/Todos)
  - Valor mínimo/máximo

- **Resumo:**
  - Total de Negócios no Período
  - Valor Total em Negócios
  - Comissão Total Gerada
  - Valor Médio por Negócio
  - Tipo de Negócio Mais Comum

- **Gráficos (Opcional MVP):**
  - Linha: Evolução de Negócios por Mês
  - Pizza: Distribuição por Tipo

- **Tabela:**
  - Colunas:
    - Data
    - Parceiro
    - Tipo
    - Valor
    - Comissão Total
    - Status dos Pagamentos
  - Indicador visual: % pago das comissões
  - Paginação

**Componentes:**
- `Card` (resumos)
- `Table<BusinessReportData>`
- `Input` (filtros)
- `Select` (filtros)
- `Badge` (status)
- `Button` (exportar)
- `Pagination`

### Controle de Permissões

**AdminGlobal:**
- Vê relatórios de todos os vetores
- Filtro de vetor disponível

**AdminVetor / Operador:**
- Vê apenas dados do seu vetor
- Filtro de vetor não aparece

### Critérios de Aceitação
- [ ] Dashboard exibe métricas atualizadas
- [ ] Relatório de parceiros calcula totais corretamente
- [ ] Relatório financeiro separa por nível e vetor
- [ ] Relatório de negócios mostra resumos precisos
- [ ] Todos os filtros funcionam corretamente
- [ ] Paginação e ordenação implementadas
- [ ] Performance adequada com grandes volumes
- [ ] Layout responsivo em todas as telas

### Arquivos a Criar/Modificar
```
src/
  pages/
    DashboardPage.tsx (ATUALIZAR)
    reports/
      PartnersReportPage.tsx
      FinancialReportPage.tsx
      BusinessReportPage.tsx
```

---

## 🔥 Entregável 09 - Auditoria e Logs

### Objetivo
Implementar visualização de logs de auditoria para rastreamento de ações críticas no sistema.

### Use Cases Cobertos
- **UC-80**: Registrar Log de Ação (automático no backend)
- **UC-81**: Consultar Logs

### Páginas a Criar

#### 9.1. Lista de Logs de Auditoria
**Arquivo:** `src/pages/audit/AuditLogsPage.tsx`

**Funcionalidades:**
- **Permissão:** Apenas AdminGlobal

- **Filtros:**
  - Usuário (select com busca)
  - Ação (select: Login, Logout, Create, Update, Delete, Payment, Cancel)
  - Entidade (select: User, Vector, Partner, Business, Payment)
  - Data Início/Fim
  - Texto livre (busca em payload)

- **Tabela:**
  - Colunas:
    - Data/Hora
    - Usuário
    - Ação
    - Entidade
    - ID Entidade
    - Detalhes (modal)
  - Ordenação por data (DESC default)
  - Paginação (50 por página)

- **Modal de Detalhes:**
  - Exibir JSON do payload formatado
  - IP do usuário (se disponível)
  - User Agent

**Componentes:**
- `Table<AuditLog>`
- `Input` (filtros)
- `Select` (filtros)
- `Modal` (detalhes)
- `Badge` (tipo de ação)
- `Pagination`
- `Button` (ver detalhes)

#### 9.2. Timeline de Auditoria por Entidade
**Arquivo:** `src/pages/audit/AuditTimelinePage.tsx`

**Funcionalidades:**
- Recebe ID de entidade na URL: `/audit/timeline/:entityType/:entityId`
- Exibe linha do tempo de todas as ações naquela entidade
- Layout vertical com cards
- Cada card mostra:
  - Data/Hora
  - Usuário
  - Ação
  - Mudanças (diff se possível)

**Uso:**
- Link "Ver Histórico" em detalhes de usuário, parceiro, negócio, etc.

**Componentes:**
- `Card` (eventos da timeline)
- `Badge` (tipo de ação)
- `Button` (voltar)

### Regras de Auditoria

**Ações Auditadas (backend já implementa):**
- Login/Logout
- Criação de entidades (User, Vector, Partner, Business, Payment)
- Atualização de entidades
- Inativação/Cancelamento
- Processamento de pagamentos

**Dados Registrados:**
- ID do usuário
- Data/Hora
- Ação realizada
- Tipo de entidade
- ID da entidade
- Payload (JSON com dados relevantes)
- IP (opcional)

### Controle de Permissões

**AdminGlobal:**
- Acesso completo aos logs
- Vê ações de todos os usuários em todos os vetores

**AdminVetor / Operador:**
- Sem acesso à auditoria (opcional: permitir ver logs do próprio vetor)

### Critérios de Aceitação
- [ ] Lista de logs exibe apenas para AdminGlobal
- [ ] Filtros funcionam corretamente
- [ ] Paginação eficiente (backend otimizado)
- [ ] Modal de detalhes exibe payload formatado
- [ ] Timeline por entidade funciona
- [ ] Ordenação por data DESC default
- [ ] Performance adequada com milhares de logs

### Arquivos a Criar
```
src/
  pages/
    audit/
      AuditLogsPage.tsx
      AuditTimelinePage.tsx
```

---

## 🔥 Entregável 10 - Refinamentos e Integração Final

### Objetivo
Polimento final, testes de integração, correções de bugs e ajustes de UX/UI para entrega do MVP.

### Atividades

#### 10.1. Revisão de UX/UI
- **Consistência Visual:**
  - Revisar todas as páginas para garantir padrão visual
  - Verificar uso correto de cores (preto/branco/cinza)
  - Padronizar espaçamentos e tamanhos
  - Garantir responsividade em todas as telas

- **Feedback ao Usuário:**
  - Todos os toasts implementados
  - Loading states em todas as requisições
  - Estados vazios (EmptyState) em todas as listas
  - Mensagens de erro claras e acionáveis

- **Navegação:**
  - Breadcrumbs em páginas de detalhe/edição
  - Botões de voltar funcionais
  - Links de navegação rápida no dashboard
  - Menu lateral com indicadores ativos

#### 10.2. Validações e Tratamento de Erros
- **Formulários:**
  - Todas as validações do Zod implementadas
  - Mensagens de erro traduzidas e claras
  - Focus automático em campo com erro
  - Disabled states durante submit

- **Requisições:**
  - Tratamento de 401 (token expirado) → refresh automático
  - Tratamento de 403 (sem permissão) → mensagem e redirect
  - Tratamento de 404 → mensagem "não encontrado"
  - Tratamento de 500 → mensagem genérica de erro
  - Retry automático em falhas de rede (React Query)

- **ErrorBoundary:**
  - Captura erros não tratados
  - Exibe página de erro amigável
  - Botão de "Tentar Novamente"
  - Log de erros no console (dev mode)

#### 10.3. Testes de Integração
- **Fluxos Principais:**
  - Login → Dashboard → Criar Parceiro → Criar Negócio → Ver Comissões → Pagar
  - Login como AdminGlobal → Criar Vetor → Criar AdminVetor → Logout → Login como AdminVetor
  - Filtros e paginação em todas as listas
  - Edição de entidades existentes
  - Cancelamento de negócios
  - Relatórios com filtros

- **Permissões:**
  - AdminGlobal vê tudo
  - AdminVetor vê apenas seu vetor
  - Operador não vê usuários/vetores
  - Rotas protegidas redirecionam corretamente

- **Edge Cases:**
  - Parceiro sem recomendador (vetor recebe tudo)
  - Árvore com apenas 1 ou 2 níveis
  - Negócio de valor R$ 0,01 (mínimo)
  - Lista vazia em todas as telas
  - Timeout de requisição

#### 10.4. Performance
- **Otimizações:**
  - React Query cache configurado corretamente
  - Invalidação de queries após mutações
  - Lazy loading de rotas (React.lazy + Suspense)
  - Debounce em filtros de busca
  - Paginação obrigatória em listas grandes

- **Métricas:**
  - Tempo de carregamento inicial < 3s
  - Transições suaves entre páginas
  - Requisições de lista < 1s
  - Sem flickering de loading

#### 10.5. Documentação
- **README do Frontend:**
  - Como rodar o projeto
  - Variáveis de ambiente
  - Scripts disponíveis
  - Estrutura de pastas
  - Convenções de código

- **Documentação de Componentes:**
  - Props e tipos documentados
  - Exemplos de uso dos componentes comuns

- **Guia de Deploy:**
  - Build de produção
  - Configuração de CORS
  - Variáveis de ambiente de produção

#### 10.6. Ajustes Finais
- **Correções de Bugs:**
  - Lista de bugs encontrados durante testes
  - Priorização e correção

- **Melhorias de Código:**
  - Remover console.logs
  - Remover código comentado
  - Organizar imports
  - Verificar TypeScript strict mode

- **Acessibilidade (MVP Básico):**
  - Labels em inputs
  - Alt text em imagens/ícones
  - Navegação por teclado funcional
  - Contraste adequado

### Critérios de Aceitação
- [ ] Todas as páginas seguem o mesmo padrão visual
- [ ] Todos os toasts e feedbacks implementados
- [ ] Tratamento de erros completo
- [ ] Fluxos principais testados e funcionais
- [ ] Performance adequada
- [ ] README e documentação básica criados
- [ ] Build de produção funcional
- [ ] Zero erros de TypeScript
- [ ] Zero warnings críticos

### Tarefas
```
1. Revisar todas as 20+ páginas criadas
2. Testar todos os fluxos principais
3. Corrigir bugs encontrados
4. Otimizar queries pesadas
5. Implementar lazy loading
6. Escrever README
7. Preparar build de produção
8. Deploy de teste
```

---

## 📊 Resumo Executivo

### Estatísticas do MVP

| Categoria | Quantidade |
|-----------|-----------|
| **Entregáveis Totais** | 10 |
| **Use Cases Cobertos** | 35 |
| **Páginas a Criar** | 24 |
| **Componentes Comuns** | 19 (já criados) |
| **Componentes Especiais** | 2 (PartnerTreeView, Timeline) |
| **Rotas Protegidas** | 22+ |
| **Endpoints Backend** | 40+ (já implementados) |

### Tempo Estimado Total

| Entregável | Horas |
|-----------|-------|
| 01 - Setup | ✅ 2h |
| 02 - Base | ✅ 6h |
| 03 - Componentes | ✅ 12h |
| 04 - Autenticação | 8h |
| 05 - Usuários/Vetores | 16h |
| 06 - Parceiros/Tipos | 12h |
| 07 - Negócios/Pagamentos | 16h |
| 08 - Relatórios | 12h |
| 09 - Auditoria | 6h |
| 10 - Refinamentos | 10h |
| **TOTAL** | **100 horas** |

### Estrutura Final de Arquivos

```
frontend/
├── public/
├── src/
│   ├── api/
│   │   ├── axios.config.ts ✅
│   │   └── endpoints/ ✅
│   │       ├── auth.api.ts
│   │       ├── users.api.ts
│   │       ├── vectors.api.ts
│   │       ├── partners.api.ts
│   │       ├── business.api.ts
│   │       ├── businessTypes.api.ts
│   │       ├── payments.api.ts
│   │       ├── reports.api.ts
│   │       └── audit.api.ts
│   ├── components/
│   │   ├── common/ ✅
│   │   │   ├── Button/
│   │   │   ├── Input/
│   │   │   ├── Select/
│   │   │   ├── Textarea/
│   │   │   ├── Checkbox/
│   │   │   ├── Radio/
│   │   │   ├── Table/
│   │   │   ├── Modal/
│   │   │   ├── Pagination/
│   │   │   ├── Badge/
│   │   │   ├── Card/
│   │   │   ├── Loading/
│   │   │   ├── Alert/
│   │   │   ├── Toast/
│   │   │   ├── ConfirmDialog/
│   │   │   ├── EmptyState/
│   │   │   ├── ErrorBoundary/
│   │   │   └── PartnerTreeView/ (novo)
│   │   └── layout/ ✅
│   │       ├── Header/
│   │       ├── Sidebar/
│   │       ├── Footer/
│   │       ├── Layout/
│   │       └── PublicLayout/ (novo)
│   ├── pages/
│   │   ├── auth/
│   │   │   └── LoginPage.tsx (novo)
│   │   ├── DashboardPage.tsx (novo)
│   │   ├── users/
│   │   │   ├── UsersListPage.tsx (novo)
│   │   │   └── UserFormPage.tsx (novo)
│   │   ├── vectors/
│   │   │   ├── VectorsListPage.tsx (novo)
│   │   │   └── VectorFormPage.tsx (novo)
│   │   ├── partners/
│   │   │   ├── PartnersListPage.tsx (novo)
│   │   │   ├── PartnerFormPage.tsx (novo)
│   │   │   └── PartnerTreePage.tsx (novo)
│   │   ├── business-types/
│   │   │   ├── BusinessTypesListPage.tsx (novo)
│   │   │   └── BusinessTypeFormPage.tsx (novo)
│   │   ├── business/
│   │   │   ├── BusinessListPage.tsx (novo)
│   │   │   ├── BusinessFormPage.tsx (novo)
│   │   │   └── BusinessDetailPage.tsx (novo)
│   │   ├── payments/
│   │   │   └── PaymentsListPage.tsx (novo)
│   │   ├── reports/
│   │   │   ├── PartnersReportPage.tsx (novo)
│   │   │   ├── FinancialReportPage.tsx (novo)
│   │   │   └── BusinessReportPage.tsx (novo)
│   │   └── audit/
│   │       ├── AuditLogsPage.tsx (novo)
│   │       └── AuditTimelinePage.tsx (novo)
│   ├── routes/
│   │   ├── index.tsx ✅ (atualizar)
│   │   ├── PrivateRoute.tsx ✅
│   │   └── PermissionRoute.tsx ✅
│   ├── store/
│   │   └── auth.store.ts ✅
│   ├── types/ ✅
│   │   ├── auth.types.ts
│   │   ├── user.types.ts
│   │   ├── vector.types.ts
│   │   ├── partner.types.ts
│   │   ├── business.types.ts
│   │   ├── payment.types.ts
│   │   ├── report.types.ts
│   │   └── common.types.ts
│   ├── utils/
│   │   ├── formatters.ts (novo - formatação de moeda, data)
│   │   └── validators.ts (novo - validações customizadas)
│   ├── App.tsx ✅
│   ├── main.tsx ✅
│   └── index.css ✅
├── .env (novo)
├── .env.example (novo)
├── .gitignore ✅
├── package.json ✅
├── tsconfig.json ✅
├── vite.config.ts ✅
├── tailwind.config.js ✅
└── README.md (atualizar)
```

---

## 🎯 Ordem de Implementação Recomendada

### Fase 1: Autenticação e Base (Semana 1)
1. ✅ Entregável 01 - Setup
2. ✅ Entregável 02 - Base
3. ✅ Entregável 03 - Componentes
4. Entregável 04 - Autenticação

**Milestone:** Usuário consegue fazer login e acessar sistema

### Fase 2: Cadastros Básicos (Semana 2)
5. Entregável 05 - Usuários e Vetores
6. Entregável 06 - Parceiros e Tipos

**Milestone:** Sistema permite gerenciar toda a estrutura de cadastros

### Fase 3: Core do Negócio (Semana 3)
7. Entregável 07 - Negócios e Pagamentos

**Milestone:** Sistema calcula e gerencia comissões automaticamente

### Fase 4: Análise e Finalização (Semana 4)
8. Entregável 08 - Relatórios
9. Entregável 09 - Auditoria
10. Entregável 10 - Refinamentos

**Milestone:** MVP completo e pronto para uso

---

## 🔒 Controle de Qualidade

### Checklist por Entregável

Cada entregável deve passar por:

- [ ] Implementação completa das funcionalidades
- [ ] Validação TypeScript (0 erros)
- [ ] Testes manuais de fluxo
- [ ] Verificação de permissões
- [ ] Tratamento de erros
- [ ] Feedback visual (toasts, loading)
- [ ] Responsividade básica
- [ ] Code review (se em equipe)

### Padrões de Código

**TypeScript:**
- Strict mode habilitado
- Tipos explícitos em props
- Interfaces para objetos complexos
- Enums para valores fixos

**React:**
- Functional components
- Hooks personalizados quando necessário
- React Query para servidor state
- Zustand para client state

**Estilos:**
- Tailwind CSS classes
- Sem CSS customizado exceto necessário
- Padrão de cores: preto, branco, cinza

**Nomenclatura:**
- PascalCase: componentes, tipos, interfaces
- camelCase: funções, variáveis
- kebab-case: arquivos de componentes (pasta)
- UPPER_CASE: constantes

### Git Flow

```
main
  └── develop
       ├── feature/entregavel-04-autenticacao
       ├── feature/entregavel-05-usuarios-vetores
       ├── feature/entregavel-06-parceiros
       ├── feature/entregavel-07-negocios
       ├── feature/entregavel-08-relatorios
       ├── feature/entregavel-09-auditoria
       └── feature/entregavel-10-refinamentos
```

---

## 🚀 Preparação para Deploy

### Variáveis de Ambiente

**`.env.example`:**
```env
VITE_API_BASE_URL=http://localhost:5000/api
VITE_APP_NAME=Sistema de Rede de Credenciamento
VITE_APP_VERSION=1.0.0
```

**Produção:**
```env
VITE_API_BASE_URL=https://api.seudominio.com.br/api
VITE_APP_NAME=Sistema de Rede de Credenciamento
VITE_APP_VERSION=1.0.0
```

### Build de Produção

```bash
npm run build
```

### Deploy Sugerido

**Opções:**
- Vercel (frontend)
- Netlify (frontend)
- Azure Static Web Apps
- AWS S3 + CloudFront

**Backend:**
- Azure App Service
- AWS Elastic Beanstalk
- Heroku (dev/staging)

---

## 📚 Recursos e Referências

### Documentação
- [React Query](https://tanstack.com/query/latest)
- [React Hook Form](https://react-hook-form.com/)
- [Zustand](https://zustand-demo.pmnd.rs/)
- [Tailwind CSS](https://tailwindcss.com/)
- [React Router](https://reactrouter.com/)

### Ferramentas
- VS Code + extensões (Prettier, ESLint, Tailwind IntelliSense)
- Postman (testar API)
- React DevTools
- Redux DevTools (para Zustand)

---

## ✅ Conclusão

Este plano de entregáveis cobre **100% dos use cases** do MVP, organizados em **10 entregáveis** sequenciais e lógicos.

**Total de Páginas:** 24  
**Total de Componentes:** 21  
**Total de Use Cases:** 35  
**Tempo Estimado:** 100 horas (4 semanas - 1 dev full-time)

Cada entregável é **independente e testável**, permitindo validação incremental e feedback contínuo.

**Status Atual:** 
- ✅ Entregável 01, 02, 03 (30% do MVP)
- 🚀 Próximo: Entregável 04 - Autenticação

---

**Última Atualização:** 15/12/2025  
**Versão do Documento:** 1.0  
**Responsável:** GitHub Copilot
