# Docker Setup - PartnerMesh

Este documento descreve como configurar e executar o banco de dados SQL Server usando Docker.

## Pré-requisitos

- Docker Desktop instalado e em execução
- Docker Compose (geralmente incluído com Docker Desktop)

## Configuração

### 1. Variáveis de Ambiente (Opcional)

Se desejar customizar as configurações, copie o arquivo `.env.example` para `.env`:

```bash
cp .env.example .env
```

E edite as variáveis conforme necessário.

### 2. Iniciar o Banco de Dados

Para subir o SQL Server:

```bash
docker-compose up -d
```

O comando acima irá:
- Baixar a imagem do SQL Server 2022 (se ainda não estiver no cache)
- Criar e iniciar o container
- Expor a porta 1433 para conexões locais
- Criar um volume persistente para os dados

### 3. Verificar o Status

Para verificar se o container está rodando:

```bash
docker-compose ps
```

Para ver os logs:

```bash
docker-compose logs -f sqlserver
```

### 4. Configurar a Connection String

Atualize o arquivo `Api/appsettings.Development.json` com a connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=PartnerMeshDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  }
}
```

**Importante:** Use a mesma senha definida em `SA_PASSWORD` no docker-compose.yml

### 5. Executar as Migrations

Com o banco rodando, execute as migrations do Entity Framework:

```bash
cd Api
dotnet ef database update
```

Ou a partir da raiz do projeto:

```bash
dotnet ef database update --project Api --startup-project Api
```

## Comandos Úteis

### Parar o Banco de Dados

```bash
docker-compose stop
```

### Parar e Remover os Containers

```bash
docker-compose down
```

### Parar e Remover Containers + Volumes (apaga os dados)

```bash
docker-compose down -v
```

### Reiniciar o Banco de Dados

```bash
docker-compose restart
```

## Conectar ao SQL Server

### Via Azure Data Studio ou SQL Server Management Studio

- **Server:** localhost,1433
- **Authentication:** SQL Login
- **Username:** sa
- **Password:** YourStrong@Passw0rd (ou a senha definida no docker-compose.yml)

### Via Docker Exec (linha de comando)

```bash
docker exec -it partnermesh-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd
```

## Troubleshooting

### Container não inicia

Verifique os logs:
```bash
docker-compose logs sqlserver
```

### Erro de senha

A senha do SA deve atender aos requisitos de complexidade do SQL Server:
- Pelo menos 8 caracteres
- Incluir maiúsculas, minúsculas, números e caracteres especiais

### Porta 1433 já em uso

Se você já tem uma instância do SQL Server rodando localmente, altere a porta no docker-compose.yml:
```yaml
ports:
  - "1434:1433"  # Use 1434 ou outra porta disponível
```

E atualize a connection string para usar a nova porta.

## Notas de Segurança

⚠️ **IMPORTANTE:** A senha padrão (`YourStrong@Passw0rd`) é apenas para desenvolvimento local. 

Para ambientes de produção:
1. Use senhas fortes e únicas
2. Nunca commite arquivos `.env` com credenciais reais
3. Use secrets management (Azure Key Vault, AWS Secrets Manager, etc.)
4. Considere usar autenticação integrada ou managed identities quando possível
