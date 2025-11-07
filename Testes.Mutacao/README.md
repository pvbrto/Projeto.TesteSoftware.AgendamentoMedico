# 🧬 Testes de Mutação - Sistema de Agendamento Médico

## 📋 Visão Geral

Os **Testes de Mutação** são uma técnica avançada de teste que valida a qualidade dos próprios testes. O conceito é simples mas poderoso: introduzir pequenas mudanças (mutações) no código fonte e verificar se os testes conseguem detectar essas mudanças.

## 🚀 Início Rápido

```bash
# 1. Navegar para o diretório
cd Testes.Mutacao

# 2. Executar pipeline completo
./run-complete-mutation-pipeline.sh

# 3. Abrir relatório HTML gerado
open StrykerOutput/reports/mutation-report.html
```

## 📁 Estrutura do Projeto

```
Testes.Mutacao/
├── 📄 Testes.Mutacao.csproj          # Projeto principal
├── 📄 stryker-config.json            # Configuração do Stryker
├── 📄 README.md                      # Este arquivo
├── 📄 Estrategias-Mutacao.md         # Guia de estratégias avançadas
├── 
├── 🗂️ CodeUnderTest/                 # Código a ser testado
│   ├── PacienteLogic.cs             # Lógica de pacientes
│   ├── ConsultaLogic.cs             # Lógica de consultas  
│   └── AgendamentoLogic.cs          # Lógica de agendamento
├── 
├── 🗂️ MutationTests/                # Testes de mutação
│   ├── PacienteLogicMutationTests.cs
│   ├── ConsultaLogicMutationTests.cs
│   └── AgendamentoLogicMutationTests.cs
├── 
└── 🗂️ Scripts/                      # Scripts de automação
    ├── run-mutation-tests.sh        # Executa testes de mutação
    ├── analyze-mutation-results.sh  # Analisa resultados
    └── run-complete-mutation-pipeline.sh # Pipeline completo
```

## 🎯 Objetivo dos Testes de Mutação

### O que são Mutações?
Mutações são pequenas alterações no código que simulam bugs comuns:
- Trocar `>` por `>=`
- Mudar `+` para `-`
- Alterar `true` para `false`
- Trocar `==` por `!=`

### Por que são Importantes?
- **Validam a qualidade dos testes**: Se uma mutação não é detectada, o teste pode estar incompleto
- **Identificam código não testado**: Mutações que passam indicam gaps na cobertura
- **Melhoram a confiança**: Testes que detectam mutações são mais robustos

## 🔬 Como Funcionam

### 1. Mutação do Código
```csharp
// Código Original
if (idade >= 18) {
    return true;
}

// Mutação 1: Operador Relacional
if (idade > 18) {  // >= mudou para >
    return true;
}

// Mutação 2: Valor Constante
if (idade >= 19) {  // 18 mudou para 19
    return true;
}
```

### 2. Execução dos Testes
- Cada mutação é testada individualmente
- Os testes são executados contra o código mutado
- Se os testes **falham**, a mutação foi **detectada** ✅
- Se os testes **passam**, a mutação **sobreviveu** ❌

### 3. Cálculo do Mutation Score
```
Mutation Score = (Mutações Detectadas / Total de Mutações) × 100%
```

## 🛠️ Ferramentas Utilizadas

### Stryker.NET
- **Ferramenta principal** para testes de mutação em .NET
- **Mutações automáticas** em operadores, condições e valores
- **Relatórios detalhados** em HTML e JSON
- **Integração** com pipelines de CI/CD

### Tipos de Mutações Suportadas

#### 1. Operadores Aritméticos
```csharp
// Original → Mutação
+  →  -
-  →  +
*  →  /
/  →  *
%  →  *
```

#### 2. Operadores Relacionais
```csharp
// Original → Mutação
>   →  >=, <, <=, ==, !=
>=  →  >, <, <=, ==, !=
<   →  <=, >, >=, ==, !=
<=  →  <, >, >=, ==, !=
==  →  !=, >, >=, <, <=
!=  →  ==, >, >=, <, <=
```

#### 3. Operadores Lógicos
```csharp
// Original → Mutação
&&  →  ||
||  →  &&
!   →  (removido)
```

#### 4. Valores Constantes
```csharp
// Original → Mutação
true   →  false
false  →  true
0      →  1
1      →  0
""     →  "Stryker was here!"
```

#### 5. Operadores de Atribuição
```csharp
// Original → Mutação
+=  →  -=
-=  →  +=
*=  →  /=
/=  →  *=
```

## 📊 Interpretando Resultados

### Mutation Score Ideal
- **90-100%**: Excelente qualidade de testes
- **80-89%**: Boa qualidade, algumas melhorias possíveis
- **70-79%**: Qualidade moderada, precisa de atenção
- **< 70%**: Qualidade baixa, testes insuficientes

