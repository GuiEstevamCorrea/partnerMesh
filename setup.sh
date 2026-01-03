#!/bin/bash

# PartnerMesh - Script de Inicialização
# Este script configura e inicia o projeto completo

set -e

echo "🚀 PartnerMesh - Setup Completo"
echo "================================"

# Verificar pré-requisitos
check_prerequisites() {
    echo "📋 Verificando pré-requisitos..."
    
    if ! command -v docker &> /dev/null; then
        echo "❌ Docker não encontrado. Instale o Docker Desktop."
        exit 1
    fi
    
    if ! command -v docker-compose &> /dev/null; then
        echo "❌ Docker Compose não encontrado. Instale o Docker Compose."
        exit 1
    fi
    
    echo "✅ Docker e Docker Compose encontrados"
}

# Configurar ambiente
setup_environment() {
    echo "🔧 Configurando ambiente..."
    
    if [ ! -f .env ]; then
        if [ -f .env.example ]; then
            cp .env.example .env
            echo "✅ Arquivo .env criado a partir do .env.example"
        else
            echo "⚠️  Arquivo .env.example não encontrado"
        fi
    fi
    
    # Criar diretórios necessários
    mkdir -p data/mssql
    mkdir -p data/prod/mssql
    mkdir -p backups
    echo "✅ Diretórios criados"
}

# Iniciar serviços
start_services() {
    echo "🐳 Iniciando serviços Docker..."
    
    # Parar containers existentes
    docker-compose down 2>/dev/null || true
    
    # Iniciar em background
    docker-compose up -d
    
    echo "⏳ Aguardando serviços ficarem saudáveis..."
    
    # Aguardar SQL Server
    echo "📊 Aguardando SQL Server..."
    for i in {1..30}; do
        if docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P PartnerMesh@2026 -Q "SELECT 1" &>/dev/null; then
            echo "✅ SQL Server pronto"
            break
        fi
        echo "⏳ Tentativa $i/30..."
        sleep 10
    done
    
    # Aguardar Backend
    echo "🔧 Aguardando Backend..."
    for i in {1..20}; do
        if curl -s http://localhost:5000/health &>/dev/null; then
            echo "✅ Backend pronto"
            break
        fi
        echo "⏳ Tentativa $i/20..."
        sleep 15
    done
}

# Configurar banco de dados
setup_database() {
    echo "🗄️  Configurando banco de dados..."
    
    # Executar migrações
    if docker-compose exec -T backend dotnet ef database update; then
        echo "✅ Migrações aplicadas com sucesso"
    else
        echo "❌ Erro ao aplicar migrações. Tentando localmente..."
        cd Api
        if dotnet ef database update; then
            echo "✅ Migrações aplicadas localmente"
        else
            echo "❌ Erro ao aplicar migrações"
            exit 1
        fi
        cd ..
    fi
}

# Verificar status
check_status() {
    echo "📊 Verificando status dos serviços..."
    docker-compose ps
    
    echo ""
    echo "🔍 Testando endpoints..."
    
    # Testar SQL Server
    if docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P PartnerMesh@2026 -Q "SELECT 1" &>/dev/null; then
        echo "✅ SQL Server: OK"
    else
        echo "❌ SQL Server: ERRO"
    fi
    
    # Testar Backend
    if curl -s http://localhost:5000/health &>/dev/null; then
        echo "✅ Backend: OK"
    else
        echo "❌ Backend: ERRO"
    fi
    
    # Testar Frontend
    if curl -s http://localhost:3000 &>/dev/null; then
        echo "✅ Frontend: OK"
    else
        echo "⏳ Frontend: Ainda carregando..."
    fi
}

# Mostrar informações de acesso
show_access_info() {
    echo ""
    echo "🎉 Setup concluído com sucesso!"
    echo "================================"
    echo ""
    echo "📱 Acesse a aplicação:"
    echo "   Frontend:  http://localhost:3000"
    echo "   Backend:   http://localhost:5000/api"
    echo "   Swagger:   http://localhost:5000/swagger"
    echo ""
    echo "🗄️  SQL Server:"
    echo "   Host:      localhost:1433"
    echo "   Usuário:   sa"
    echo "   Senha:     PartnerMesh@2026"
    echo ""
    echo "👤 Credenciais de login:"
    echo "   Email:     admin@partnermesh.com"
    echo "   Senha:     Admin@123"
    echo ""
    echo "🛠️  Comandos úteis:"
    echo "   Ver logs:           docker-compose logs -f"
    echo "   Parar serviços:     docker-compose down"
    echo "   Reiniciar:          docker-compose restart"
    echo ""
}

# Função principal
main() {
    check_prerequisites
    setup_environment
    start_services
    setup_database
    check_status
    show_access_info
}

# Executar apenas se chamado diretamente
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi