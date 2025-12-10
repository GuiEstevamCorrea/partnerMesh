<!-- Baseado no Projeto ponto MD vamos fazer por partes. Seguinto a arquitetura hexagonal, com a estrutura de projeto que já existe. -->

# Projeto **Sistema de Rede de Credenciamento / Vetores**

[Rede de credenciados Vetor.xlsx](attachment:14dcdb0e-a73f-4053-b622-c3d66e5fdc95:Rede_de_credenciados_Vetor.xlsx)

[Tabela de comissionamento - Vetor.xlsx](attachment:5c978af9-79d5-410b-a091-4a0e08608916:Tabela_de_comissionamento_-_Vetor.xlsx)

# **LEVANTAMENTO DE REQUISITOS**

## **1. Objetivo Geral**

Gerenciar uma rede de parceiros recomendados em até 3 níveis, registrar negócios de diferentes tipos, calcular automaticamente as comissões e controlar o pagamento dessas comissões.

O sistema inicia com **um vetor principal**, mas já preparado para suportar **múltiplos vetores independentes**, cada um com sua própria árvore de parceiros e regras personalizáveis no futuro.

---

# **2. Entidades Principais**

## **2.1 Usuário**

Representa quem acessa o sistema.

- Nome
- Email (login)
- Senha (hash)
- Perfil de acesso:
    - **Admin global** (gestor do sistema)
    - **Admin de vetor**
    - **Operador de vetor**
    - **Parceiro** (acesso limitado ao próprio painel, se aplicável futuramente)
- Status (ativo/inativo)
- Vetor associado (somente quando usuário é admin/operador de um vetor)

**Regras:**

- Um usuário admin global pode gerenciar todos os vetores.
- Administradores de vetor só enxergam e gerenciam seu próprio vetor.
- Operadores podem cadastrar parceiros, negócios e pagar comissões dentro do vetor.
- Futuramente, parceiros poderão ter login para ver ganhos → já deixado preparado.

---

## **2.2 Vetor**

- Nome
- E-mail
- Login gerencial
- Status (ativo/inativo)
- Configurações próprias (ex.: percentuais, futuro)

Cada vetor tem sua própria árvore de parceiros.

---

## **2.3 Parceiro**

- Nome
- Contato
- Status (ativo/inativo)
- **Recomendador** (outro parceiro ou vetor)
- Vetor ao qual pertence
- Data de cadastro

---

## **2.4 Tipo de Negócio**

- Nome
- Descrição
- Configurações futuras específicas por tipo

---

## **2.5 Negócio**

- Parceiro que fechou
- Tipo
- Valor
- Data
- Observações
- Status (ativo/cancelado)

---

## **2.6 Comissão**

- Negócio associado
- Valor total (10% do valor do negócio)
- Gerada automaticamente

### **Comissão por pessoa (destinatários)**

- Parceiro ou vetor
- Valor
- Nível (1, 2 ou 3)
- Status: *a pagar / pago*
- Data do pagamento

---

# **3. Regras de Negócio**

## **3.1 Profundidade máxima de distribuição**

A rede pode ter profundidade arbitrária, mas **distribuição de comissões ocorre somente até 3 níveis**.

---

## **3.2 Comissão total**

Sempre baseada em:

```
Comissão_total = Valor_negócio × 0.10

```

---

# **4. Regras de Distribuição da Comissão**

### **Nível 1**

- Vetor: 50%
- Recomendador direto: 50%

### **Nível 2**

- Vetor: 15%
- Level 0 (você): 35%
- Recomendador intermediário: 50%

### **Nível 3**

- Vetor: 10%
- Level 0 (você): 15%
- Nível 1 intermediário: 25%
- Nível 2 intermediário: 50%

---

# **5. Regras Financeiras**

Cada comissão gera pagamentos individuais com:

- Valor
- Destinatário
- Status
- Data de pagamento

Relatórios permitem filtros diversos (pendentes, pagos, períodos, vetores, parceiros).

---

# **6. Fluxos Funcionais (VERSÃO DETALHADA)**

A seção abaixo descreve todos os fluxos operacionais do sistema, incluindo regras, validações, exceções, eventos e responsabilidades por perfil de usuário.

**Este é o coração funcional do projeto.**

---

# **6.1 Cadastro e Gestão de Usuários**

