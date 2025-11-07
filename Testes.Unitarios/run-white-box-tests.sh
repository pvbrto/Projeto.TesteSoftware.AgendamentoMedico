#!/bin/bash

echo "🧪 Executando Testes de Caixa Branca - Sistema de Agendamento Médico"
echo "===================================================================="

# Cores para output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Função para executar testes com categoria
run_test_category() {
    local category=$1
    local description=$2
    
    echo -e "\n${YELLOW}📋 Executando: $description${NC}"
    echo "----------------------------------------"
    
    if dotnet test Testes.Unitarios/Testes.Unitarios.csproj --filter "Category=$category" --verbosity minimal; then
        echo -e "${GREEN}✅ $description - PASSOU${NC}"
    else
        echo -e "${RED}❌ $description - FALHOU${NC}"
    fi
}

# Função para executar testes com cobertura
run_with_coverage() {
    local description=$1
    local filter=$2
    
    echo -e "\n${CYAN}📊 Executando com cobertura: $description${NC}"
    echo "----------------------------------------"
    
    dotnet test Testes.Unitarios/Testes.Unitarios.csproj \
        --filter "$filter" \
        --collect:"XPlat Code Coverage" \
        --results-directory ./coverage \
        --verbosity minimal
}

echo -e "\n${BLUE}🏗️  Compilando projeto de testes...${NC}"
dotnet build Testes.Unitarios/Testes.Unitarios.csproj

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Falha na compilação${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Compilação concluída${NC}"

# Testes por categoria (Caixa Branca)
echo -e "\n${BLUE}⬜ Executando testes de caixa branca por categoria...${NC}"

run_test_category "Domain" "Testes de Domínio (Entidades e Regras de Negócio)"
run_test_category "Application" "Testes de Aplicação (Casos de Uso e Serviços)"
run_test_category "Infrastructure" "Testes de Infraestrutura (Repositórios e Dados)"
run_test_category "Integration" "Testes de Integração (Fluxos Completos)"
run_test_category "Coverage" "Testes de Cobertura (Todos os Caminhos)"

# Testes por tipo
echo -e "\n${BLUE}🎯 Executando testes por tipo...${NC}"

run_test_category "Unit" "Testes Unitários"
run_test_category "Integration" "Testes de Integração"

# Executar todos os testes com cobertura
echo -e "\n${CYAN}📊 Executando TODOS os testes com análise de cobertura...${NC}"
run_with_coverage "Cobertura Completa" ""

# Gerar relatório de cobertura se disponível
if command -v reportgenerator &> /dev/null; then
    echo -e "\n${CYAN}📈 Gerando relatório de cobertura HTML...${NC}"
    reportgenerator \
        -reports:"coverage/**/coverage.cobertura.xml" \
        -targetdir:"coverage-report" \
        -reporttypes:"Html;TextSummary"
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Relatório gerado em: coverage-report/index.html${NC}"
    else
        echo -e "${YELLOW}⚠️  Erro ao gerar relatório HTML${NC}"
    fi
else
    echo -e "\n${YELLOW}⚠️  ReportGenerator não encontrado. Para instalar:${NC}"
    echo -e "   dotnet tool install -g dotnet-reportgenerator-globaltool"
fi

# Executar testes de performance
echo -e "\n${BLUE}⚡ Executando testes de performance...${NC}"
dotnet test Testes.Unitarios/Testes.Unitarios.csproj \
    --filter "TestName~Performance" \
    --verbosity minimal

# Executar testes de mutação se disponível
if command -v dotnet-stryker &> /dev/null; then
    echo -e "\n${CYAN}🧬 Executando testes de mutação...${NC}"
    dotnet stryker --project Testes.Unitarios/Testes.Unitarios.csproj
else
    echo -e "\n${YELLOW}⚠️  Stryker.NET não encontrado. Para instalar:${NC}"
    echo -e "   dotnet tool install -g dotnet-stryker"
fi

# Resumo final
echo -e "\n${BLUE}📋 Executando resumo final...${NC}"
dotnet test Testes.Unitarios/Testes.Unitarios.csproj --logger "console;verbosity=normal"

echo -e "\n${GREEN}🎉 Execução de testes de caixa branca concluída!${NC}"
echo -e "\n${BLUE}📊 Relatórios disponíveis:${NC}"
echo -e "   - Cobertura de código: coverage-report/index.html"
echo -e "   - Resultados XML: coverage/**/coverage.cobertura.xml"
echo -e "\n${BLUE}💡 Comandos úteis:${NC}"
echo -e "   - Ver cobertura: dotnet test --collect:\"XPlat Code Coverage\""
echo -e "   - Testes específicos: dotnet test --filter \"Category=Domain\""
echo -e "   - Debug: dotnet test --logger \"console;verbosity=diagnostic\""
echo -e "\n${BLUE}🎯 Diferenças Caixa Branca vs Caixa Preta:${NC}"
echo -e "   ⬜ Caixa Branca: Testa estrutura interna, cobertura, lógica"
echo -e "   🔲 Caixa Preta: Testa comportamento externo, APIs, contratos"
echo "===================================================================="