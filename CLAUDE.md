# CLAUDE.md — Good Hamburger Order System

> Arquivo de contexto para o Claude Code. Leia este arquivo antes de qualquer tarefa no projeto.

---

## Visão Geral do Projeto

Sistema de registro de pedidos para a lanchonete **"Good Hamburger"**, implementado como uma **API REST em C# com .NET / ASP.NET Core**. O foco é demonstrar organização de código, modelagem de domínio e boas decisões técnicas.

---

## Cardápio

### Sanduíches

| Item     | Preço   |
|----------|---------|
| X Burger | R$ 5,00 |
| X Egg    | R$ 4,50 |
| X Bacon  | R$ 7,00 |

### Acompanhamentos

| Item          | Preço   |
|---------------|---------|
| Batata frita  | R$ 2,00 |
| Refrigerante  | R$ 2,50 |

---

## Regras de Negócio

### Composição do Pedido

- Cada pedido pode conter **no máximo**:
  - 1 sanduíche
  - 1 batata frita
  - 1 refrigerante
- Itens duplicados devem ser **rejeitados com mensagem de erro clara**.

### Regras de Desconto

| Combinação                            | Desconto |
|---------------------------------------|----------|
| Sanduíche + Batata + Refrigerante     | 20%      |
| Sanduíche + Refrigerante              | 15%      |
| Sanduíche + Batata                    | 10%      |
| Apenas Sanduíche (sem acompanhamento) | 0%       |

### Cálculo de Valores

Para cada pedido, calcular e retornar:
- **Subtotal**: soma dos preços dos itens (sem desconto)
- **Desconto aplicado**: valor em reais do desconto
- **Total final**: subtotal - desconto

---

## Requisitos Funcionais

### CRUD de Pedidos

| Operação  | Descrição                              |
|-----------|----------------------------------------|
| `POST`    | Criar novo pedido                      |
| `GET`     | Listar todos os pedidos                |
| `GET /{id}` | Consultar pedido por identificador   |
| `PUT /{id}` | Atualizar pedido existente           |
| `DELETE /{id}` | Remover pedido                    |

### Endpoints Adicionais

- `GET /menu` (ou similar) — Retornar o cardápio completo com itens e preços.

### Validações Obrigatórias

- Itens duplicados no mesmo pedido → erro 400 com mensagem clara
- Pedido inválido (sem sanduíche, item inexistente etc.) → erro 400
- Recurso não encontrado → erro 404

---

## Diferenciais Opcionais

- **Frontend em Blazor** consumindo a API
- **Testes automatizados** das regras de negócio (xUnit ou NUnit recomendado)

---

## Stack e Decisões de Arquitetura

### Obrigatório
- **Linguagem**: C#
- **Framework**: .NET / ASP.NET Core (versão mais recente estável)
- **Tipo**: API REST

### Recomendações de Arquitetura
- Seguir separação em camadas: `Api`, `Application`, `Domain`, `Infrastructure`
- Usar **Clean Architecture** ou **arquitetura em camadas simples** — documentar a escolha no README
- Persistência: SQLSERVER
- Usar **DTOs** para request/response — não expor entidades de domínio diretamente
- Aplicar validações com **FluentValidation** ou Data Annotations
- Documentar a API com **Swagger/OpenAPI** (`Swashbuckle`)

### Estrutura de Projeto Sugerida
```
GoodHamburger/
├── src/
│   ├── GoodHamburger.Api/           # Controllers, Program.cs, middlewares
│   ├── GoodHamburger.Application/   # Use cases, DTOs, interfaces de serviço
│   ├── GoodHamburger.Domain/        # Entidades, enums, regras de negócio puras
│   └── GoodHamburger.Infrastructure/# Repositórios, persistência
└── tests/
    └── GoodHamburger.Tests/         # Testes unitários das regras de negócio
```

---

## Entregáveis

- [ ] Código-fonte em **repositório Git público**
- [ ] **README.md** contendo:
  - Instruções de execução (como rodar localmente)
  - Decisões de arquitetura tomadas
  - O que foi deixado de fora e por quê
- [ ] Prazo: **7 dias corridos** (avisar caso precise de mais tempo)

---

## Exemplos de Payload

### Criar Pedido — `POST /orders`

```json
{
  "sanduicheId": 1,
  "acompanhamento": [1, 2]
}
```

### Resposta Esperada

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "sanduiche": { "nome": "X Burger", "preco": 5.00 },
  "acompanhamento": [
    { "nome": "Batata frita", "preco": 2.00 },
    { "nome": "Refrigerante", "preco": 2.50 }
  ],
  "subtotal": 9.50,
  "porcentagemDesconto": 20,
  "totalDisconto": 1.90,
  "total": 7.60
}
```

### Erro — Item duplicado

```json
{
  "error": "Duplicate item",
  "message": "Each order can only contain one sandwich, one fries, and one drink."
}
```

---

## Notas para o Claude Code

- Priorizar **clareza e organização** do código acima de otimizações prematuras.
- As regras de desconto são **mutuamente exclusivas** — aplicar sempre a maior combinação possível.
- O cardápio é **fixo** (não precisa de CRUD de itens), pode ser configurado via seed ou constante.
- Usar `Guid` como identificador de pedidos.
- Retornar sempre `ProblemDetails` padrão do ASP.NET Core para erros.