### **Quem pode fazer**

- Admin Global
- Admin de Vetor (somente para seu próprio vetor)

### **Campos**

- Nome
- Email
- Senha (hashada)
- Perfil (admin global, admin de vetor, operador, parceiro – futuro)
- Vetor associado (obrigatório exceto para admin global)
- Status (ativo/inativo)

### **Validações**

- Email deve ser único.
- Usuários de vetor devem ter *exclusivamente* 1 vetor vinculado.
- Admin global não pode ter vetor vinculado.
- Um vetor deve sempre ter ao menos 1 admin (regra opcional).

### **Ações permitidas**

- Criar usuário
- Editar dados
- Alterar perfil
- Associar/alterar vetor
- Resetar senha
- Ativar/inativar usuário

---

# **6.2 Autenticação e Sessão**

### **Entrada**

- Email
- Senha

### **Processo**

1. Valida credenciais.
2. Gera JWT contendo:
    - ID do usuário
    - Nome
    - Perfil
    - Vetor vinculado
    - Permissões
    - Data de expiração

### **Recursos**

- Renovação automática do token quando prestes a expirar (refresh token opcional).
- Logout (invalidar refresh token).

### **Falhas**

- Email não cadastrado
- Senha incorreta
- Usuário inativo
- Vetor inativo

---

# **6.3 Controle de Acesso (Autorização)**

Todos os endpoints da API respeitam o seguinte modelo de permissão:

### **Perfis e poderes**

| Perfil | Permissões |
| --- | --- |
| **Admin Global** | Acesso total a todos os vetores e dados |
| **Admin de Vetor** | Acesso total apenas ao próprio vetor |
| **Operador** | Gerencia parceiros, negócios e pagamentos do vetor |
| **Parceiro (futuro)** | Vê seus ganhos, seus negócios e sua árvore |

---

# **6.4 Cadastro de Vetor (MULTITENANCY)**

### **Quem pode fazer**

- **Apenas Admin Global**

### **Fluxo**

1. Criar vetor
2. Definir nome, email, status
3. Criar usuário administrador desse vetor ou associar um existente
4. (Futuro) Definir regras personalizadas de comissão

### **Validações**

- Nome único
- Email único
- Um vetor sempre deve ter **pelo menos um admin de vetor** (obrigatório)

### **Comportamento interno**

- A criação de um vetor cria:
    - Base lógica isolada para árvore
    - Permissão exclusiva aos administradores desse vetor
- No futuro:
    - Percentuais configuráveis por vetor
    - Stripe/PayPal para cobrança SaaS
    - Domínio próprio do cliente

---

# **6.5 Cadastro de Parceiro**

### **Quem pode fazer**

- Admin de Vetor
- Operador

### **Campos**

- Nome
- Contato
- Recomendador (parceiro ou vetor)
- Status
- Data de cadastro

### **Fluxo**

1. Selecionar o recomendador dentro do mesmo vetor
2. Validar:
    - Se o recomendador pertence ao mesmo vetor
    - Se não cria ciclo
    - Se existe
3. Salvar novo parceiro

### **Comportamentos internos**

- Parceiro entra na árvore imediatamente
- A profundidade real pode ser superior a 3; apenas **a distribuição** é limitada a 3 níveis

### **Exceções**

- Recomendador fora do vetor → erro
- Parceiro inativo não pode ser recomendador
- Ciclo detectado → erro

---

# **6.6 Cadastro de Tipo de Negócio**

### **Quem pode fazer**

- Admin Global
- Admin de Vetor

### **Campos**

- Nome
- Descrição
- Regras específicas futuras

### **Fluxo**

- CRUD simples

---

# **6.7 Cadastro de Negócio (PROCESSAMENTO DE COMISSÕES)**

### **Quem pode fazer**

- Admin de Vetor
- Operador

### **Campos**

- Parceiro que fechou
- Tipo de negócio
- Valor
- Data
- Observações

### **Fluxo detalhado**

1. Usuário seleciona o parceiro que fechou.
2. Sistema identifica recomendadores ascendentes:
    - Nível 1: recomendador direto
    - Nível 2: recomendador do recomendador
    - Nível 3: recomendado do nível 2
    - Vetor
3. Calcula comissão total:
    
    ```
    total = valor × 0.10
    
    ```
    
