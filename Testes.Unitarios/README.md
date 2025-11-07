# Testes de Caixa Branca - Sistema de Agendamento Médico

## Visão Geral

Este projeto implementa uma suíte completa de **testes de caixa branca** (white box tests) para o sistema de agendamento médico. Os testes focam na estrutura interna do código, cobertura de código, fluxos de controle e validação da lógica de negócio.

## Diferença entre Caixa Preta vs Caixa Branca

### 🔲 Caixa Preta (Black Box)
- Testa comportamento externo
- Não conhece implementação interna
- Foca em entradas e saídas
- Testa através de interfaces públicas (APIs)

### ⬜ Caixa Branca (White Box)
- Testa estrutura interna do código
- Conhece e utiliza detalhes da implementação
- Foca em fluxos de controle e lógica
- Testa métodos, classes e componentes internos

## Estrutura dos Testes

### 📁 Domain (Testes de Entidades e Regras de Negócio)
- **Entities**: Testes das entidades de domínio
- **ValueObjects**: Testes de objetos de valor
- **Services**: Testes de serviços de domínio
- **Validators**: Testes de validadores de negócio

### 📁 Application (Testes de Casos de Uso)
- **UseCases**: Testes dos casos de uso da aplicação
- **Services**: Testes de serviços de aplicação
- **Handlers**: Testes de handlers de comandos e queries
- **Mappers**: Testes de mapeamento de dados

### 📁 Infrastructure (Testes de Infraestrutura)
- **Repositories**: Testes de repositórios
- **Data**: Testes de acesso a dados
- **External**: Testes de serviços externos
- **Configuration**: Testes de configuração

### 📁 Integration (Testes de Integração Interna)
- **Database**: Testes com banco de dados em memória
- **Services**: Testes de integração entre serviços
- **Workflows**: Testes de fluxos completos internos

## Tipos de Testes Implementados

### ✅ Testes Unitários
- Testam unidades isoladas de código
- Usam mocks para dependências
- Validam lógica de negócio específica
- Cobertura de todos os caminhos de código

### ✅ Testes de Integração
- Testam interação entre componentes
- Usam banco de dados em memória
- Validam fluxos de dados internos
- Testam configurações e injeção de dependência

### ✅ Testes de Cobertura
- Garantem cobertura mínima de código
- Identificam código não testado
- Validam todos os branches e condições
- Relatórios de cobertura detalhados

### ✅ Testes de Performance Interna
- Testam algoritmos e estruturas de dados
- Validam complexidade computacional
- Identificam gargalos internos
- Testam uso de memória

## Ferramentas Utilizadas

- **xUnit**: Framework de testes
- **FluentAssertions**: Assertions mais legíveis
- **Moq**: Framework de mocking
- **AutoFixture**: Geração automática de dados de teste
- **EntityFramework InMemory**: Banco de dados em memória para testes
- **Coverlet**: Análise de cobertura de código

## Padrões de Teste

### 🎯 AAA Pattern (Arrange, Act, Assert)
```csharp
[Fact]
public void DeveCalcularIdadeCorretamente()
{
    // Arrange
    var dataNascimento = new DateTime(1990, 1, 1);
    var paciente = new Paciente("João", dataNascimento);
    
    // Act
    var idade = paciente.CalcularIdade();
    
    // Assert
    idade.Should().Be(34);
}
```

### 🎯 Mocking de Dependências
```csharp
[Fact]
public void DeveSalvarPacienteNoRepositorio()
{
    // Arrange
    var mockRepository = new Mock<IPacienteRepository>();
    var service = new PacienteService(mockRepository.Object);
    
    // Act & Assert
    mockRepository.Verify(r => r.Save(It.IsAny<Paciente>()), Times.Once);
}
```

### 🎯 Testes Parametrizados
```csharp
[Theory]
[InlineData("", false)]
[InlineData("12345", true)]
[InlineData("ABCDE", true)]
public void DeveValidarCrm(string crm, bool esperado)
{
    // Testa múltiplos cenários
}
```

## Cobertura de Código

### Metas de Cobertura
- **Entidades de Domínio**: 100%
- **Serviços de Aplicação**: 95%
- **Repositórios**: 90%
- **Controladores**: 85%
- **Geral**: 90%

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

## Boas Práticas Implementadas

### 🔍 Testes Determinísticos
- Resultados sempre iguais para mesmas entradas
- Não dependem de fatores externos
- Usam dados controlados

### ⚡ Testes Rápidos
- Executam em milissegundos
- Usam mocks para dependências externas
- Banco de dados em memória

### 🎯 Testes Focados
- Cada teste valida uma única funcionalidade
- Nomes descritivos e claros
- Cenários bem definidos

### 🔄 Testes Independentes
- Não dependem da ordem de execução
- Estado limpo entre testes
- Isolamento completo

## Próximos Passos

1. **Mutation Testing**: Implementar testes de mutação
2. **Property-Based Testing**: Adicionar testes baseados em propriedades
3. **Benchmarking**: Testes de performance detalhados
4. **Análise Estática**: Integração com ferramentas de análise de código
5. **CI/CD**: Automação completa dos testes