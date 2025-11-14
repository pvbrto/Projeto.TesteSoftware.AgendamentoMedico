# Testes de Caixa Branca


### Relatórios
```bash
# Gerar relatório de cobertura
dotnet test --collect:"XPlat Code Coverage"

# Gerar relatório HTML
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report
```

## Como Executar

### Todos os Testes
```bash
dotnet test Testes.Unitarios/
```

### Por Categoria
```bash
# Testes de Domínio
dotnet test --filter "Category=Domain"

# Testes de Aplicação
dotnet test --filter "Category=Application"

# Testes de Infraestrutura
dotnet test --filter "Category=Infrastructure"
```

### Com Cobertura
```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