4. Aplica distribuição de percentuais por nível (detalhada no item 4).
5. Para cada pessoa elegível:
    - Cria um registro de pagamento individual
    - Status = “a pagar”
    - Valor = percentual x total
    - Guarda o nível

### **Validações**

- Parceiro pertence ao vetor
- Parceiro está ativo
- Tipo de negócio existe
- Valor > 0

### **Exceções**

- Parceiro sem recomendador → sistema trata como “vetor recebe tudo”
- Árvore com menos de 3 níveis → distribui apenas o que existe

---

# **6.8 Gestão de Pagamentos de Comissão**

### **Quem pode fazer**

- Admin de Vetor
- Operador

### **Fluxo**

1. Usuário acessa lista de pagamentos pendentes
2. Filtra por parceiro, vetor, período
3. Seleciona 1 ou mais pagamentos
4. Confirma pagamento
5. Sistema:
    - marca registro como “pago”
    - grava data/hora
    - grava usuário responsável
    - gera log de auditoria

### **Regras**

- Não existe estorno (fase 2)
- Pagamentos são individuais, não agrupados
- Futuros:
    - PIX automático
    - Exportação CNAB
    - Auditoria financeira

---

# **6.9 Relatórios**

Cada relatório possui:

- filtros
- paginação
- ordenação
- exportação (CSV, PDF na fase 2)

### **6.9.1 Relatório de Parceiros**

- Árvore completa do vetor
- Quantidade de recomendados por nível
- Total recebido
- Total a receber
- Ativos / inativos

### **6.9.2 Relatório Financeiro**

Filtros:

- Pagos / pendentes
- Período
- Parceiro
- Vetor
- Tipo de negócio

Indicadores:

- Total pago no mês
- Total pendente
- Total por nível
- Total por vetor

### **6.9.3 Relatório de Negócios**

Filtros:

- Tipo
- Parceiro
- Valor
- Período

Informações:

- Parceiro que fechou
- Valor
- Comissão total
- Status dos pagamentos

---

# **7. Requisitos Não Funcionais (DETALHADOS)**

### **7.1 Arquitetura**

- API RESTful
- .NET 8 ou superior
- DDD modular
- Repository Pattern
- Multitenancy por vetor (futuro: schema por cliente)

### **7.2 Segurança**

- JWT com refresh
- Hash de senha com BCrypt
- Logs de acesso
- Permissões por perfil

### **7.3 Banco**

- PostgreSQL
- Chaves estrangeiras obrigatórias
- Triggers de auditoria (opcional)

### **7.4 Performance**

- Paginação em todas as listagens
- Index em:
    - parceiro.vetor_id
    - negocio.parceiro_id
    - pagamento.status

### **7.5 SaaS Ready**

- Cada vetor poderá ser isolado
- Regras variáveis por vetor
- Cobrança futura

# DIAGRAMA DAS TABELAS

![Untitled diagram-2025-12-04-233418.png](attachment:a6dc9d0f-dc71-4cc0-9eb4-1adfca9a74c4:Untitled_diagram-2025-12-04-233418.png)

# **USE CASES DO SISTEMA DE REDE DE CREDENCIAMENTO / VETORES**

## **1. Autenticação e Sessão**

### **UC-01 – Autenticar Usuário - OK**

- Input: email, senha
- Output: JWT + Refresh Token
- Regras: validar credenciais, usuário ativo, vetor ativo (se existir)

### **UC-02 – Renovar Token - OK**

- Input: refresh token
- Output: novo JWT
- Regras: validade, revogação

### **UC-03 – Logout - OK**

- Invalida refresh token

---

# **2. Gestão de Usuários**

### **UC-10 – Criar Usuário - OK**

- Perfis permitidos: Admin Global, Admin de Vetor
- Regras: email único, admin global não tem vetor, outros perfis devem ter.

### **UC-11 – Atualizar Usuário - OK**

- Permite alterar nome, email, permission, vetor.

### **UC-12 – Alterar Senha - OK**

- Admin global pode forçar reset
- Usuário pode alterar sua própria

### **UC-13 – Ativar/Inativar Usuário - OK**

- Respeita restrição: vetor deve ter ao menos 1 admin

### **UC-14 – Listar Usuários - OK**

### **UC-15 – Obter Dados do Usuário por ID - OK**

