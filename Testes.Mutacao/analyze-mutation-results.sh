#!/bin/bash

# Script para análise detalhada dos resultados de testes de mutação
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

# Verificar se jq está disponível para análise JSON
if ! command_exists jq; then
    print_message $YELLOW "⚠️  jq não está instalado. Instalando..."
    
    # Tentar instalar jq baseado no sistema
    if [[ "$OSTYPE" == "darwin"* ]]; then
        if command_exists brew; then
            brew install jq
        else
            print_message $RED "❌ Homebrew não encontrado. Instale jq manualmente: https://stedolan.github.io/jq/"
            exit 1
        fi
    elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
        if command_exists apt-get; then
            sudo apt-get update && sudo apt-get install -y jq
        elif command_exists yum; then
            sudo yum install -y jq
        else
            print_message $RED "❌ Gerenciador de pacotes não suportado. Instale jq manualmente."
            exit 1
        fi
    else
        print_message $RED "❌ Sistema operacional não suportado para instalação automática do jq."
        exit 1
    fi
fi

print_header "📊 ANÁLISE DE RESULTADOS DE MUTAÇÃO"

# Verificar se existe relatório JSON
JSON_REPORT="StrykerOutput/reports/mutation-report.json"
if [ ! -f "$JSON_REPORT" ]; then
    print_message $RED "❌ Relatório JSON não encontrado: $JSON_REPORT"
    print_message $YELLOW "Execute primeiro: ./run-mutation-tests.sh"
    exit 1
fi

print_message $GREEN "✅ Relatório encontrado: $JSON_REPORT"

# Extrair dados básicos
print_header "📈 MÉTRICAS GERAIS"

mutation_score=$(jq -r '.thresholds.high // "N/A"' "$JSON_REPORT" 2>/dev/null || echo "N/A")
total_mutants=$(jq -r '[.files[].mutants | length] | add // 0' "$JSON_REPORT")
killed_mutants=$(jq -r '[.files[].mutants[] | select(.status == "Killed")] | length' "$JSON_REPORT")
survived_mutants=$(jq -r '[.files[].mutants[] | select(.status == "Survived")] | length' "$JSON_REPORT")
timeout_mutants=$(jq -r '[.files[].mutants[] | select(.status == "Timeout")] | length' "$JSON_REPORT")
no_coverage_mutants=$(jq -r '[.files[].mutants[] | select(.status == "NoCoverage")] | length' "$JSON_REPORT")
compile_error_mutants=$(jq -r '[.files[].mutants[] | select(.status == "CompileError")] | length' "$JSON_REPORT")

# Calcular percentuais
if [ "$total_mutants" -gt 0 ]; then
    killed_percent=$(echo "scale=1; $killed_mutants * 100 / $total_mutants" | bc -l 2>/dev/null || echo "0")
    survived_percent=$(echo "scale=1; $survived_mutants * 100 / $total_mutants" | bc -l 2>/dev/null || echo "0")
    timeout_percent=$(echo "scale=1; $timeout_mutants * 100 / $total_mutants" | bc -l 2>/dev/null || echo "0")
    no_coverage_percent=$(echo "scale=1; $no_coverage_mutants * 100 / $total_mutants" | bc -l 2>/dev/null || echo "0")
else
    killed_percent="0"
    survived_percent="0"
    timeout_percent="0"
    no_coverage_percent="0"
fi

echo ""
print_message $CYAN "🎯 Total de Mutações: $total_mutants"
print_message $GREEN "✅ Detectadas (Killed): $killed_mutants ($killed_percent%)"
print_message $RED "❌ Sobreviventes (Survived): $survived_mutants ($survived_percent%)"
print_message $YELLOW "⏱️  Timeout: $timeout_mutants ($timeout_percent%)"
print_message $PURPLE "📭 Sem Cobertura: $no_coverage_mutants ($no_coverage_percent%)"
print_message $BLUE "🔧 Erro de Compilação: $compile_error_mutants"

# Determinar qualidade baseada no mutation score
if [ "$killed_percent" != "N/A" ]; then
    if (( $(echo "$killed_percent >= 90" | bc -l) )); then
        quality="🏆 EXCELENTE"
        quality_color=$GREEN
    elif (( $(echo "$killed_percent >= 80" | bc -l) )); then
        quality="👍 BOA"
        quality_color=$GREEN
    elif (( $(echo "$killed_percent >= 70" | bc -l) )); then
        quality="⚠️  MODERADA"
        quality_color=$YELLOW
    else
        quality="❌ BAIXA"
        quality_color=$RED
    fi
    
    echo ""
    print_message $quality_color "📊 Qualidade dos Testes: $quality ($killed_percent%)"
fi

# Análise por arquivo
print_header "📁 ANÁLISE POR ARQUIVO"