### Status das Mutações

#### ✅ Killed (Detectada)
```
Mutação: idade >= 18 → idade > 18
Status: KILLED
Teste que detectou: EhMaiorDeIdade_DeveRetornarTrue_QuandoPacienteExatamente18Anos
```

#### ❌ Survived (Sobreviveu)
```
Mutação: nome != null → nome == null
Status: SURVIVED
Problema: Nenhum teste verifica comportamento com nome null
```

#### ⏭️ No Coverage (Sem Cobertura)
```
Mutação: código nunca executado
Status: NO COVERAGE
Problema: Código não é testado
```

#### ⏸️ Timeout (Timeout)
```
Mutação: causou loop infinito
Status: TIMEOUT
Resultado: Considerado detectado (teste preveniu problema)
```

## 🚀 Como Executar

### Instalação do Stryker
```bash
# Instalar globalmente
dotnet tool install -g dotnet-stryker

# Ou localmente no projeto
dotnet new tool-manifest
dotnet tool install dotnet-stryker
```

### Configuração Básica
```bash
# Inicializar configuração
dotnet stryker init

# Executar testes de mutação
dotnet stryker
```

### Configuração Avançada
Arquivo `stryker-config.json`:
```json
{
  "stryker-config": {
    "project": "Testes.Mutacao.csproj",
    "test-projects": ["../Testes.Unitarios/Testes.Unitarios.csproj"],
    "reporters": ["html", "json", "console"],
    "thresholds": {
      "high": 90,
      "low": 70,
      "break": 60
    },
    "mutation-level": "Complete",
    "timeout-ms": 10000
  }
}
```

## 📈 Estratégias de Melhoria

### 1. Analisar Mutações Sobreviventes
```csharp
// Mutação sobreviveu: idade > 18 → idade >= 18
// Problema: Não há teste para idade exatamente 18

[Fact]
public void EhMaiorDeIdade_DeveRetornarTrue_QuandoIdadeExatamente18()
{
    // Arrange
    var idade = 18;
    
    // Act
    var resultado = EhMaiorDeIdade(idade);
    
    // Assert
    resultado.Should().BeTrue();
}
```

### 2. Testar Valores Limítrofes
```csharp
[Theory]
[InlineData(17, false)]  // Menor que 18
[InlineData(18, true)]   // Exatamente 18
[InlineData(19, true)]   // Maior que 18
public void EhMaiorDeIdade_DeveValidarCorretamente(int idade, bool esperado)
{
    var resultado = EhMaiorDeIdade(idade);
    resultado.Should().Be(esperado);
}
```

### 3. Testar Condições Negativas
```csharp
[Fact]
public void ValidarEmail_DeveRetornarFalse_QuandoEmailNull()
{
    // Testa especificamente o caso null
    var resultado = ValidarEmail(null);
    resultado.Should().BeFalse();
}
```

## 🎯 Casos de Uso Específicos

### Validação de CPF
```csharp
// Código a ser testado
public bool ValidarCpf(string cpf)
{
    if (string.IsNullOrEmpty(cpf)) return false;  // Mutação: || → &&
    if (cpf.Length != 11) return false;           // Mutação: != → ==
    return CalcularDigitoVerificador(cpf);        // Mutação: return true
}

// Testes necessários para detectar mutações
[Theory]
[InlineData(null, false)]        // Detecta mutação em IsNullOrEmpty
[InlineData("", false)]          // Detecta mutação em IsNullOrEmpty  
[InlineData("123", false)]       // Detecta mutação em Length != 11
[InlineData("12345678901", ?)]   // Detecta mutação no return
```

### Cálculo de Idade
```csharp
// Código a ser testado
public int CalcularIdade(DateTime nascimento)
{
    var hoje = DateTime.Now;
    var idade = hoje.Year - nascimento.Year;     // Mutação: - → +
    
    if (hoje.Month < nascimento.Month ||         // Mutação: < → <=
        (hoje.Month == nascimento.Month &&       // Mutação: == → !=
         hoje.Day < nascimento.Day))             // Mutação: < → <=
    {
        idade--;                                 // Mutação: -- → ++
    }
    
    return idade;
}
```

## 📋 Checklist de Qualidade

### ✅ Antes dos Testes de Mutação
- [ ] Cobertura de código > 80%
- [ ] Todos os testes unitários passando
- [ ] Testes de integração funcionando
- [ ] Documentação dos testes atualizada

### ✅ Durante a Análise
- [ ] Mutation Score > 80%
- [ ] Mutações sobreviventes analisadas
- [ ] Novos testes criados para gaps identificados
- [ ] Casos extremos cobertos

### ✅ Após Melhorias
- [ ] Mutation Score melhorado
- [ ] Testes mais robustos
- [ ] Documentação atualizada
- [ ] CI/CD configurado com thresholds

