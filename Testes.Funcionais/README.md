# Testes de Caixa Preta


### Comandos

```bash
# Executar todos os testes
dotnet test

# Executar testes específicos
dotnet test --filter "ClassName=PacienteBlackBoxTests"

# Executar com relatório de cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar testes de performance
dotnet test --filter "Category=Performance"
```

