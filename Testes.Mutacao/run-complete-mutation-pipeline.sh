#!/bin/bash

# Pipeline completo de testes de mutação
# Autor: Sistema de Testes de Mutação
# Data: $(date +%Y-%m-%d)

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
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

print_header "🧬 PIPELINE COMPLETO DE TESTES DE MUTAÇÃO"

# Verificar se estamos no diretório correto
if [ ! -f "Testes.Mutacao.csproj" ]; then
    print_message $RED "❌ Execute este script a partir do diretório Testes.Mutacao/"
    exit 1
fi

# Verificar dependências
print_message $BLUE "🔍 Verificando dependências..."

if ! command_exists dotnet; then
    print_message $RED "❌ .NET SDK não encontrado!"
    print_message $YELLOW "Instale: https://dotnet.microsoft.com/download"
    exit 1
fi

if ! command_exists bc; then
    print_message $YELLOW "⚠️  bc não encontrado. Instalando..."
    if [[ "$OSTYPE" == "darwin"* ]]; then
        if command_exists brew; then
            brew install bc
        else
            print_message $RED "❌ Instale bc manualmente: brew install bc"
            exit 1
        fi
    elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
        if command_exists apt-get; then
            sudo apt-get update && sudo apt-get install -y bc
        elif command_exists yum; then
            sudo yum install -y bc
        fi
    fi
fi

print_message $GREEN "✅ Dependências verificadas"

# Etapa 1: Preparação do ambiente
print_header "📦 ETAPA 1: PREPARAÇÃO DO AMBIENTE"

print_message $BLUE "🧹 Limpando builds anteriores..."
dotnet clean > /dev/null 2>&1

print_message $BLUE "📦 Restaurando dependências..."
dotnet restore

print_message $BLUE "🔨 Compilando projeto..."
dotnet build --no-restore

if [ $? -ne 0 ]; then
    print_message $RED "❌ Falha na compilação!"
    exit 1
fi

print_message $GREEN "✅ Ambiente preparado com sucesso"

# Etapa 2: Execução dos testes unitários
print_header "🧪 ETAPA 2: VALIDAÇÃO DOS TESTES UNITÁRIOS"

print_message $BLUE "🧪 Executando testes unitários..."
dotnet test --no-build --verbosity quiet --logger "console;verbosity=minimal"

if [ $? -ne 0 ]; then
    print_message $RED "❌ Testes unitários falharam!"
    print_message $YELLOW "Corrija os testes antes de executar mutação"
    exit 1
fi

print_message $GREEN "✅ Todos os testes unitários passaram"

# Etapa 3: Execução dos testes de mutação
print_header "🧬 ETAPA 3: EXECUÇÃO DOS TESTES DE MUTAÇÃO"

print_message $BLUE "🚀 Iniciando análise de mutação..."
print_message $YELLOW "⏱️  Isso pode levar vários minutos..."

start_time=$(date +%s)

# Executar script de mutação
if ./run-mutation-tests.sh; then
    end_time=$(date +%s)
    duration=$((end_time - start_time))
    
    print_message $GREEN "✅ Testes de mutação concluídos em ${duration}s"
else
    print_message $RED "❌ Falha nos testes de mutação!"
    exit 1
fi

# Etapa 4: Análise dos resultados
print_header "📊 ETAPA 4: ANÁLISE DOS RESULTADOS"

if [ -f "StrykerOutput/reports/mutation-report.json" ]; then
    print_message $BLUE "📈 Analisando resultados..."
    
    # Executar script de análise
    if ./analyze-mutation-results.sh; then
        print_message $GREEN "✅ Análise concluída"
    else
        print_message $YELLOW "⚠️  Análise parcial (algumas ferramentas podem estar ausentes)"
    fi
else
    print_message $RED "❌ Relatório de mutação não encontrado!"
    exit 1
fi

# Etapa 5: Geração de relatório consolidado
print_header "📋 ETAPA 5: RELATÓRIO CONSOLIDADO"

REPORT_FILE="StrykerOutput/pipeline-report.md"

