# Git Flow - PartnerMesh

Este projeto segue o modelo **Git Flow** para gerenciamento de branches e versionamento.

## 📌 Estrutura de Branches

### Branches Principais

- **`main`**: Branch de produção. Contém apenas código estável e testado.
- **`develop`**: Branch de desenvolvimento. Integração contínua de features.

### Branches de Suporte

- **`feature/*`**: Novas funcionalidades
- **`bugfix/*`**: Correções de bugs na develop
- **`release/*`**: Preparação para nova versão
- **`hotfix/*`**: Correções urgentes em produção
- **`support/*`**: Manutenção de versões antigas

---

## 🚀 Comandos Rápidos

### Inicializar Git Flow
```bash
git flow init
```

### Features (Novas Funcionalidades)
```bash
# Criar nova feature
git flow feature start nome-da-feature

# Finalizar feature (merge para develop)
git flow feature finish nome-da-feature

# Publicar feature para colaboração
git flow feature publish nome-da-feature
```

### Releases (Preparação de Versão)
```bash
# Criar release
git flow release start 1.0.0

# Finalizar release (merge para main e develop)
git flow release finish 1.0.0
```

### Hotfixes (Correções Urgentes)
```bash
# Criar hotfix
git flow hotfix start 1.0.1

# Finalizar hotfix (merge para main e develop)
git flow hotfix finish 1.0.1
```

### Bugfixes (Correções na Develop)
```bash
# Criar bugfix
git flow bugfix start nome-do-bugfix

# Finalizar bugfix
git flow bugfix finish nome-do-bugfix
```

---

## 📋 Workflow Padrão

### 1. Nova Funcionalidade
```bash
# 1. Criar branch de feature
git flow feature start UC70-relatorio-parceiros

# 2. Desenvolver e commitar
git add .
git commit -m "feat: adicionar relatório de parceiros"

# 3. Finalizar feature
git flow feature finish UC70-relatorio-parceiros
```

### 2. Nova Versão
```bash
# 1. Criar release
git flow release start 1.1.0

# 2. Ajustes finais (bump version, changelog, etc)
git commit -am "chore: preparar release 1.1.0"

# 3. Finalizar release
git flow release finish 1.1.0

# 4. Push das branches e tags
git push origin main develop --tags
```

### 3. Correção Urgente
```bash
# 1. Criar hotfix
git flow hotfix start 1.1.1

# 2. Corrigir bug
git commit -am "fix: corrigir cálculo de comissões"

# 3. Finalizar hotfix
git flow hotfix finish 1.1.1

# 4. Push
git push origin main develop --tags
```

---

## 🏷️ Convenção de Commits (Conventional Commits)

Utilize prefixos semânticos nos commits:

- **`feat:`** Nova funcionalidade
  ```bash
  git commit -m "feat: adicionar filtro por status na lista de parceiros"
  ```

- **`fix:`** Correção de bug
  ```bash
  git commit -m "fix: corrigir cálculo de comissões para nível 3"
  ```

- **`refactor:`** Refatoração de código
  ```bash
  git commit -m "refactor: simplificar lógica de distribuição de comissões"
  ```

- **`docs:`** Documentação
  ```bash
  git commit -m "docs: atualizar README com instruções de setup"
  ```

- **`test:`** Testes
  ```bash
  git commit -m "test: adicionar testes unitários para CommissionPayment"
  ```

- **`chore:`** Manutenção/configuração
  ```bash
  git commit -m "chore: atualizar dependências do frontend"
  ```

- **`style:`** Formatação de código
  ```bash
  git commit -m "style: aplicar prettier no código frontend"
  ```

- **`perf:`** Melhoria de performance
  ```bash
  git commit -m "perf: otimizar query de busca de pagamentos"
  ```

---

## 🔄 Exemplo de Ciclo Completo

```bash
# Desenvolver feature
git flow feature start UC80-auditoria-logs
# ... desenvolver ...
git commit -m "feat: implementar sistema de auditoria"
git flow feature finish UC80-auditoria-logs

# Outra feature
git flow feature start UC81-filtros-avancados
# ... desenvolver ...
git commit -m "feat: adicionar filtros avançados na lista"
git flow feature finish UC81-filtros-avancados

# Preparar release
git flow release start 2.0.0
# ... ajustes finais ...
git commit -m "chore: preparar versão 2.0.0"
git flow release finish 2.0.0

# Push
git push origin main develop --tags

# Se necessário, hotfix
git flow hotfix start 2.0.1
git commit -m "fix: corrigir erro crítico no pagamento"
git flow hotfix finish 2.0.1
git push origin main develop --tags
```

---

## 📚 Referências

- [Git Flow Original](https://nvie.com/posts/a-successful-git-branching-model/)
- [Git Flow Cheatsheet](https://danielkummer.github.io/git-flow-cheatsheet/)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

## ⚙️ Configuração Adicional

### Instalar Git Flow
```bash
# Windows (Chocolatey)
choco install gitflow-avh

# macOS (Homebrew)
brew install git-flow-avh

# Linux (apt)
sudo apt-get install git-flow
```

### Configurar Aliases Úteis
```bash
git config --global alias.lg "log --graph --pretty=format:'%Cred%h%Creset -%C(yellow)%d%Creset %s %Cgreen(%cr) %C(bold blue)<%an>%Creset' --abbrev-commit"
git config --global alias.st "status -sb"
git config --global alias.co "checkout"
git config --global alias.br "branch"
```
