# Estratégias Avançadas de Teste de Mutação

## 🎯 Objetivo

Este documento apresenta estratégias específicas para criar testes que detectem mutações de forma eficaz, maximizando o **Mutation Score** e garantindo a qualidade dos testes.

## 🧬 Tipos de Mutações e Como Detectá-las

### 1. Mutações em Operadores Relacionais

#### Mutações Comuns:
```csharp
// Original → Mutações possíveis
>=  →  >, <, <=, ==, !=
<=  →  <, >, >=, ==, !=
>   →  >=, <, <=, ==, !=
<   →  <=, >, >=, ==, !=
==  →  !=, >, >=, <, <=
!=  →  ==, >, >=, <, <=
```

#### Estratégia de Teste:
```csharp
// Código a ser testado
public bool EhMaiorDeIdade(int idade)
{
    return idade >= 18;  // Mutação: >= → >
}

// Testes para detectar mutações
[Theory]
[InlineData(17, false)]  // Menor que 18
[InlineData(18, true)]   // Exatamente 18 - CRÍTICO para detectar >= → >
[InlineData(19, true)]   // Maior que 18
public void EhMaiorDeIdade_DeveDetectarMutacoesRelacionais(int idade, bool esperado)
{
    var resultado = EhMaiorDeIdade(idade);
    resultado.Should().Be(esperado);
}
```

**💡 Dica**: Sempre teste os **valores limítrofes** (boundary values) para detectar mutações em operadores relacionais.

### 2. Mutações em Operadores Aritméticos

#### Mutações Comuns:
```csharp
// Original → Mutações possíveis
+  →  -, *, /, %
-  →  +, *, /, %
*  →  +, -, /, %
/  →  +, -, *, %
%  →  +, -, *, /
```

#### Estratégia de Teste:
```csharp
// Código a ser testado
public double CalcularDesconto(double valor, double percentual)
{
    return valor - (valor * percentual / 100);  // Múltiplas mutações possíveis
}

// Testes para detectar mutações
[Theory]
[InlineData(100.0, 10.0, 90.0)]   // Caso padrão
[InlineData(100.0, 0.0, 100.0)]   // Zero desconto - detecta mutações
[InlineData(0.0, 10.0, 0.0)]      // Valor zero - detecta mutações
[InlineData(50.0, 20.0, 40.0)]    // Valores específicos
public void CalcularDesconto_DeveDetectarMutacoesAritmeticas(double valor, double perc, double esperado)
{
    var resultado = CalcularDesconto(valor, perc);
    resultado.Should().Be(esperado);
}
```

### 3. Mutações em Operadores Lógicos

#### Mutações Comuns:
```csharp
// Original → Mutações possíveis
&&  →  ||
||  →  &&
!   →  (removido)
```

#### Estratégia de Teste:
```csharp
// Código a ser testado
public bool ValidarUsuario(string nome, int idade, bool ativo)
{
    return !string.IsNullOrEmpty(nome) && idade >= 18 && ativo;
}

// Testes para detectar mutações lógicas
[Theory]
[InlineData("João", 18, true, true)]    // Todos válidos
[InlineData("", 18, true, false)]       // Nome inválido - detecta && → ||
[InlineData("João", 17, true, false)]   // Idade inválida - detecta && → ||
[InlineData("João", 18, false, false)]  // Inativo - detecta && → ||
[InlineData(null, 18, true, false)]     // Nome null - detecta !
public void ValidarUsuario_DeveDetectarMutacoesLogicas(string nome, int idade, bool ativo, bool esperado)
{
    var resultado = ValidarUsuario(nome, idade, ativo);
    resultado.Should().Be(esperado);
}
```

### 4. Mutações em Valores Constantes

#### Mutações Comuns:
```csharp
// Original → Mutações possíveis
0      →  1, -1
1      →  0, 2
true   →  false
false  →  true
""     →  "Stryker was here!"
null   →  new object()
```

#### Estratégia de Teste:
```csharp
// Código a ser testado
public string ClassificarNota(int nota)
{
    if (nota >= 7) return "Aprovado";     // Mutação: 7 → 6, 8
    if (nota >= 5) return "Recuperação";  // Mutação: 5 → 4, 6
    return "Reprovado";
}

// Testes para detectar mutações em constantes
[Theory]
[InlineData(7, "Aprovado")]        // Exato limite - detecta 7 → 6
[InlineData(6, "Recuperação")]     // Abaixo do limite - detecta 7 → 8
[InlineData(5, "Recuperação")]     // Exato limite - detecta 5 → 4
[InlineData(4, "Reprovado")]       // Abaixo do limite - detecta 5 → 6
public void ClassificarNota_DeveDetectarMutacoesEmConstantes(int nota, string esperado)
{
    var resultado = ClassificarNota(nota);
    resultado.Should().Be(esperado);
}
```

### 5. Mutações em Métodos de String

#### Mutações Comuns:
```csharp
// Original → Mutações possíveis
IsNullOrEmpty    →  IsNullOrWhiteSpace, !IsNullOrEmpty
Contains         →  !Contains, StartsWith, EndsWith
StartsWith       →  EndsWith, Contains, !StartsWith
ToUpper          →  ToLower
Trim             →  TrimStart, TrimEnd
```

#### Estratégia de Teste:
```csharp
// Código a ser testado
public bool ValidarEmail(string email)
{
    return !string.IsNullOrEmpty(email) && email.Contains("@");
}

// Testes para detectar mutações em strings
[Theory]
[InlineData("user@domain.com", true)]   // Email válido
[InlineData("", false)]                 // String vazia - detecta IsNullOrEmpty
[InlineData(null, false)]               // Null - detecta IsNullOrEmpty
[InlineData("user.domain.com", false)]  // Sem @ - detecta Contains
[InlineData("@", true)]                 // Só @ - detecta !Contains
public void ValidarEmail_DeveDetectarMutacoesEmStrings(string email, bool esperado)
{
    var resultado = ValidarEmail(email);
    resultado.Should().Be(esperado);
}
```

## 🎯 Estratégias por Tipo de Código

### 1. Validações de Entrada

```csharp
// Código típico de validação
public bool ValidarIdade(int idade)
{
    return idade >= 0 && idade <= 120;
}

// Estratégia: Testar todos os limites
[Theory]
[InlineData(-1, false)]   // Abaixo do mínimo
[InlineData(0, true)]     // Exato mínimo
[InlineData(1, true)]     // Acima do mínimo
[InlineData(119, true)]   // Abaixo do máximo
[InlineData(120, true)]   // Exato máximo
[InlineData(121, false)]  // Acima do máximo
```

### 2. Cálculos Matemáticos

```csharp
// Código de cálculo
public double CalcularJuros(double principal, double taxa, int meses)
{
    return principal * Math.Pow(1 + taxa / 100, meses) - principal;
}

// Estratégia: Casos conhecidos e extremos
[Theory]
[InlineData(1000, 1, 12, 126.83)]  // Caso padrão calculado
[InlineData(1000, 0, 12, 0)]       // Taxa zero
[InlineData(0, 1, 12, 0)]          // Principal zero
[InlineData(1000, 1, 0, 0)]        // Meses zero
```

### 3. Lógica de Negócio Complexa

```csharp
// Código com múltiplas condições
public string CalcularFrete(double peso, string destino, bool expresso)
{
    double valor = peso * 2.5;
    
    if (destino == "SP" || destino == "RJ")
        valor *= 0.9;  // 10% desconto
    
    if (expresso)
        valor *= 1.5;  // 50% adicional
    
    return valor < 10 ? 10 : valor;  // Valor mínimo
}

// Estratégia: Matriz de combinações
[Theory]
[InlineData(2, "SP", false, 4.5)]    // Peso baixo, SP, normal
[InlineData(2, "SP", true, 6.75)]    // Peso baixo, SP, expresso
[InlineData(2, "MG", false, 10)]     // Peso baixo, outros, normal (mínimo)
[InlineData(10, "RJ", true, 33.75)]  // Peso alto, RJ, expresso
```

## 🔍 Técnicas Avançadas

### 1. Teste de Mutações em Loops

```csharp
// Código com loop
public int ContarPares(int[] numeros)
{
    int count = 0;
    for (int i = 0; i < numeros.Length; i++)  // Mutações: <, <=, ++, --
    {
        if (numeros[i] % 2 == 0)              // Mutações: ==, !=, %
            count++;                          // Mutações: ++, +=
    }
    return count;
}

// Estratégia: Arrays específicos
[Theory]
[InlineData(new int[] { }, 0)]              // Array vazio
[InlineData(new int[] { 1 }, 0)]            // Um ímpar
[InlineData(new int[] { 2 }, 1)]            // Um par
[InlineData(new int[] { 1, 2, 3, 4 }, 2)]  // Misturado
[InlineData(new int[] { 2, 4, 6 }, 3)]     // Todos pares
```

### 2. Teste de Mutações em Exceções

```csharp
// Código que lança exceções
public double Dividir(double a, double b)
{
    if (b == 0)  // Mutação: == → !=
        throw new DivideByZeroException("Divisão por zero");
    
    return a / b;  // Mutação: / → *
}

// Estratégia: Testar exceção e casos normais
[Theory]
[InlineData(10, 2, 5)]      // Divisão normal
[InlineData(0, 5, 0)]       // Zero no numerador
[InlineData(-10, 2, -5)]    // Números negativos

[Fact]
public void Dividir_DeveDetectarMutacaoEmExcecao()
{
    var exception = Assert.Throws<DivideByZeroException>(() => Dividir(10, 0));
    exception.Message.Should().Contain("zero");
}
```

### 3. Teste de Mutações em Coleções