# Extrair métricas básicas
if command_exists jq; then
    JSON_REPORT="StrykerOutput/reports/mutation-report.json"
    
    total_mutants=$(jq -r '[.files[].mutants | length] | add // 0' "$JSON_REPORT")
    killed_mutants=$(jq -r '[.files[].mutants[] | select(.status == "Killed")] | length' "$JSON_REPORT")
    survived_mutants=$(jq -r '[.files[].mutants[] | select(.status == "Survived")] | length' "$JSON_REPORT")
    
    if [ "$total_mutants" -gt 0 ]; then
        mutation_score=$(echo "scale=1; $killed_mutants * 100 / $total_mutants" | bc -l)
    else
        mutation_score="0"
    fi
    
    # Gerar relatório em Markdown
    {
        echo "# Relatório do Pipeline de Testes de Mutação"
        echo ""
        echo "**Data:** $(date '+%d/%m/%Y %H:%M:%S')"
        echo "**Duração:** ${duration}s"
        echo ""
        echo "## 📊 Resumo Executivo"
        echo ""
        echo "| Métrica | Valor |"
        echo "|---------|-------|"
        echo "| **Total de Mutações** | $total_mutants |"
        echo "| **Mutações Detectadas** | $killed_mutants |"
        echo "| **Mutações Sobreviventes** | $survived_mutants |"
        echo "| **Mutation Score** | $mutation_score% |"
        echo ""
        
        # Determinar status
        if (( $(echo "$mutation_score >= 90" | bc -l) )); then
            echo "## 🏆 Status: EXCELENTE"
            echo ""
            echo "✅ **Parabéns!** Seus testes têm qualidade excepcional."
        elif (( $(echo "$mutation_score >= 80" | bc -l) )); then
            echo "## 👍 Status: BOM"
            echo ""
            echo "✅ **Boa qualidade** de testes. Pequenos ajustes podem melhorar ainda mais."
        elif (( $(echo "$mutation_score >= 70" | bc -l) )); then
            echo "## ⚠️ Status: MODERADO"
            echo ""
            echo "⚠️ **Qualidade moderada**. Melhorias são recomendadas."
        else
            echo "## ❌ Status: BAIXO"
            echo ""
            echo "❌ **Qualidade baixa**. Revisão significativa dos testes é necessária."
        fi
        
        echo ""
        echo "## 🎯 Próximas Ações"
        echo ""
        
        if [ "$survived_mutants" -gt 0 ]; then
            echo "1. **Analisar $survived_mutants mutações sobreviventes**"
            echo "   - Abra o relatório HTML para detalhes"
            echo "   - Adicione testes específicos para detectá-las"
            echo ""
        fi
        
        echo "2. **Revisar áreas críticas**"
        echo "   - Foque em validações de segurança"
        echo "   - Priorize cálculos de negócio"
        echo ""
        echo "3. **Melhorar cobertura**"
        echo "   - Teste valores limítrofes"
        echo "   - Adicione casos extremos"
        echo ""
        echo "## 📁 Arquivos Gerados"
        echo ""
        echo "- 📊 **Relatório HTML:** \`StrykerOutput/reports/mutation-report.html\`"
        echo "- 📋 **Dados JSON:** \`StrykerOutput/reports/mutation-report.json\`"
        echo "- 📄 **Resumo:** \`StrykerOutput/mutation-summary.txt\`"
        echo "- 📋 **Este relatório:** \`StrykerOutput/pipeline-report.md\`"
        echo ""
        echo "## 🔄 Executar Novamente"
        echo ""
        echo "\`\`\`bash"
        echo "./run-complete-mutation-pipeline.sh"
        echo "\`\`\`"
        
    } > "$REPORT_FILE"
    
    print_message $GREEN "📋 Relatório consolidado gerado: $REPORT_FILE"
    
    # Mostrar resumo no terminal
    echo ""
    print_message $CYAN "📊 RESUMO FINAL:"
    print_message $CYAN "   🎯 Mutation Score: $mutation_score%"
    print_message $CYAN "   ✅ Detectadas: $killed_mutants"
    print_message $CYAN "   ❌ Sobreviventes: $survived_mutants"
    
else
    print_message $YELLOW "⚠️  jq não disponível. Relatório básico gerado."
    
    {
        echo "# Relatório do Pipeline de Testes de Mutação"
        echo ""
        echo "**Data:** $(date '+%d/%m/%Y %H:%M:%S')"
        echo "**Duração:** ${duration}s"
        echo ""
        echo "Pipeline executado com sucesso!"
        echo ""
        echo "Verifique os arquivos em StrykerOutput/ para detalhes."
    } > "$REPORT_FILE"
fi

# Etapa 6: Ações finais
print_header "🎉 PIPELINE CONCLUÍDO COM SUCESSO"

print_message $GREEN "✅ Todas as etapas foram executadas"
print_message $BLUE "📊 Abra o relatório HTML para análise detalhada:"
print_message $YELLOW "   StrykerOutput/reports/mutation-report.html"

echo ""
print_message $BLUE "📋 Relatórios disponíveis:"
find StrykerOutput -name "*.html" -o -name "*.json" -o -name "*.md" -o -name "*.txt" | while read file; do
    print_message $GREEN "   📄 $file"
done

echo ""
print_message $CYAN "🔄 Para executar novamente:"
print_message $CYAN "   ./run-complete-mutation-pipeline.sh"

echo ""
print_message $PURPLE "💡 Dicas para melhorar:"
print_message $PURPLE "   1. Analise mutações sobreviventes no relatório HTML"
print_message $PURPLE "   2. Adicione testes para valores limítrofes"
print_message $PURPLE "   3. Teste condições negativas e casos extremos"
print_message $PURPLE "   4. Execute regularmente no pipeline de CI/CD"

print_header "🚀 PRONTO PARA PRÓXIMA ITERAÇÃO"