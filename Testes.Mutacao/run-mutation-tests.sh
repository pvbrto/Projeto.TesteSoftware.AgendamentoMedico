#!/bin/bash

# Script para executar testes de mutação com Stryker.NET
# Autor: Sistema de Testes de Mutação
# Data: $(date +%Y-%m-%d)

set -e  # Parar execução em caso de erro

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para imprimir mensagens coloridas
print_message() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Função para imprimir cabeçalho
print_header() {
    echo ""
    print_message $BLUE "=================================================="
    print_message $BLUE "$1"
    print_message $BLUE "=================================================="
    echo ""
}

# Função para verificar se comando existe
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Verificar se .NET está instalado
if ! command_exists dotnet; then
    print_message $RED "❌ .NET não está instalado!"
    print_message $YELLOW "Instale o .NET SDK: https://dotnet.microsoft.com/download"
    exit 1
fi

print_header "🧬 EXECUTANDO TESTES DE MUTAÇÃO"

# Verificar se Stryker está instalado
if ! command_exists dotnet-stryker; then
    print_message $YELLOW "⚠️  Stryker.NET não encontrado. Instalando..."
    
    # Verificar se existe tool-manifest
    if [ ! -f ".config/dotnet-tools.json" ]; then
        print_message $BLUE "📦 Criando manifest de ferramentas..."
        dotnet new tool-manifest
    fi
    
    # Instalar Stryker localmente
    print_message $BLUE "📦 Instalando Stryker.NET..."
    dotnet tool install dotnet-stryker
    
    print_message $GREEN "✅ Stryker.NET instalado com sucesso!"
else
    print_message $GREEN "✅ Stryker.NET já está instalado"
fi

# Verificar se o projeto de teste existe
if [ ! -f "Testes.Mutacao.csproj" ]; then
    print_message $RED "❌ Projeto Testes.Mutacao.csproj não encontrado!"
    print_message $YELLOW "Execute este script a partir do diretório Testes.Mutacao/"
    exit 1
fi

# Limpar builds anteriores
print_message $BLUE "🧹 Limpando builds anteriores..."
dotnet clean > /dev/null 2>&1

# Restaurar dependências
print_message $BLUE "📦 Restaurando dependências..."
dotnet restore

# Compilar projeto
print_message $BLUE "🔨 Compilando projeto..."
dotnet build --no-restore

# Executar testes unitários primeiro
print_message $BLUE "🧪 Executando testes unitários..."
dotnet test --no-build --verbosity quiet

if [ $? -ne 0 ]; then
    print_message $RED "❌ Testes unitários falharam! Corrija os testes antes de executar mutação."
    exit 1
fi

print_message $GREEN "✅ Todos os testes unitários passaram!"

# Criar diretório de output se não existir
mkdir -p StrykerOutput

# Configurar parâmetros do Stryker
STRYKER_PARAMS=(
    "--project" "Testes.Mutacao.csproj"
    "--reporters" "html,json,console"
    "--output" "StrykerOutput"
    "--mutation-level" "Complete"
    "--timeout-ms" "10000"
    "--thresholds-high" "90"
    "--thresholds-low" "70"
    "--thresholds-break" "60"
    "--verbosity" "info"
)

# Verificar se existe configuração personalizada
if [ -f "stryker-config.json" ]; then
    print_message $BLUE "📋 Usando configuração personalizada: stryker-config.json"
    STRYKER_PARAMS+=("--config-file" "stryker-config.json")
fi

print_header "🚀 INICIANDO ANÁLISE DE MUTAÇÃO"

print_message $BLUE "⏱️  Isso pode levar alguns minutos..."
print_message $YELLOW "💡 Dica: Quanto mais testes, mais tempo levará"

# Executar Stryker
start_time=$(date +%s)

if dotnet stryker "${STRYKER_PARAMS[@]}"; then
    end_time=$(date +%s)
    duration=$((end_time - start_time))
    
    print_header "✅ ANÁLISE DE MUTAÇÃO CONCLUÍDA"
    
    print_message $GREEN "⏱️  Tempo de execução: ${duration}s"
    
    # Verificar se relatório HTML foi gerado
    if [ -f "StrykerOutput/reports/mutation-report.html" ]; then
        print_message $GREEN "📊 Relatório HTML gerado: StrykerOutput/reports/mutation-report.html"
        
        # Tentar abrir relatório no navegador (macOS/Linux)
        if command_exists open; then
            print_message $BLUE "🌐 Abrindo relatório no navegador..."
            open "StrykerOutput/reports/mutation-report.html"
        elif command_exists xdg-open; then
            print_message $BLUE "🌐 Abrindo relatório no navegador..."
            xdg-open "StrykerOutput/reports/mutation-report.html"
        fi
    fi
    
    # Verificar se relatório JSON foi gerado
    if [ -f "StrykerOutput/reports/mutation-report.json" ]; then
        print_message $GREEN "📋 Relatório JSON gerado: StrykerOutput/reports/mutation-report.json"
        
        # Extrair métricas básicas do JSON (se jq estiver disponível)
        if command_exists jq; then
            print_message $BLUE "📈 Extraindo métricas..."
            
            mutation_score=$(jq -r '.thresholds.high // "N/A"' "StrykerOutput/reports/mutation-report.json" 2>/dev/null || echo "N/A")
            killed_mutants=$(jq -r '.files | map(.mutants | map(select(.status == "Killed")) | length) | add // 0' "StrykerOutput/reports/mutation-report.json" 2>/dev/null || echo "N/A")
            survived_mutants=$(jq -r '.files | map(.mutants | map(select(.status == "Survived")) | length) | add // 0' "StrykerOutput/reports/mutation-report.json" 2>/dev/null || echo "N/A")
            
            echo ""
            print_message $BLUE "📊 RESUMO DOS RESULTADOS:"
            print_message $GREEN "   🎯 Mutation Score: ${mutation_score}%"
            print_message $GREEN "   ✅ Mutações Detectadas: ${killed_mutants}"
            print_message $YELLOW "   ❌ Mutações Sobreviventes: ${survived_mutants}"
        fi
    fi
    
    echo ""
    print_message $BLUE "📁 Arquivos gerados:"
    find StrykerOutput -name "*.html" -o -name "*.json" | while read file; do
        print_message $GREEN "   📄 $file"
    done
    
    echo ""
    print_message $BLUE "🎯 PRÓXIMOS PASSOS:"
    print_message $YELLOW "   1. Abra o relatório HTML para análise detalhada"
    print_message $YELLOW "   2. Identifique mutações sobreviventes"
    print_message $YELLOW "   3. Adicione testes para cobrir gaps identificados"
    print_message $YELLOW "   4. Execute novamente para melhorar o Mutation Score"
    
else
    print_message $RED "❌ Falha na execução dos testes de mutação!"
    print_message $YELLOW "Verifique os logs acima para identificar o problema"
    exit 1
fi

print_header "🎉 PROCESSO CONCLUÍDO COM SUCESSO"