```csharp
// Código com LINQ
public List<string> FiltrarAtivos(List<Usuario> usuarios)
{
    return usuarios
        .Where(u => u.Ativo)           // Mutação: u.Ativo → !u.Ativo
        .Select(u => u.Nome)           // Mutação: Nome → Email
        .OrderBy(nome => nome)         // Mutação: OrderBy → OrderByDescending
        .ToList();
}

// Estratégia: Dados controlados
[Fact]
public void FiltrarAtivos_DeveDetectarMutacoesEmLinq()
{
    var usuarios = new List<Usuario>
    {
        new() { Nome = "Ana", Ativo = true },
        new() { Nome = "Bruno", Ativo = false },
        new() { Nome = "Carlos", Ativo = true }
    };
    
    var resultado = FiltrarAtivos(usuarios);
    
    resultado.Should().HaveCount(2);
    resultado.Should().Equal("Ana", "Carlos");  // Ordem específica
    resultado.Should().NotContain("Bruno");     // Inativo excluído
}
```

## 📊 Métricas e Análise

### 1. Interpretando Mutation Score

| Score | Qualidade | Ação Recomendada |
|-------|-----------|------------------|
| 90-100% | 🏆 Excelente | Manter qualidade |
| 80-89% | 👍 Boa | Pequenos ajustes |
| 70-79% | ⚠️ Moderada | Melhorias necessárias |
| 60-69% | 😟 Baixa | Revisão significativa |
| < 60% | ❌ Crítica | Reescrita dos testes |

### 2. Priorizando Mutações Sobreviventes

#### Alta Prioridade:
- Mutações em validações críticas
- Operadores relacionais em condições de segurança
- Cálculos financeiros ou de negócio

#### Média Prioridade:
- Mutações em formatação de dados
- Operadores lógicos em validações secundárias
- Constantes de configuração

#### Baixa Prioridade:
- Mutações em logs ou debug
- Strings de mensagem
- Métodos ToString/GetHashCode

## 🛠️ Ferramentas e Configuração

### 1. Configuração Otimizada do Stryker

```json
{
  "stryker-config": {
    "mutation-level": "Complete",
    "thresholds": {
      "high": 90,
      "low": 70,
      "break": 60
    },
    "ignore-methods": [
      "*ToString*",
      "*GetHashCode*",
      "*Equals*"
    ],
    "ignore-mutations": [
      "StringLiteral[Empty]"
    ],
    "timeout-ms": 15000,
    "concurrency": 4
  }
}
```

### 2. Integração com CI/CD

```yaml
# GitHub Actions
- name: Run Mutation Tests
  run: dotnet stryker --reporter json --break-at 70
  
- name: Fail if Low Quality
  run: |
    SCORE=$(jq '.thresholds.high' mutation-report.json)
    if [ "$SCORE" -lt 80 ]; then
      echo "Mutation score too low: $SCORE%"
      exit 1
    fi
```

## 🎓 Boas Práticas

### 1. Desenvolvimento Orientado por Mutação (MDT)

1. **Escreva o código**
2. **Execute testes de mutação**
3. **Identifique mutações sobreviventes**
4. **Adicione testes específicos**
5. **Repita até atingir score desejado**

### 2. Padrões de Teste Eficazes

#### ✅ Faça:
- Teste valores limítrofes
- Use Theory com múltiplos casos
- Teste condições negativas
- Verifique exceções específicas
- Teste casos extremos (null, zero, vazio)

#### ❌ Evite:
- Testes genéricos demais
- Ignorar valores limítrofes
- Testar apenas casos de sucesso
- Usar valores "mágicos" sem significado
- Testes que dependem de estado externo

### 3. Estratégias por Domínio

#### Sistemas Financeiros:
- Foque em precisão de cálculos
- Teste arredondamentos
- Valide limites de transação

#### Sistemas de Saúde:
- Priorize validações de segurança
- Teste cálculos de dosagem
- Valide datas críticas

#### E-commerce:
- Teste cálculos de preço
- Valide regras de desconto
- Verifique lógica de estoque

## 🚀 Próximos Passos

1. **Implementar testes básicos** com cobertura > 80%
2. **Executar primeira análise** de mutação
3. **Priorizar mutações sobreviventes** por criticidade
4. **Adicionar testes específicos** para gaps identificados
5. **Automatizar no pipeline** de CI/CD
6. **Monitorar evolução** do Mutation Score
7. **Treinar equipe** em técnicas avançadas

## 📚 Recursos Adicionais

- [Stryker.NET Documentation](https://stryker-mutator.io/docs/stryker-net/introduction)
- [Mutation Testing Patterns](https://github.com/stryker-mutator/stryker-net/blob/master/docs/Mutators.md)
- [FluentAssertions Guide](https://fluentassertions.com/introduction)
- [xUnit Best Practices](https://xunit.net/docs/getting-started/netcore/cmdline)

---

**💡 Lembre-se**: O objetivo não é apenas atingir 100% de Mutation Score, mas sim garantir que os testes sejam **robustos** e **confiáveis** para detectar bugs reais no código de produção.