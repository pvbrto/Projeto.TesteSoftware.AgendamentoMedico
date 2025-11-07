# Comparação: Testes de Caixa Branca vs Caixa Preta

## Visão Geral

Este documento demonstra as diferenças práticas entre **testes de caixa branca** e **testes de caixa preta** no contexto do sistema de agendamento médico.

## Diferenças Fundamentais

### 🔲 Testes de Caixa Preta (Black Box)
- **Foco**: Comportamento externo
- **Conhecimento**: Não conhece implementação interna
- **Entrada/Saída**: Testa apenas interfaces públicas
- **Objetivo**: Validar se o sistema atende aos requisitos

### ⬜ Testes de Caixa Branca (White Box)
- **Foco**: Estrutura interna do código
- **Conhecimento**: Conhece e utiliza detalhes da implementação
- **Cobertura**: Testa todos os caminhos de código
- **Objetivo**: Garantir qualidade interna e cobertura completa

## Exemplos Práticos

### Exemplo 1: Validação de Email

#### 🔲 Teste de Caixa Preta
```csharp
[Fact]
public async Task Create_DeveRetornarBadRequest_QuandoEmailInvalido()
{
    // Arrange
    var pacienteData = new { Nome = "João", Email = "email-invalido" };

    // Act - Chama API externa
    var response = await PostAsync("/Paciente", pacienteData);

    // Assert - Valida apenas o resultado
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
}
```

#### ⬜ Teste de Caixa Branca
```csharp
[Theory]
[InlineData("", false)]                    // Branch: email vazio
[InlineData("email-sem-arroba", false)]    // Branch: formato inválido
[InlineData("@sem-usuario.com", false)]    // Branch: usuário vazio
[InlineData("usuario@", false)]            // Branch: domínio vazio
[InlineData("teste@exemplo.com", true)]    // Branch: email válido
public void ValidarEmail_DeveCobrirTodosBranches(string email, bool esperado)
{
    // Act - Testa método interno diretamente
    var resultado = ValidarEmail(email);

    // Assert - Valida lógica interna
    resultado.Should().Be(esperado);
}
```

### Exemplo 2: Agendamento de Consulta

#### 🔲 Teste de Caixa Preta
```csharp
[Fact]
public async Task Create_DeveRetornarCreated_QuandoDadosValidos()
{
    // Arrange
    var consultaData = new
    {
        PacienteId = 1,
        MedicoId = 2,
        DataHora = DateTime.Now.AddDays(1)
    };

    // Act - Chama endpoint da API
    var response = await PostAsync("/Consulta", consultaData);

    // Assert - Valida apenas resposta HTTP
    response.StatusCode.Should().Be(HttpStatusCode.Created);
}
```

#### ⬜ Teste de Caixa Branca
```csharp
[Fact]
public async Task AgendarConsulta_DeveExecutarFluxoCompleto_QuandoDadosValidos()
{
    // Arrange - Mock de todas as dependências
    var mockPacienteRepo = new Mock<IPacienteRepository>();
    var mockMedicoRepo = new Mock<IMedicoRepository>();
    var mockConsultaRepo = new Mock<IConsultaRepository>();
    
    mockPacienteRepo.Setup(r => r.ObterPorIdAsync(1))
        .ReturnsAsync(new Paciente { Nome = "João" });
    
    mockMedicoRepo.Setup(r => r.ObterPorIdAsync(2))
        .ReturnsAsync(new Medico { Nome = "Dr. Pedro", Ativo = true });
    
    mockConsultaRepo.Setup(r => r.VerificarDisponibilidadeAsync(2, It.IsAny<DateTime>()))
        .ReturnsAsync(true);

    var service = new AgendamentoService(mockConsultaRepo.Object, 
        mockMedicoRepo.Object, mockPacienteRepo.Object);

    // Act - Testa lógica de negócio diretamente
    var resultado = await service.AgendarConsultaAsync(request);

    // Assert - Valida fluxo interno completo
    resultado.Sucesso.Should().BeTrue();
    
    // Verifica se todos os métodos foram chamados corretamente
    mockPacienteRepo.Verify(r => r.ObterPorIdAsync(1), Times.Once);
    mockMedicoRepo.Verify(r => r.ObterPorIdAsync(2), Times.Once);
    mockConsultaRepo.Verify(r => r.VerificarDisponibilidadeAsync(2, It.IsAny<DateTime>()), Times.Once);
    mockConsultaRepo.Verify(r => r.SalvarAsync(It.IsAny<Consulta>()), Times.Once);
}
```

### Exemplo 3: Validação de Horário Comercial

#### 🔲 Teste de Caixa Preta
```csharp
[Fact]
public async Task Create_DeveRetornarBadRequest_QuandoHorarioInvalido()
{
    // Arrange - Sábado às 14h
    var consultaData = new
    {
        PacienteId = 1,
        MedicoId = 2,
        DataHora = new DateTime(2024, 11, 9, 14, 0, 0) // Sábado
    };

    // Act
    var response = await PostAsync("/Consulta", consultaData);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
}
```

