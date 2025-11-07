# Testes de Caixa Preta - Sistema de Agendamento Médico

## Visão Geral

Este projeto implementa uma suíte completa de **testes de caixa preta** para o sistema de agendamento médico. Os testes focam no comportamento externo das APIs, validando funcionalidades sem conhecimento da implementação interna.

## Estrutura dos Testes

### 📁 Infrastructure
- **TestWebApplicationFactory**: Factory para criar instâncias de teste das APIs
- **ApiTestBase**: Classe base com métodos auxiliares para testes de API

### 📁 Fixtures
- **TestDataGenerator**: Gerador de dados de teste usando Bogus (dados realistas em português)

### 📁 CadastroService
- **PacienteBlackBoxTests**: Testes para endpoints de pacientes
- **MedicoBlackBoxTests**: Testes para endpoints de médicos

### 📁 AgendamentoService
- **ConsultaBlackBoxTests**: Testes para endpoints de consultas

### 📁 EndToEnd
- **FluxoCompletoAgendamentoTests**: Testes de fluxos completos do sistema

### 📁 Performance
- **PerformanceBlackBoxTests**: Testes de performance e carga

### 📁 Security
- **SecurityBlackBoxTests**: Testes básicos de segurança

## Tipos de Testes Implementados

### ✅ Testes Funcionais
- Validação de endpoints com dados válidos
- Validação de endpoints com dados inválidos
- Testes de validação de campos obrigatórios
- Testes de formatos de dados (email, telefone, etc.)
- Testes de códigos de status HTTP

### ✅ Testes de Borda (Edge Cases)
- IDs inexistentes
- Payloads vazios
- Valores negativos ou zero
- Dados malformados

### ✅ Testes de Performance
- Tempo de resposta
- Chamadas simultâneas
- Carga de trabalho

### ✅ Testes de Segurança
- Validação de entrada maliciosa (XSS, SQL Injection)
- Path traversal
- Payloads muito grandes
- Headers de segurança

## Como Executar

### Pré-requisitos
- .NET 8.0 SDK
- APIs de Cadastro e Agendamento funcionando

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

## Cenários de Teste

### 🏥 Cadastro de Pacientes
- ✅ Criar paciente com dados válidos
- ✅ Rejeitar paciente com dados inválidos
- ✅ Validar campos obrigatórios
- ✅ Validar formato de email e telefone
- ✅ Buscar paciente por ID
- ✅ Atualizar dados do paciente
- ✅ Excluir paciente

### 👨‍⚕️ Cadastro de Médicos
- ✅ Criar médico com dados válidos
- ✅ Validar CRM obrigatório
- ✅ Validar especialidade obrigatória
- ✅ Buscar médicos por especialidade
- ✅ Atualizar dados do médico
- ✅ Excluir médico

### 📅 Agendamento de Consultas
- ✅ Criar consulta com dados válidos
- ✅ Validar data futura obrigatória
- ✅ Validar paciente e médico obrigatórios
- ✅ Realizar consulta
- ✅ Filtrar consultas por período
- ✅ Listar todas as consultas

## Ferramentas Utilizadas

- **xUnit**: Framework de testes
- **FluentAssertions**: Assertions mais legíveis
- **Microsoft.AspNetCore.Mvc.Testing**: Testes de integração para APIs
- **Bogus**: Geração de dados de teste realistas
- **Newtonsoft.Json**: Serialização JSON

## Boas Práticas Implementadas

### 🎯 Testes de Caixa Preta
- Foco no comportamento externo
- Sem dependência da implementação interna
- Validação de contratos de API

### 📊 Dados de Teste
- Dados realistas usando Bogus
- Cenários válidos e inválidos
- Edge cases cobertos

### 🔒 Segurança
- Validação de entradas maliciosas
- Testes de path traversal
- Validação de payloads grandes

### ⚡ Performance
- Testes de tempo de resposta
- Testes de carga básicos
- Validação de chamadas simultâneas

## Próximos Passos

1. **Integração Contínua**: Configurar execução automática dos testes
2. **Relatórios**: Implementar relatórios detalhados de cobertura
3. **Dados de Teste**: Expandir cenários com mais variações
4. **Testes E2E**: Implementar fluxos completos entre serviços
5. **Monitoramento**: Adicionar métricas de performance dos testes

## Contribuindo

Para adicionar novos testes:

1. Identifique o tipo de teste (funcional, performance, segurança)
2. Use a classe base `ApiTestBase` apropriada
3. Utilize `TestDataGenerator` para dados de teste
4. Siga o padrão AAA (Arrange, Act, Assert)
5. Use `FluentAssertions` para assertions claras