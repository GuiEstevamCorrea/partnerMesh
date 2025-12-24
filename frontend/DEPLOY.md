# Guia de Deploy em Produção

Instruções completas para realizar o deploy do frontend React em ambientes de produção.

## 📋 Índice

- [Pré-requisitos](#pré-requisitos)
- [Build de Produção](#build-de-produção)
- [Variáveis de Ambiente](#variáveis-de-ambiente)
- [Configuração do Backend](#configuração-do-backend)
- [Opções de Deploy](#opções-de-deploy)
  - [Vercel](#vercel)
  - [Netlify](#netlify)
  - [AWS S3 + CloudFront](#aws-s3--cloudfront)
  - [Servidor Nginx](#servidor-nginx)
  - [Docker](#docker)
- [Configuração de CORS](#configuração-de-cors)
- [Performance e Otimização](#performance-e-otimização)
- [Monitoramento](#monitoramento)
- [Troubleshooting](#troubleshooting)

---

## Pré-requisitos

Antes de fazer o deploy, certifique-se de que:

- ✅ Backend da API está rodando e acessível
- ✅ Variáveis de ambiente estão configuradas corretamente
- ✅ CORS está configurado no backend para aceitar origem do frontend
- ✅ Certificado SSL/HTTPS está configurado (recomendado)
- ✅ Build local foi testado com sucesso

---

## Build de Produção

### 1. Preparação

```bash
# Limpe builds anteriores
rm -rf dist/

# Instale dependências (se necessário)
npm ci
```

### 2. Configuração de Ambiente

Crie arquivo `.env.production`:

```env
# API Configuration
VITE_API_BASE_URL=https://api.yourdomain.com/api

# Application
VITE_APP_NAME=Sistema de Rede de Credenciamento
VITE_APP_VERSION=1.0.0

# Environment
VITE_ENV=production
```

**⚠️ IMPORTANTE:** 
- Use HTTPS na URL da API em produção
- Nunca commite `.env.production` no Git
- Todas as variáveis devem começar com `VITE_`

### 3. Build

```bash
# Build de produção
npm run build

# Resultado: pasta dist/ com arquivos otimizados
```

### 4. Preview Local (Opcional)

```bash
# Testar build localmente antes do deploy
npm run preview

# Acesse: http://localhost:4173
```

**Verificações:**
- ✅ Páginas carregam corretamente
- ✅ Autenticação funciona
- ✅ Requisições para API funcionam
- ✅ Rotas protegidas redirecionam para login
- ✅ Sem erros no console do navegador

---

## Variáveis de Ambiente

### Desenvolvimento

**Arquivo:** `.env.local`

```env
VITE_API_BASE_URL=http://localhost:5000/api
VITE_APP_NAME=Sistema de Rede de Credenciamento
VITE_APP_VERSION=1.0.0
VITE_ENV=development
```

### Staging

**Arquivo:** `.env.staging`

```env
VITE_API_BASE_URL=https://api-staging.yourdomain.com/api
VITE_APP_NAME=Sistema de Rede de Credenciamento [STAGING]
VITE_APP_VERSION=1.0.0-beta
VITE_ENV=staging
```

### Produção

**Arquivo:** `.env.production`

```env
VITE_API_BASE_URL=https://api.yourdomain.com/api
VITE_APP_NAME=Sistema de Rede de Credenciamento
VITE_APP_VERSION=1.0.0
VITE_ENV=production
```

### Scripts de Build por Ambiente

Adicione ao `package.json`:

```json
{
  "scripts": {
    "build:staging": "vite build --mode staging",
    "build:production": "vite build --mode production"
  }
}
```

---

## Configuração do Backend

### CORS - Program.cs (Backend .NET)

```csharp
// Configure CORS para aceitar o domínio do frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "https://yourdomain.com",           // Produção
                "https://staging.yourdomain.com",   // Staging
                "http://localhost:5173"             // Desenvolvimento
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ...

app.UseCors("AllowFrontend");
```

### Headers de Segurança

Configure os seguintes headers no backend:

```
Access-Control-Allow-Origin: https://yourdomain.com
Access-Control-Allow-Methods: GET, POST, PUT, DELETE, PATCH
Access-Control-Allow-Headers: Content-Type, Authorization
Access-Control-Allow-Credentials: true
```

---

## Opções de Deploy

### Vercel

Ideal para: Deploy rápido e fácil com CI/CD integrado

#### Passos:

1. **Instale Vercel CLI:**
```bash
npm install -g vercel
```

2. **Login:**
```bash
vercel login
```

3. **Deploy:**
```bash
# Deploy de preview
vercel

# Deploy de produção
vercel --prod
```

4. **Configuração (vercel.json):**
```json
{
  "buildCommand": "npm run build",
  "outputDirectory": "dist",
  "devCommand": "npm run dev",
  "framework": "vite",
  "rewrites": [
    {
      "source": "/(.*)",
      "destination": "/index.html"
    }
  ],
  "env": {
    "VITE_API_BASE_URL": "https://api.yourdomain.com/api"
  }
}
```

5. **Variáveis de Ambiente:**
   - Acesse Dashboard da Vercel
   - Settings → Environment Variables
   - Adicione `VITE_API_BASE_URL`, etc.

---

### Netlify

Ideal para: Deploy com formulários e funções serverless

#### Passos:

1. **Instale Netlify CLI:**
```bash
npm install -g netlify-cli
```

2. **Login:**
```bash
netlify login
```

3. **Deploy:**
```bash
# Deploy de teste
netlify deploy

# Deploy de produção
netlify deploy --prod
```

4. **Configuração (netlify.toml):**
```toml
[build]
  command = "npm run build"
  publish = "dist"

[[redirects]]
  from = "/*"
  to = "/index.html"
  status = 200

[build.environment]
  VITE_API_BASE_URL = "https://api.yourdomain.com/api"
```

5. **Variáveis de Ambiente:**
   - Acesse Netlify Dashboard
   - Site settings → Build & deploy → Environment
   - Adicione variáveis

---

### AWS S3 + CloudFront

Ideal para: Alta escalabilidade e controle total

#### Passos:

1. **Crie Bucket S3:**
```bash
aws s3 mb s3://your-frontend-bucket --region us-east-1
```

2. **Configure como Website:**
```bash
aws s3 website s3://your-frontend-bucket \
  --index-document index.html \
  --error-document index.html
```

3. **Build e Upload:**
```bash
npm run build
aws s3 sync dist/ s3://your-frontend-bucket --delete
```

4. **Configure CloudFront:**
   - Origin: S3 bucket
   - Viewer Protocol: Redirect HTTP to HTTPS
   - Error Pages: 404 → /index.html (200)
   - Invalidation: `/*` após cada deploy

5. **Script de Deploy (deploy.sh):**
```bash
#!/bin/bash
set -e

echo "🏗️  Building..."
npm run build

echo "☁️  Uploading to S3..."
aws s3 sync dist/ s3://your-frontend-bucket --delete

echo "🔄 Invalidating CloudFront..."
aws cloudfront create-invalidation \
  --distribution-id YOUR_DISTRIBUTION_ID \
  --paths "/*"

echo "✅ Deploy completo!"
```

---

### Servidor Nginx

Ideal para: Servidor próprio com controle total

#### Passos:

1. **Build Local:**
```bash
npm run build
```

2. **Transfira arquivos para servidor:**
```bash
scp -r dist/* user@server:/var/www/frontend/
```

3. **Configuração Nginx (/etc/nginx/sites-available/frontend):**
```nginx
server {
    listen 80;
    server_name yourdomain.com www.yourdomain.com;

    # Redirect HTTP to HTTPS
    return 301 https://$host$request_uri;
}

server {
    listen 443 ssl http2;
    server_name yourdomain.com www.yourdomain.com;

    # SSL Configuration
    ssl_certificate /etc/letsencrypt/live/yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/yourdomain.com/privkey.pem;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;

    # Root directory
    root /var/www/frontend;
    index index.html;

    # Gzip compression
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
    gzip_min_length 1000;

    # Cache static assets
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }

    # Handle React Router (SPA)
    location / {
        try_files $uri $uri/ /index.html;
    }

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Referrer-Policy "no-referrer-when-downgrade" always;
}
```

4. **Ativar site e reload:**
```bash
sudo ln -s /etc/nginx/sites-available/frontend /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

5. **Certificado SSL com Let's Encrypt:**
```bash
sudo apt install certbot python3-certbot-nginx
sudo certbot --nginx -d yourdomain.com -d www.yourdomain.com
```

---

### Docker

Ideal para: Deploy consistente em qualquer ambiente

#### Dockerfile:

```dockerfile
# Stage 1: Build
FROM node:18-alpine AS builder

WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

# Stage 2: Production
FROM nginx:alpine

# Copy build
COPY --from=builder /app/dist /usr/share/nginx/html

# Copy nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose port
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
```

#### nginx.conf (para Docker):

```nginx
server {
    listen 80;
    server_name _;

    root /usr/share/nginx/html;
    index index.html;

    # Gzip
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;

    # Cache
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }

    # SPA fallback
    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

#### docker-compose.yml:

```yaml
version: '3.8'

services:
  frontend:
    build: .
    ports:
      - "80:80"
    environment:
      - VITE_API_BASE_URL=https://api.yourdomain.com/api
    restart: unless-stopped
```

#### Build e Run:

```bash
# Build
docker build -t partnermesh-frontend .

# Run
docker run -d -p 80:80 \
  -e VITE_API_BASE_URL=https://api.yourdomain.com/api \
  --name frontend \
  partnermesh-frontend

# Com Docker Compose
docker-compose up -d
```

---

## Configuração de CORS

### Frontend (axios.config.ts)

```typescript
const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true, // Para enviar cookies se necessário
});
```

### Backend (ASP.NET Core)

```csharp
// Configuração detalhada de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy
            .WithOrigins("https://yourdomain.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

app.UseCors("Production");
```

### Verificação

```bash
# Teste CORS com curl
curl -H "Origin: https://yourdomain.com" \
     -H "Access-Control-Request-Method: POST" \
     -H "Access-Control-Request-Headers: Content-Type, Authorization" \
     -X OPTIONS \
     https://api.yourdomain.com/api/auth/login
```

Resposta esperada:
```
Access-Control-Allow-Origin: https://yourdomain.com
Access-Control-Allow-Methods: GET, POST, PUT, DELETE, PATCH
Access-Control-Allow-Headers: Content-Type, Authorization
```

---

## Performance e Otimização

### 1. Code Splitting (Lazy Loading)

Implemente lazy loading nas rotas:

```typescript
// router.tsx
import { lazy, Suspense } from 'react';

const DashboardPage = lazy(() => import('@/pages/DashboardPage'));
const UsersListPage = lazy(() => import('@/pages/Users/UsersListPage'));

// Wrap com Suspense
<Suspense fallback={<Loading fullScreen />}>
  <DashboardPage />
</Suspense>
```

### 2. Compressão

Nginx/Apache já fazem gzip automaticamente, mas verifique:

```bash
# Teste compressão
curl -H "Accept-Encoding: gzip" -I https://yourdomain.com

# Resposta esperada:
# Content-Encoding: gzip
```

### 3. Cache de Assets

Configure cache agressivo para assets estáticos:

```nginx
# Cache por 1 ano
location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2)$ {
    expires 1y;
    add_header Cache-Control "public, immutable";
}
```

### 4. CDN (Opcional)

Use CDN como Cloudflare para:
- Cache global
- Proteção DDoS
- SSL grátis
- Melhor performance global

### 5. Análise de Bundle

```bash
# Instale plugin de análise
npm install -D rollup-plugin-visualizer

# Build com análise
npm run build

# Abra stats.html gerado
```

**Metas de Performance:**
- ✅ Bundle principal < 500KB (gzipped)
- ✅ First Contentful Paint < 2s
- ✅ Time to Interactive < 3s
- ✅ Lighthouse Score > 90

---

## Monitoramento

### 1. Logs de Erros

Configure Sentry ou similar:

```bash
npm install @sentry/react
```

```typescript
// main.tsx
import * as Sentry from "@sentry/react";

Sentry.init({
  dsn: "YOUR_SENTRY_DSN",
  environment: import.meta.env.VITE_ENV,
  tracesSampleRate: 1.0,
});
```

### 2. Analytics

Configure Google Analytics:

```typescript
// utils/analytics.ts
export const trackPageView = (url: string) => {
  if (window.gtag) {
    window.gtag('config', 'GA_MEASUREMENT_ID', {
      page_path: url,
    });
  }
};
```

### 3. Health Check

Endpoint de status:

```typescript
// pages/HealthCheck.tsx
export function HealthCheckPage() {
  const { data } = useQuery({
    queryKey: ['health'],
    queryFn: () => axios.get('/health'),
  });
  
  return <div>Status: {data?.status}</div>;
}
```

### 4. Alertas

Configure alertas para:
- ❌ Build falhando
- ❌ Deploy com erro
- ❌ Taxa de erro > 5%
- ❌ Tempo de resposta > 5s

---

## Troubleshooting

### Problema: Página em branco após deploy

**Causas:**
- Base path incorreto
- Variáveis de ambiente faltando
- CORS bloqueando requisições

**Solução:**
```bash
# Verifique console do navegador (F12)
# Teste URL da API
curl https://api.yourdomain.com/api/health

# Verifique variáveis
console.log(import.meta.env.VITE_API_BASE_URL)
```

### Problema: 404 em rotas ao recarregar

**Causa:** Servidor não configurado para SPA

**Solução:**
```nginx
# Nginx: adicione fallback
location / {
    try_files $uri $uri/ /index.html;
}
```

### Problema: CORS errors

**Solução:**
```csharp
// Backend: adicione origem exata do frontend
.WithOrigins("https://yourdomain.com")
```

### Problema: Assets não carregam

**Causa:** Base path errado

**Solução:**
```typescript
// vite.config.ts
export default defineConfig({
  base: '/', // ou '/subpath/' se não for root
});
```

### Problema: Build muito grande

**Solução:**
```bash
# Analise bundle
npm run build

# Implemente code splitting
# Lazy load rotas menos usadas
```

---

## Checklist de Deploy

Antes de fazer deploy em produção:

### Pré-Deploy
- [ ] Todos os testes passando
- [ ] Build local funciona (`npm run build && npm run preview`)
- [ ] Variáveis de ambiente configuradas
- [ ] CORS configurado no backend
- [ ] SSL/HTTPS configurado
- [ ] Domínio apontando corretamente

### Deploy
- [ ] Build de produção executado
- [ ] Arquivos enviados para servidor/CDN
- [ ] Cache invalidado (se aplicável)
- [ ] DNS propagado

### Pós-Deploy
- [ ] Site acessível via HTTPS
- [ ] Login funciona
- [ ] Requisições para API funcionam
- [ ] Sem erros no console
- [ ] Rotas protegidas funcionam
- [ ] Performance adequada (Lighthouse)
- [ ] Monitoramento ativo

---

## Scripts Úteis

### deploy.sh (Nginx)

```bash
#!/bin/bash
set -e

echo "🏗️  Building..."
npm run build

echo "📦 Compressing..."
tar -czf dist.tar.gz dist/

echo "📤 Uploading..."
scp dist.tar.gz user@server:/tmp/

echo "🚀 Deploying..."
ssh user@server << 'EOF'
  cd /var/www/frontend
  rm -rf *
  tar -xzf /tmp/dist.tar.gz --strip-components=1
  rm /tmp/dist.tar.gz
EOF

echo "✅ Deploy completo!"
```

### rollback.sh

```bash
#!/bin/bash
set -e

echo "⏮️  Rolling back..."
ssh user@server << 'EOF'
  cd /var/www/frontend
  rm -rf *
  tar -xzf /var/backups/frontend-backup.tar.gz
EOF

echo "✅ Rollback completo!"
```

---

## Suporte

Para problemas de deploy:
1. Verifique logs do servidor
2. Teste build localmente
3. Valide variáveis de ambiente
4. Consulte documentação do provedor (Vercel, Netlify, etc.)

**Última atualização:** Dezembro 2024
