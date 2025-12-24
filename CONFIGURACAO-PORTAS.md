# Configuração de Portas - PartnerMesh

Este documento explica como configurar as portas do backend e frontend.

## Backend (API .NET)

### Configuração da Porta

A porta do backend é configurada no arquivo [`Api/appsettings.json`](Api/appsettings.json):

```json
{
  "Urls": "http://localhost:5000"
}
```

**Para alterar a porta:**

1. Edite o arquivo `Api/appsettings.json`
2. Modifique o valor de `Urls` para a porta desejada:
   ```json
   "Urls": "http://localhost:8080"
   ```

### Múltiplas URLs

Você pode configurar múltiplas URLs separadas por ponto e vírgula:

```json
"Urls": "http://localhost:5000;https://localhost:5001"
```

### Via Variável de Ambiente

Alternativamente, você pode definir a porta via variável de ambiente:

**Windows (PowerShell):**
```powershell
$env:ASPNETCORE_URLS = "http://localhost:8080"
dotnet run --project Api
```

**Linux/Mac:**
```bash
export ASPNETCORE_URLS="http://localhost:8080"
dotnet run --project Api
```

### Via Linha de Comando

```bash
dotnet run --project Api --urls "http://localhost:8080"
```

---

## Frontend (React + Vite)

### Configuração da URL da API

A URL da API é configurada no arquivo [`frontend/.env`](frontend/.env):

```env
VITE_API_BASE_URL=http://localhost:5000/api
```

**Para alterar:**

1. Edite o arquivo `frontend/.env`
2. Modifique o valor de `VITE_API_BASE_URL`:
   ```env
   VITE_API_BASE_URL=http://localhost:8080/api
   ```

⚠️ **IMPORTANTE:** Sempre inclua `/api` no final da URL, pois esse é o prefixo das rotas da API.

### Configuração da Porta do Frontend

A porta do servidor de desenvolvimento do frontend é configurada no arquivo [`frontend/vite.config.ts`](frontend/vite.config.ts):

```typescript
export default defineConfig({
  server: {
    port: 3000,  // Altere aqui
  }
})
```

---

## Exemplo Completo de Configuração

### Cenário: Backend na porta 8080, Frontend na porta 4000

#### 1. Backend (`Api/appsettings.json`):
```json
{
  "Urls": "http://localhost:8080"
}
```

#### 2. Frontend (`frontend/.env`):
```env
VITE_API_BASE_URL=http://localhost:8080/api
```

#### 3. Frontend (`frontend/vite.config.ts`):
```typescript
export default defineConfig({
  server: {
    port: 4000,
  }
})
```

---

## Checklist de Configuração

Ao alterar a porta do backend:

- [ ] Atualizar `Api/appsettings.json` → `Urls`
- [ ] Atualizar `frontend/.env` → `VITE_API_BASE_URL`
- [ ] Reiniciar o backend
- [ ] Reiniciar o frontend (Vite recarrega automaticamente o `.env`)

---

## Testando a Configuração

### 1. Verificar se o Backend está rodando

Acesse no navegador (substitua pela sua porta):
```
http://localhost:5000/swagger
```

### 2. Verificar a conexão do Frontend

Abra o console do navegador (F12) e faça login. Verifique a aba Network para confirmar que as requisições estão sendo feitas para a URL correta.

---

## Troubleshooting

### Erro: "Network Error" ou "Failed to fetch"

✅ **Solução:** Verifique se o `VITE_API_BASE_URL` no frontend corresponde à porta onde o backend está rodando.

### Erro: CORS

✅ **Solução:** Certifique-se de que o CORS está configurado corretamente no backend (`Program.cs`).

### Frontend não atualiza a URL da API

✅ **Solução:** Após alterar o `.env`, reinicie o servidor de desenvolvimento do Vite:
```bash
# Pare com Ctrl+C e rode novamente
npm run dev
```

---

## Configuração para Produção

Para produção, use variáveis de ambiente ao invés de hardcoded values:

### Backend
```bash
export ASPNETCORE_URLS="http://0.0.0.0:80"
```

### Frontend (Build Time)
```bash
VITE_API_BASE_URL=https://api.seudominio.com/api npm run build
```
