# PartnerMesh - Script de Inicialização (Windows)
# Este script configura e inicia o projeto completo

Write-Host "🚀 PartnerMesh - Setup Completo" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green

function Test-Prerequisites {
    Write-Host "📋 Verificando pré-requisitos..." -ForegroundColor Yellow
    
    if (!(Get-Command docker -ErrorAction SilentlyContinue)) {
        Write-Host "❌ Docker não encontrado. Instale o Docker Desktop." -ForegroundColor Red
        exit 1
    }
    
    if (!(Get-Command docker-compose -ErrorAction SilentlyContinue)) {
        Write-Host "❌ Docker Compose não encontrado. Instale o Docker Compose." -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✅ Docker e Docker Compose encontrados" -ForegroundColor Green
}

function Initialize-Environment {
    Write-Host "🔧 Configurando ambiente..." -ForegroundColor Yellow
    
    if (!(Test-Path .env)) {
        if (Test-Path .env.example) {
            Copy-Item .env.example .env
            Write-Host "✅ Arquivo .env criado a partir do .env.example" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Arquivo .env.example não encontrado" -ForegroundColor Yellow
        }
    }
    
    # Criar diretórios necessários
    $directories = @("data\mssql", "data\prod\mssql", "backups")
    foreach ($dir in $directories) {
        if (!(Test-Path $dir)) {
            New-Item -ItemType Directory -Path $dir -Force | Out-Null
        }
    }
    Write-Host "✅ Diretórios criados" -ForegroundColor Green
}

function Start-Services {
    Write-Host "🐳 Iniciando serviços Docker..." -ForegroundColor Yellow
    
    # Parar containers existentes
    docker-compose down 2>$null
    
    # Iniciar em background
    docker-compose up -d
    
    Write-Host "⏳ Aguardando serviços ficarem saudáveis..." -ForegroundColor Yellow
    
    # Aguardar SQL Server
    Write-Host "📊 Aguardando SQL Server..." -ForegroundColor Cyan
    for ($i = 1; $i -le 30; $i++) {
        try {
            $result = docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P PartnerMesh@2026 -Q "SELECT 1" 2>$null
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✅ SQL Server pronto" -ForegroundColor Green
                break
            }
        } catch {}
        Write-Host "⏳ Tentativa $i/30..." -ForegroundColor Yellow
        Start-Sleep -Seconds 10
    }
    
    # Aguardar Backend
    Write-Host "🔧 Aguardando Backend..." -ForegroundColor Cyan
    for ($i = 1; $i -le 20; $i++) {
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -TimeoutSec 5 2>$null
            if ($response.StatusCode -eq 200) {
                Write-Host "✅ Backend pronto" -ForegroundColor Green
                break
            }
        } catch {}
        Write-Host "⏳ Tentativa $i/20..." -ForegroundColor Yellow
        Start-Sleep -Seconds 15
    }
}

function Initialize-Database {
    Write-Host "🗄️  Configurando banco de dados..." -ForegroundColor Yellow
    
    # Executar migrações
    try {
        docker-compose exec -T backend dotnet ef database update
        Write-Host "✅ Migrações aplicadas com sucesso" -ForegroundColor Green
    } catch {
        Write-Host "❌ Erro ao aplicar migrações. Tentando localmente..." -ForegroundColor Yellow
        Set-Location Api
        try {
            dotnet ef database update
            Write-Host "✅ Migrações aplicadas localmente" -ForegroundColor Green
        } catch {
            Write-Host "❌ Erro ao aplicar migrações" -ForegroundColor Red
            exit 1
        }
        Set-Location ..
    }
}

function Test-Services {
    Write-Host "📊 Verificando status dos serviços..." -ForegroundColor Yellow
    docker-compose ps
    
    Write-Host ""
    Write-Host "🔍 Testando endpoints..." -ForegroundColor Yellow
    
    # Testar SQL Server
    try {
        docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P PartnerMesh@2026 -Q "SELECT 1" 2>$null
        Write-Host "✅ SQL Server: OK" -ForegroundColor Green
    } catch {
        Write-Host "❌ SQL Server: ERRO" -ForegroundColor Red
    }
    
    # Testar Backend
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -TimeoutSec 5
        Write-Host "✅ Backend: OK" -ForegroundColor Green
    } catch {
        Write-Host "❌ Backend: ERRO" -ForegroundColor Red
    }
    
    # Testar Frontend
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:3000" -TimeoutSec 5
        Write-Host "✅ Frontend: OK" -ForegroundColor Green
    } catch {
        Write-Host "⏳ Frontend: Ainda carregando..." -ForegroundColor Yellow
    }
}

function Show-AccessInfo {
    Write-Host ""
    Write-Host "🎉 Setup concluído com sucesso!" -ForegroundColor Green
    Write-Host "================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "📱 Acesse a aplicação:" -ForegroundColor Cyan
    Write-Host "   Frontend:  http://localhost:3000" -ForegroundColor White
    Write-Host "   Backend:   http://localhost:5000/api" -ForegroundColor White
    Write-Host "   Swagger:   http://localhost:5000/swagger" -ForegroundColor White
    Write-Host ""
    Write-Host "🗄️  SQL Server:" -ForegroundColor Cyan
    Write-Host "   Host:      localhost:1433" -ForegroundColor White
    Write-Host "   Usuário:   sa" -ForegroundColor White
    Write-Host "   Senha:     PartnerMesh@2026" -ForegroundColor White
    Write-Host ""
    Write-Host "👤 Credenciais de login:" -ForegroundColor Cyan
    Write-Host "   Email:     admin@partnermesh.com" -ForegroundColor White
    Write-Host "   Senha:     Admin@123" -ForegroundColor White
    Write-Host ""
    Write-Host "🛠️  Comandos úteis:" -ForegroundColor Cyan
    Write-Host "   Ver logs:           docker-compose logs -f" -ForegroundColor White
    Write-Host "   Parar serviços:     docker-compose down" -ForegroundColor White
    Write-Host "   Reiniciar:          docker-compose restart" -ForegroundColor White
    Write-Host ""
}

# Função principal
function Main {
    Test-Prerequisites
    Initialize-Environment
    Start-Services
    Initialize-Database
    Test-Services
    Show-AccessInfo
}

# Executar
Main