---

# **3. Gestão de Vetores (Tenants)**

### **UC-20 – Criar Vetor - OK**

- Apenas Admin Global
- Regras: nome único, email único, criar admin por vetor.

### **UC-21 – Atualizar Vetor - OK**

- Nome, email, status

### **UC-22 – Inativar Vetor - OK**

- Valida se existe administrador ativo

### **UC-23 – Listar Vetores - OK**

### **UC-24 – Obter Vetor por ID - OK**

---

# **4. Gestão de Parceiros**

### **UC-30 – Criar Parceiro - OK**

- Campos: nome, contato, recomendador
- Regras:
    - recomendador pertence ao mesmo vetor
    - parceiro ativo
    - sem ciclo

### **UC-31 – Atualizar Parceiro - OK**

### **UC-32 – Ativar/Inativar Parceiro - OK**

- Parceiro inativo não pode recomendar

### **UC-33 – Listar Parceiros - OK**

- Com filtros

### **UC-34 – Obter Parceiro por ID - OK**

### **UC-35 – Obter Árvore de Parceiros - OK**

- Retorna estrutura hierárquica
- Filtrada por vetor

---

# **5. Gestão de Tipos de Negócio**

### **UC-40 – Criar Tipo de Negócio - OK**

### **UC-41 – Atualizar Tipo de Negócio - OK**

### **UC-42 – Remover/Inativar Tipo de Negócio - OK**

### **UC-43 – Listar Tipos de Negócio**

### **UC-44 – Obter Tipo por ID**

---

# **6. Gestão de Negócios**

### **UC-50 – Criar Negócio**

Fluxo completo:

1. Validar parceiro
2. Validar tipo
3. Criar negócio
4. Calcular comissão total (10%)
5. Resolver os 3 níveis de recomendadores
6. Criar os Pagamentos de Comissão “a pagar”

### **UC-51 – Atualizar Negócio**

- Somente campos não críticos
- Não recalcula comissão após criado (regra opcional)

### **UC-52 – Cancelar Negócio**

- Marca como cancelado
- Regras:
    - cancelar todos os pagamentos pendentes
    - não cancela os já pagos

### **UC-53 – Listar Negócios**

### **UC-54 – Obter Negócio por ID**

---

# **7. Gestão de Pagamentos (Comissões)**

### **UC-60 – Listar Pagamentos**

- Filtros: vetor, parceiro, período, status, nível

### **UC-61 – Efetuar Pagamento**

- Troca status de “a pagar” → “pago”
- Registra data, usuário

### **UC-62 – Buscar Pagamentos de um Negócio**

---

# **8. Relatórios**

### **UC-70 – Relatório de Parceiros**

- Árvore
- Totais por nível
- Totais recebidos e pendentes

### **UC-71 – Relatório Financeiro**

- Totais pagos
- Totais pendentes
- Totais por nível
- Totais por vetor

### **UC-72 – Relatório de Negócios**

- Filtros: tipo, parceiro, período
- Com comissão total e status

---

# **9. Auditoria / Logs**

### **UC-80 – Registrar Log de Ação**

- Cada ação crítica grava: usuário, data, operação, payload

### **UC-81 – Consultar Logs**

*(Restrito ao Admin Global)*

---

# **10. Futuro / Preparado (não implementa agora mas previsto)**

### **UC-F1 – Painel do Parceiro**

- Ver ganhos
- Ver negócios
- Ver sua árvore

### **UC-F2 – Regras customizadas por vetor**

- Percentuais editáveis

### **UC-F3 – Pagamento automático via PIX**

### **UC-F4 – Exportação CNAB / PDF**

---

# 🔥 **Resumo dos Use Cases (para organizar a arquitetura)**

| Bloco | Use Cases |
| --- | --- |
| Autenticação | UC-01 a UC-03 |
| Usuários | UC-10 a UC-15 |
| Vetores | UC-20 a UC-24 |
| Parceiros | UC-30 a UC-35 |
| Tipos de Negócio | UC-40 a UC-44 |
| Negócio + comissões | UC-50 a UC-54 |
| Pagamentos | UC-60 a UC-62 |
| Relatórios | UC-70 a UC-72 |
| Auditoria | UC-80 a UC-81 |

Total: **35 Use Cases reais**

- **4 futuros planejados**