## Início Rápido

```bash
# 1. Navegar para o diretório
cd Testes.Mutacao

# 2. Executar pipeline completo
./run-complete-mutation-pipeline.sh

# 3. Abrir relatório HTML gerado
open StrykerOutput/reports/mutation-report.html
```


## Comandos

```bash
# Executar apenas testes de mutação
./run-mutation-tests.sh

# Analisar resultados existentes
./analyze-mutation-results.sh

# Pipeline completo (recomendado)
./run-complete-mutation-pipeline.sh

# Executar com configuração específica
dotnet stryker --config-file stryker-config.json

# Executar apenas para arquivos específicos
dotnet stryker --mutate "**/PacienteLogic.cs"

# Gerar apenas relatório HTML
dotnet stryker --reporters html

# Executar com mais threads (mais rápido)
dotnet stryker --concurrency 8
```

