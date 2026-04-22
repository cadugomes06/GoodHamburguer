---
name: code-review
description: >
  Revisão técnica aprofundada de código C#/.NET aplicando Clean Code, princípios SOLID e Design Patterns.
  Use esta skill sempre que o usuário pedir para revisar, analisar, auditar ou avaliar código — mesmo que
  use palavras informais como "olha esse código", "o que acha disso?", "tem algo errado aqui?",
  "pode melhorar?", ou enviar um trecho de código sem instrução explícita. Também acione quando o usuário
  perguntar sobre qualidade, legibilidade, manutenibilidade, acoplamento, coesão ou boas práticas em C#.
  A skill gera um relatório estruturado com severidade por problema, exemplos de código corrigido
  e uma nota de qualidade geral (0–10).
---

# Code Review Skill — C#/.NET

## Objetivo

Realizar revisões de código técnicas, didáticas e acionáveis, focadas em três pilares:

1. **Clean Code** — clareza, nomenclatura, funções pequenas, ausência de ruído
2. **Princípios SOLID** — responsabilidade única, aberto/fechado, substituição, segregação, inversão
3. **Design Patterns** — identificar ausência, uso incorreto ou oportunidade de aplicação

---

## Workflow de Revisão

Execute sempre nesta ordem:

### 1. Leitura e Mapeamento

Antes de qualquer análise, leia o código por inteiro e identifique:
- Linguagem e versão do .NET (inferir pelo syntax)
- Contexto do código (domínio, camada: API, Application, Domain, Infrastructure)
- Intenção aparente do código

### 2. Análise por Pilar

Execute a análise nas três dimensões. Consulte os arquivos de referência para critérios detalhados:
- Critérios de Clean Code → `references/clean-code.md`
- Critérios SOLID → `references/solid.md`
- Catálogo de Patterns → `references/design-patterns.md`

### 3. Classificação de Severidade

Para cada problema encontrado, classifique:

| Severidade | Símbolo | Critério |
|---|---|---|
| Crítico | 🔴 | Quebra de contrato, bug latente, acoplamento severo |
| Alto | 🟠 | Viola SOLID diretamente, dificulta testes, alta complexidade ciclomática |
| Médio | 🟡 | Viola Clean Code, nomenclatura ruim, método longo |
| Baixo | 🔵 | Sugestão de pattern, refactor cosmético, convenção |
| Positivo | ✅ | Boa prática que merece ser destacada |

### 4. Geração do Relatório

Use SEMPRE este template:

---

## 📋 Relatório de Code Review

### Contexto
> [Breve descrição do que o código faz e em que camada está]

### Nota de Qualidade: `X/10`
> [Justificativa de 1-2 linhas]

---

### Problemas Encontrados

#### [SEVERIDADE] [PILAR] — [Título curto do problema]

**Localização:** `NomeClasse.NomeMetodo()` ou linha aproximada

**Problema:**
[Explicação clara do problema e por que viola o princípio]

**Código atual:**
```csharp
// trecho problemático
```

**Código sugerido:**
```csharp
// trecho corrigido com comentário explicativo
```

> 💡 **Referência:** [Nome do princípio ou pattern aplicado]

---

[Repita para cada problema, ordenado por severidade decrescente]

### ✅ Pontos Positivos
- [Liste o que está bem feito]

### 🗺️ Resumo de Ações
| Prioridade | Ação | Esforço |
|---|---|---|
| 1 | [Ação mais urgente] | Baixo/Médio/Alto |
| 2 | ... | ... |

---

## Regras de Comportamento

- **Sempre gere código corrigido** — não apenas aponte o problema, mostre a solução
- **Seja preciso na localização** — mencione o método ou classe afetado
- **Priorize por impacto** — problemas de design antes de estilo
- **Reconheça o bom** — sempre liste pelo menos um ponto positivo, se houver
- **Seja didático** — explique o *porquê* de cada violação, não apenas o *o quê*
- **Respeite o contexto** — não sugira over-engineering para código simples
- Se o trecho for muito curto para revisão completa, pergunte se o usuário quer enviar mais contexto antes de concluir

## Exemplo de Acionamento

**Input do usuário:**
> "Olha esse código aqui"
> ```csharp
> public class OrderService {
>     public void Process(Order o) {
>         var db = new SqlConnection("...");
>         // lógica misturada com persistência
>     }
> }
> ```

**A skill deve**: identificar SRP violado (lógica + persistência), DIP violado (instância direta de SqlConnection), nomeclatura ruim (`o`), e sugerir injeção de dependência + repositório.