## 🔄 Integração com CI/CD

### GitHub Actions
```yaml
name: Mutation Testing
on: [push, pull_request]

jobs:
  mutation-test:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    
    - name: Install Stryker
      run: dotnet tool install -g dotnet-stryker
    
    - name: Run Mutation Tests
      run: dotnet stryker --reporter html --reporter json
    
    - name: Upload Results
      uses: actions/upload-artifact@v3
      with:
        name: mutation-report
        path: StrykerOutput/
```

## 📊 Métricas e KPIs

### Métricas Principais
- **Mutation Score**: Percentual de mutações detectadas
- **Killed Mutants**: Número de mutações detectadas
- **Survived Mutants**: Número de mutações não detectadas
- **Coverage**: Percentual de código coberto por testes

### Relatórios Gerados
- **HTML Report**: Visualização interativa
- **JSON Report**: Dados para integração
- **Console Output**: Resumo rápido
- **Baseline**: Comparação com execuções anteriores

## 🎓 Benefícios dos Testes de Mutação

### Para Desenvolvedores
- **Confiança**: Testes mais robustos
- **Qualidade**: Código mais confiável
- **Aprendizado**: Melhores práticas de teste

### Para o Projeto
- **Redução de Bugs**: Detecção precoce de problemas
- **Manutenibilidade**: Refatoração mais segura
- **Documentação**: Testes como especificação viva

### Para a Equipe
- **Padronização**: Critérios objetivos de qualidade
- **Melhoria Contínua**: Feedback constante
- **Conhecimento**: Compartilhamento de boas práticas

## 💻 Exemplos Práticos

### Exemplo 1: Detectando Mutação em Operador Relacional

```csharp
// Código original
public bool EhMaiorDeIdade(int idade)
{
    return idade >= 18;  // Stryker vai mutar para: idade > 18
}

// Teste que DETECTA a mutação
[Theory]
[InlineData(17, false)]  // Menor que 18
[InlineData(18, true)]   // CRÍTICO: detecta >= → >
[InlineData(19, true)]   // Maior que 18
public void EhMaiorDeIdade_DeveDetectarMutacao(int idade, bool esperado)
{
    var resultado = EhMaiorDeIdade(idade);
    resultado.Should().Be(esperado);
}
```

### Exemplo 2: Detectando Mutação em Operador Aritmético

```csharp
// Código original
public double CalcularDesconto(double valor, double percentual)
{
    return valor - (valor * percentual / 100);  // Múltiplas mutações possíveis
}

// Testes que DETECTAM mutações
[Theory]
[InlineData(100.0, 10.0, 90.0)]   // Caso padrão
[InlineData(100.0, 0.0, 100.0)]   // Zero desconto - detecta mutações
[InlineData(0.0, 10.0, 0.0)]      // Valor zero - detecta mutações
public void CalcularDesconto_DeveDetectarMutacoes(double valor, double perc, double esperado)
{
    var resultado = CalcularDesconto(valor, perc);
    resultado.Should().Be(esperado);
}
```

### Exemplo 3: Detectando Mutação em Operador Lógico

```csharp
// Código original
public bool ValidarUsuario(string nome, int idade, bool ativo)
{
    return !string.IsNullOrEmpty(nome) && idade >= 18 && ativo;
}

// Testes que DETECTAM mutações && → ||
[Theory]
[InlineData("João", 18, true, true)]    // Todos válidos
[InlineData("", 18, true, false)]       // Nome inválido
[InlineData("João", 17, true, false)]   // Idade inválida
[InlineData("João", 18, false, false)]  // Inativo
public void ValidarUsuario_DeveDetectarMutacoes(string nome, int idade, bool ativo, bool esperado)
{
    var resultado = ValidarUsuario(nome, idade, ativo);
    resultado.Should().Be(esperado);
}
```

## 📊 Comandos Úteis

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

## 🚀 Próximos Passos

1. **Configurar Stryker**: Setup inicial do projeto ✅
2. **Executar Baseline**: Primeira execução para estabelecer baseline
3. **Analisar Resultados**: Identificar gaps nos testes
4. **Melhorar Testes**: Adicionar testes para mutações sobreviventes
5. **Automatizar**: Integrar no pipeline de CI/CD
6. **Monitorar**: Acompanhar evolução do Mutation Score

## 📚 Documentação Adicional

- 📖 **[Estrategias-Mutacao.md](Estrategias-Mutacao.md)** - Guia completo de estratégias avançadas
- 🌐 **[Stryker.NET Docs](https://stryker-mutator.io/docs/stryker-net/introduction)** - Documentação oficial
- 💡 **[FluentAssertions](https://fluentassertions.com/)** - Biblioteca de assertions usada nos testes