#### ⬜ Teste de Caixa Branca
```csharp
[Theory]
[InlineData(2024, 11, 9, 14, 0, false)]   // Sábado - Branch 1
[InlineData(2024, 11, 10, 14, 0, false)]  // Domingo - Branch 2
[InlineData(2024, 11, 8, 7, 59, false)]   // Antes 8h - Branch 3
[InlineData(2024, 11, 8, 8, 0, true)]     // Exato 8h - Branch 4
[InlineData(2024, 11, 8, 14, 30, true)]   // Horário comercial - Branch 5
[InlineData(2024, 11, 8, 18, 0, false)]   // Exato 18h - Branch 6
[InlineData(2024, 11, 8, 18, 1, false)]   // Após 18h - Branch 7
public void ValidarHorarioComercial_DeveCobrirTodosBranches(
    int ano, int mes, int dia, int hora, int minuto, bool esperado)
{
    // Arrange
    var consulta = new Consulta 
    { 
        DataHora = new DateTime(ano, mes, dia, hora, minuto, 0) 
    };

    // Act - Testa método interno
    var resultado = consulta.ValidarHorarioComercial();

    // Assert - Valida cada branch específico
    resultado.Should().Be(esperado);
}
```

## Cobertura de Código

### 🔲 Caixa Preta - Não mede cobertura interna
```bash
# Testa apenas endpoints
curl -X POST http://localhost:5000/Consulta \
  -H "Content-Type: application/json" \
  -d '{"pacienteId": 1, "medicoId": 2, "dataHora": "2024-11-09T14:00:00"}'

# Resultado: 400 Bad Request
# Não sabemos qual linha de código causou o erro
```

### ⬜ Caixa Branca - Mede cobertura completa
```bash
# Executa com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Resultado detalhado:
# - Linha 45: if (dataHora.DayOfWeek == DayOfWeek.Saturday) ✅ Coberta
# - Linha 46: return false; ✅ Coberta
# - Linha 48: if (dataHora.DayOfWeek == DayOfWeek.Sunday) ✅ Coberta
# - Linha 49: return false; ✅ Coberta
# - Cobertura total: 95%
```

## Estratégias de Teste

### 🔲 Caixa Preta - Baseada em Requisitos
```
Requisito: "Sistema deve rejeitar agendamentos em finais de semana"

Casos de Teste:
✅ Agendar no sábado → Deve retornar erro
✅ Agendar no domingo → Deve retornar erro
✅ Agendar na segunda → Deve aceitar
```

### ⬜ Caixa Branca - Baseada em Código
```csharp
// Código a ser testado:
public bool ValidarHorarioComercial()
{
    if (DataHora.DayOfWeek == DayOfWeek.Saturday) return false;  // Branch 1
    if (DataHora.DayOfWeek == DayOfWeek.Sunday) return false;    // Branch 2
    
    var hora = DataHora.Hour;
    return hora >= 8 && hora < 18;  // Branch 3 e 4
}

// Casos de Teste (um para cada branch):
✅ Branch 1: Sábado
✅ Branch 2: Domingo  
✅ Branch 3: hora >= 8 (true)
✅ Branch 4: hora < 18 (true)
✅ Branch 5: hora >= 8 (false)
✅ Branch 6: hora < 18 (false)
```

## Ferramentas Utilizadas

### 🔲 Caixa Preta
- **Postman/Insomnia**: Testes de API
- **Selenium**: Testes de UI
- **JMeter**: Testes de carga
- **Newman**: Automação de testes de API

### ⬜ Caixa Branca
- **xUnit**: Framework de testes unitários
- **Moq**: Mocking de dependências
- **Coverlet**: Análise de cobertura
- **ReportGenerator**: Relatórios de cobertura
- **Stryker.NET**: Testes de mutação

## Quando Usar Cada Tipo

### 🔲 Use Caixa Preta quando:
- Testar funcionalidades do ponto de vista do usuário
- Validar contratos de API
- Testar integração entre sistemas
- Fazer testes de aceitação
- Testar sem conhecer a implementação

### ⬜ Use Caixa Branca quando:
- Garantir cobertura de código
- Testar lógica de negócio complexa
- Validar tratamento de exceções
- Testar algoritmos específicos
- Fazer refatoração com segurança

## Complementaridade

Os dois tipos de teste são **complementares**:

```
🔲 Caixa Preta: "O sistema faz o que deveria fazer?"
⬜ Caixa Branca: "O sistema faz da forma correta?"

Juntos: "O sistema faz o que deveria fazer, da forma correta?"
```

## Exemplo de Execução

### Executar Testes de Caixa Preta
```bash
cd Testes.Funcionais
./run-tests.sh
```

### Executar Testes de Caixa Branca
```bash
cd Testes.Unitarios
./run-white-box-tests.sh
```

### Comparar Resultados
```bash
# Caixa Preta: 61 passou, 32 falhou (APIs não rodando)
# Caixa Branca: 45 passou, 0 falhou (100% cobertura)
```

## Conclusão

- **Caixa Preta**: Essencial para validar requisitos e comportamento externo
- **Caixa Branca**: Fundamental para qualidade interna e manutenibilidade
- **Ambos**: Necessários para um sistema robusto e confiável