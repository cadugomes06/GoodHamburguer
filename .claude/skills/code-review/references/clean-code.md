# Referência: Clean Code para C#

## Nomenclatura

### Regras
- **Classes**: substantivos no singular, PascalCase — `OrderService`, não `OrderSvc` ou `ManageOrders`
- **Métodos**: verbos ou frases verbais — `CalculateTotal()`, não `Total()` ou `Calc()`
- **Parâmetros**: nomes que revelam intenção — `Order order`, não `Order o` ou `Order obj`
- **Booleanos**: prefixo `is`, `has`, `can`, `should` — `isValid`, `hasDiscount`
- **Constantes**: PascalCase em C# (não ALL_CAPS) — `MaxRetryCount`
- **Generics**: evitar `T` solto; usar nomes descritivos — `TEntity`, `TResult`

### Antipadrões a detectar
```csharp
// ❌ Nome sem intenção
void Proc(Order o) { }

// ✅ Nome revela intenção
void ProcessOrder(Order order) { }

// ❌ Abreviação obscura
var ctxMgr = new CtxMgr();

// ✅ Nome completo
var contextManager = new ContextManager();
```

---

## Funções e Métodos

### Regras
- **Tamanho**: idealmente até 20 linhas; se passar de 30, é sinal de refactor
- **Um nível de abstração por função**: não misturar lógica de domínio com I/O
- **Máximo de parâmetros**: 3; se precisar de mais, encapsule em um objeto (Parameter Object)
- **Sem side effects ocultos**: o nome deve descrever completamente o que o método faz
- **Evitar flags booleanos como parâmetro** — indicam que o método faz duas coisas

```csharp
// ❌ Flag como parâmetro
void SaveOrder(Order order, bool sendEmail) { }

// ✅ Dois métodos com nomes claros
void SaveOrder(Order order) { }
void SaveOrderAndNotify(Order order) { }
```

---

## Comentários

### Regras
- Código bom não precisa de comentário explicando *o quê* — apenas *por quê* (decisão de negócio não óbvia)
- Comentários desatualizados são piores que ausência de comentários
- XML docs (`///`) em APIs públicas são bem-vindos; em código interno, redundantes

```csharp
// ❌ Comentário que repete o código
// Incrementa i
i++;

// ✅ Comentário que explica decisão não óbvia
// Tolerância de 5% definida em contrato com o fornecedor (ticket #1234)
const decimal PriceTolerance = 0.05m;
```

---

## Tratamento de Erros

### Regras
- Use exceções tipadas, nunca retorne códigos de erro numéricos
- Nunca capture `Exception` genérico sem re-throw ou log
- Prefira Result Pattern para erros esperados de negócio (evita exceções como controle de fluxo)
- Nunca deixe `catch` vazio

```csharp
// ❌ Catch vazio — engole erros
try { ... } catch (Exception) { }

// ❌ Exceção como controle de fluxo
try { return order.Items.First(); }
catch (InvalidOperationException) { return null; }

// ✅ Result Pattern para erro de domínio esperado
public Result<Order> GetOrder(Guid id)
{
    var order = _repository.Find(id);
    if (order is null)
        return Result.Failure<Order>("Order not found");
    return Result.Success(order);
}
```

---

## Complexidade Ciclomática

Alertar quando um método tem muitos `if/else`, `switch`, operadores ternários encadeados.

```csharp
// ❌ Alta complexidade — difícil de testar
public decimal Calculate(Order order)
{
    if (order.HasSandwich && order.HasFries && order.HasDrink)
        return order.Subtotal * 0.80m;
    else if (order.HasSandwich && order.HasDrink)
        return order.Subtotal * 0.85m;
    else if (order.HasSandwich && order.HasFries)
        return order.Subtotal * 0.90m;
    else
        return order.Subtotal;
}

// ✅ Extrair para Strategy ou tabela de regras
// Ver design-patterns.md → Strategy Pattern
```

---

## Lei de Demeter (Principle of Least Knowledge)

Não encadeie mais de um nível de acesso além do objeto imediato.

```csharp
// ❌ Viola Lei de Demeter
var city = order.Customer.Address.City.Name;

// ✅ Expor o que é necessário
var city = order.GetCustomerCity();
```

---

## Checklist Rápido de Clean Code

- [ ] Nomes revelam intenção sem necessidade de comentário?
- [ ] Métodos fazem uma única coisa?
- [ ] Parâmetros ≤ 3?
- [ ] Ausência de magic numbers/strings?
- [ ] Tratamento de erro adequado (sem catch vazio)?
- [ ] Complexidade ciclomática baixa (≤ 10 por método)?
- [ ] Ausência de código morto ou comentado?
- [ ] Ausência de duplicação (DRY)?
