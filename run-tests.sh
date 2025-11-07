#!/bin/bash

echo "🧪 Executando Testes de Caixa Preta - Sistema de Agendamento Médico"
echo "=================================================================="

# Cores para output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para executar testes com categoria
run_test_category() {
    local category=$1
    local description=$2
    
    echo -e "\n${YELLOW}📋 Executando: $description${NC}"
    echo "----------------------------------------"
    
    if dotnet test Testes.Funcionais/Testes.Funcionais.csproj --filter "FullyQualifiedName~$category" --verbosity minimal; then
        echo -e "${GREEN}✅ $description - PASSOU${NC}"
    else
        echo -e "${RED}❌ $description - FALHOU (mas pode ser esperado se APIs não estiverem rodando)${NC}"
    fi
}

# Verificar se as APIs estão rodando
check_api() {
    local url=$1
    local name=$2
    
    if curl -s --connect-timeout 3 "$url" > /dev/null 2>&1; then
        echo -e "${GREEN}✅ $name está rodando${NC}"
        return 0
    else
        echo -e "${YELLOW}⚠️  $name não está rodando${NC}"
        return 1
    fi
}

echo -e "\n${BLUE}🔍 Verificando status das APIs...${NC}"
check_api "http://localhost:5000" "API de Agendamento (porta 5000)"
check_api "http://localhost:5001" "API de Cadastro (porta 5001)"

echo -e "\n${YELLOW}🏗️  Compilando projeto de testes...${NC}"
dotnet build Testes.Funcionais/Testes.Funcionais.csproj

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Falha na compilação${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Compilação concluída${NC}"

# Testes que sempre funcionam (não dependem das APIs)
echo -e "\n${BLUE}🎯 Executando testes independentes (sempre funcionam)...${NC}"
run_test_category "Validation" "Testes de Validação de Dados"
run_test_category "PerformanceSimpleTests" "Testes de Performance Básicos"

# Testes que dependem das APIs
echo -e "\n${BLUE}🌐 Executando testes que dependem das APIs...${NC}"
run_test_category "SimpleTests" "Testes Simples de API"

# Testes originais (podem falhar se APIs não estiverem rodando)
echo -e "\n${BLUE}🔧 Executando testes originais (podem falhar se APIs não estiverem rodando)...${NC}"
run_test_category "PacienteBlackBoxTests" "Testes Completos de Pacientes"
run_test_category "ConsultaBlackBoxTests" "Testes Completos de Consultas"

echo -e "\n${YELLOW}📊 Executando TODOS os testes...${NC}"
dotnet test Testes.Funcionais/Testes.Funcionais.csproj --logger "console;verbosity=normal"

echo -e "\n${GREEN}🎉 Execução de testes concluída!${NC}"
echo -e "${BLUE}💡 Dica: Para que todos os testes passem, inicie as APIs:${NC}"
echo -e "   - API de Agendamento: http://localhost:5000"
echo -e "   - API de Cadastro: http://localhost:5001"
echo "=================================================================="