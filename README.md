# Good Hamburger APP

API REST para registro de pedidos da lanchonete **Good Hamburger**, desenvolvida em C# com ASP.NET Core 9.

---

## Como executar localmente

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local ou Docker)
- `dotnet-ef` CLI: `dotnet tool install --global dotnet-ef`

### 1. Clone o repositório

```bash
git clone <url-do-repositorio>
cd GoodHamburguer
```

### 2. Configure a connection string

Edite `src/GoodHamburger.Api/appsettings.json` com as credenciais do seu SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=GoodHamburger;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Crie o banco de dados

```bash
dotnet ef database update \
  --project src/GoodHamburger.Infrastructure \
  --startup-project src/GoodHamburger.Api
```

### 4. Execute a API

```bash
dotnet run --project src/GoodHamburger.Api
```

A documentação Swagger estará disponível em `http://localhost:<porta>`.

### 5. Execute os testes

```bash
dotnet test tests/GoodHamburger.Tests/
```

---

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/menu` | Lista o cardápio completo |
| `GET` | `/orders` | Lista todos os pedidos |
| `GET` | `/orders/{id}` | Consulta pedido por ID |
| `POST` | `/orders` | Cria novo pedido |
| `PUT` | `/orders/{id}` | Atualiza pedido existente |
| `DELETE` | `/orders/{id}` | Remove pedido |

### Exemplo de criação de pedido

**Request:**
```json
POST /orders
{
  "sandwichId": 1,
  "friesId": 4,
  "drinkId": 5
}
```

**Response `201 Created`:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "sandwich": { "id": 1, "name": "X Burger", "price": 5.00 },
  "fries":    { "id": 4, "name": "Batata frita", "price": 2.00 },
  "drink":    { "id": 5, "name": "Refrigerante", "price": 2.50 },
  "subtotal": 9.50,
  "discountPercentage": 20,
  "discountAmount": 1.90,
  "total": 7.60,
  "createdAt": "2026-04-21T00:00:00Z"
}
```

### Cardápio

| ID | Item | Tipo | Preço |
|----|------|------|-------|
| 1 | X Burger | Sanduíche | R$ 5,00 |
| 2 | X Egg | Sanduíche | R$ 4,50 |
| 3 | X Bacon | Sanduíche | R$ 7,00 |
| 4 | Batata frita | Acompanhamento | R$ 2,00 |
| 5 | Refrigerante | Bebida | R$ 2,50 |

### Tabela de descontos

| Combinação | Desconto |
|------------|----------|
| Sanduíche + Batata + Refrigerante | 20% |
| Sanduíche + Refrigerante | 15% |
| Sanduíche + Batata | 10% |
| Apenas Sanduíche | 0% |

---

## Decisões de arquitetura

### Clean Architecture em camadas

O projeto segue separação em quatro camadas com dependências unidirecionais:

```
Api → Application → Domain
Infrastructure → Application → Domain
```

- **Domain** — entidades (`Order`, `MenuItem`) e enumerações. Sem dependências externas.
- **Application** — casos de uso (`OrderService`), DTOs, interfaces de repositório e validators. Desconhece infraestrutura.
- **Infrastructure** — implementações de `IOrderRepository` e `IMenuRepository`, `AppDbContext` (EF Core).
- **Api** — controllers, `Program.cs`, configuração do pipeline ASP.NET Core.

### Cardápio fixo via repositório estático

O cardápio é definido em `MenuData` (Application) e exposto via `IMenuRepository` / `StaticMenuRepository` (Infrastructure). A abstração permite substituir por dados vindos do banco futuramente sem alterar o `OrderService` ou os validators.

### Validação com FluentValidation

Toda validação de entrada está na camada Application (`Validators/`), fora dos controllers e do service. Um `AbstractValidator` base (`OrderRequestValidatorBase<T>`) centraliza as regras compartilhadas entre criação e atualização.

### Entidade com comportamento

A entidade `Order` possui setters privados e expõe o método `ApplyItems()` para mutações. Isso evita que o estado interno seja alterado de forma arbitrária fora da entidade.

### Persistência com EF Core

- SQL Server com migrations versionadas em `Infrastructure/Migrations/`
- Propriedades `decimal` configuradas explicitamente com precisão (`decimal(10,2)`)
- `AsNoTracking()` em consultas de leitura para melhor performance

---

## O que foi deixado de fora e por quê

| Item | Motivo |
|------|--------|
| **CRUD de itens do cardápio** | O cardápio é fixo conforme especificação. Não há necessidade de gerenciar itens via API. |
| **Autenticação/Autorização** | Fora do escopo do desafio técnico. |
| **Paginação em `GET /orders`** | Volume esperado de dados não justifica neste momento. |
| **Testes de integração** | Optou-se por testes unitários da camada de negócio, cobrindo os casos mais críticos. Testes de integração exigiriam banco de dados em memória ou contêiner. |