jq -r '.files[] | "\(.source)|\(.mutants | length)|\([.mutants[] | select(.status == "Killed")] | length)|\([.mutants[] | select(.status == "Survived")] | length)"' "$JSON_REPORT" | \
while IFS='|' read -r file total killed survived; do
    if [ "$total" -gt 0 ]; then
        killed_file_percent=$(echo "scale=1; $killed * 100 / $total" | bc -l 2>/dev/null || echo "0")
        
        # Determinar cor baseada na qualidade do arquivo
        if (( $(echo "$killed_file_percent >= 90" | bc -l) )); then
            file_color=$GREEN
        elif (( $(echo "$killed_file_percent >= 70" | bc -l) )); then
            file_color=$YELLOW
        else
            file_color=$RED
        fi
        
        print_message $file_color "📄 $(basename "$file"): $killed/$total detectadas ($killed_file_percent%)"
    fi
done

# Análise de mutações sobreviventes
if [ "$survived_mutants" -gt 0 ]; then
    print_header "🔍 MUTAÇÕES SOBREVIVENTES (REQUEREM ATENÇÃO)"
    
    echo ""
    print_message $YELLOW "As seguintes mutações não foram detectadas pelos testes:"
    echo ""
    
    jq -r '.files[] | select(.mutants[] | .status == "Survived") | .source as $file | .mutants[] | select(.status == "Survived") | "Arquivo: \($file)\nLinha: \(.location.start.line)\nMutação: \(.mutatorName)\nOriginal: \(.replacement // "N/A")\n---"' "$JSON_REPORT" | \
    while IFS= read -r line; do
        if [[ $line == "Arquivo:"* ]]; then
            print_message $CYAN "$line"
        elif [[ $line == "Linha:"* ]]; then
            print_message $BLUE "$line"
        elif [[ $line == "Mutação:"* ]]; then
            print_message $YELLOW "$line"
        elif [[ $line == "Original:"* ]]; then
            print_message $PURPLE "$line"
        elif [[ $line == "---" ]]; then
            echo ""
        else
            echo "$line"
        fi
    done
fi

# Análise de tipos de mutação
print_header "🧬 TIPOS DE MUTAÇÃO"

echo ""
print_message $BLUE "Distribuição por tipo de mutador:"
echo ""

jq -r '[.files[].mutants[] | .mutatorName] | group_by(.) | map({mutator: .[0], count: length}) | sort_by(.count) | reverse | .[] | "\(.mutator): \(.count)"' "$JSON_REPORT" | \
while IFS=': ' read -r mutator count; do
    print_message $CYAN "  🔬 $mutator: $count mutações"
done

# Recomendações baseadas nos resultados
print_header "💡 RECOMENDAÇÕES"

echo ""
if [ "$survived_mutants" -gt 0 ]; then
    print_message $YELLOW "🎯 AÇÕES PRIORITÁRIAS:"
    print_message $YELLOW "   1. Analise as $survived_mutants mutações sobreviventes listadas acima"
    print_message $YELLOW "   2. Adicione testes específicos para detectar essas mutações"
    print_message $YELLOW "   3. Foque em valores limítrofes e condições extremas"
    echo ""
fi

if [ "$no_coverage_mutants" -gt 0 ]; then
    print_message $RED "📭 COBERTURA INSUFICIENTE:"
    print_message $RED "   • $no_coverage_mutants mutações não têm cobertura de teste"
    print_message $RED "   • Adicione testes para cobrir essas áreas do código"
    echo ""
fi

if (( $(echo "$killed_percent < 80" | bc -l) )); then
    print_message $YELLOW "📈 MELHORIAS SUGERIDAS:"
    print_message $YELLOW "   • Adicione testes para casos extremos (null, zero, limites)"
    print_message $YELLOW "   • Teste condições negativas e caminhos de erro"
    print_message $YELLOW "   • Verifique operadores relacionais (>=, <=, ==, !=)"
    print_message $YELLOW "   • Teste valores constantes específicos"
    echo ""
fi

print_message $GREEN "🔄 PRÓXIMOS PASSOS:"
print_message $GREEN "   1. Abra o relatório HTML para análise visual detalhada"
print_message $GREEN "   2. Implemente testes para mutações sobreviventes"
print_message $GREEN "   3. Execute novamente: ./run-mutation-tests.sh"
print_message $GREEN "   4. Compare resultados com baseline anterior"

# Gerar resumo em arquivo
SUMMARY_FILE="StrykerOutput/mutation-summary.txt"
{
    echo "RESUMO DOS TESTES DE MUTAÇÃO"
    echo "Data: $(date)"
    echo "================================"
    echo ""
    echo "MÉTRICAS GERAIS:"
    echo "  Total de Mutações: $total_mutants"
    echo "  Detectadas: $killed_mutants ($killed_percent%)"
    echo "  Sobreviventes: $survived_mutants ($survived_percent%)"
    echo "  Timeout: $timeout_mutants ($timeout_percent%)"
    echo "  Sem Cobertura: $no_coverage_mutants ($no_coverage_percent%)"
    echo ""
    echo "QUALIDADE: $quality ($killed_percent%)"
    echo ""
    if [ "$survived_mutants" -gt 0 ]; then
        echo "ATENÇÃO: $survived_mutants mutações sobreviventes requerem novos testes"
    fi
} > "$SUMMARY_FILE"

print_message $BLUE "📄 Resumo salvo em: $SUMMARY_FILE"

print_header "✅ ANÁLISE CONCLUÍDA"