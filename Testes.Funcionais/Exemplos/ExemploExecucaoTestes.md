# Exemplos de Execução dos Testes de Caixa Preta

## 🚀 Execução Básica

### Executar todos os testes
```bash
dotnet test Testes.Funcionais/
```

### Executar testes específicos por classe
```bash
# Testes de Pacientes
dotnet test --filter "ClassName=PacienteBlackBoxTests"

# Testes de Médicos  
dotnet test --filter "ClassName=MedicoBlackBoxTests"

# Testes de Consultas
dotnet test --filter "ClassName=ConsultaBlackBoxTests"
```

### Executar testes por categoria
```bash
# Testes de Performance
dotnet test --filter "FullyQualifiedName~Performance"

# Testes de Segurança
dotnet test --filter "FullyQualifiedName~Security"

# Testes End-to-End
dotnet test --filter "FullyQualifiedName~EndToEnd"
```

## 📊 Relatórios e Verbosidade

### Execução com relatório detalhado
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Execução com cobertura de código
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Execução com relatório TRX
```bash
dotnet test --logger trx --results-directory ./TestResults
```

## 🎯 Cenários Específicos

### Testar apenas endpoints GET
```bash
dotnet test --filter "TestName~Get"
```

### Testar apenas validações
```bash
dotnet test --filter "TestName~Validar"
```

### Testar apenas cenários de erro
```bash
dotnet test --filter "TestName~BadRequest"
```

## 🔧 Configurações Avançadas

### Executar com timeout personalizado
```bash
dotnet test --blame-hang-timeout 30s
```

### Executar em paralelo
```bash
dotnet test --parallel
```

### Executar com configuração específica
```bash
dotnet test --configuration Release
```

## 📋 Script Automatizado

Use o script fornecido para execução completa:
```bash
./run-tests.sh
```

## 🐛 Debug e Troubleshooting

### Executar um teste específico em modo debug
```bash
dotnet test --filter "TestName=Create_DeveRetornarOk_QuandoDadosValidos" --logger "console;verbosity=diagnostic"
```

### Verificar se as APIs estão rodando
Antes de executar os testes, certifique-se de que as APIs estão funcionando:

```bash
# Testar API de Agendamento
curl http://localhost:5000/Consulta/Ping

# Testar API de Cadastro  
curl http://localhost:5001/Paciente/GetAll
```

## 📈 Interpretando Resultados

### Exemplo de saída bem-sucedida:
```
✅ Passed: PacienteBlackBoxTests.Create_DeveRetornarCreated_QuandoDadosValidos
✅ Passed: PacienteBlackBoxTests.GetAll_DeveRetornarListaDePacientes_QuandoChamado
✅ Passed: MedicoBlackBoxTests.Create_DeveRetornarCreated_QuandoDadosValidos

Total tests: 25
Passed: 25
Failed: 0
Skipped: 0
```

### Exemplo de falha:
```
❌ Failed: ConsultaBlackBoxTests.Create_DeveRetornarOk_QuandoDadosValidos
   Expected response status to be OK, but found BadRequest
   Response content: {"message": "Paciente não encontrado"}
```

## 🔄 Integração Contínua

### GitHub Actions exemplo:
```yaml
- name: Run Black Box Tests
  run: |
    dotnet test Testes.Funcionais/ \
      --logger trx \
      --results-directory ./TestResults \
      --collect:"XPlat Code Coverage"
```

### Azure DevOps exemplo:
```yaml
- task: DotNetCoreCLI@2
  displayName: 'Run Black Box Tests'
  inputs:
    command: 'test'
    projects: 'Testes.Funcionais/*.csproj'
    arguments: '--logger trx --collect:"XPlat Code Coverage"'
```