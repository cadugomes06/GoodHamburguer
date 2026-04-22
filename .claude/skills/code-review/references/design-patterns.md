# Referência: Design Patterns em C#

Catálogo focado nos patterns mais relevantes para APIs .NET. Para cada pattern: quando detectar a necessidade, e implementação mínima em C#.

---

## Creational Patterns

### Factory Method
**Quando sugerir:** Criação de objeto depende de condição ou tipo; `new` espalhado pelo código.

```csharp
// ❌ Instanciação condicional espalhada
IDiscountStrategy strategy;
if (order.HasFries && order.HasDrink) strategy = new FullComboDiscount();
else if (order.HasDrink) strategy = new DrinkComboDiscount();
else strategy = new NoDiscount();

// ✅ Factory encapsula a decisão
public static class DiscountStrategyFactory
{
    public static IDiscountStrategy Create(Order order) => order switch
    {
        { HasFries: true, HasDrink: true } => new FullComboDiscount(),
        { HasDrink: true }                 => new DrinkComboDiscount(),
        { HasFries: true }                 => new FriesComboDiscount(),
        _                                  => new NoDiscount()
    };
}
```

---

### Builder
**Quando sugerir:** Objeto com muitos parâmetros opcionais; construtor com 4+ parâmetros.

```csharp
// ❌ Construtor com muitos parâmetros opcionais
var order = new Order(sandwichId, null, drinkId, customerId, null, true);

// ✅ Builder fluente
var order = new OrderBuilder()
    .WithSandwich(sandwichId)
    .WithDrink(drinkId)
    .ForCustomer(customerId)
    .Build();
```

---

## Structural Patterns

### Repository
**Quando sugerir:** Acesso a dados misturado com lógica de negócio; `DbContext` injetado em serviços de domínio.

```csharp
public interface IOrderRepository
{
    Task<Order?> FindAsync(Guid id, CancellationToken ct = default);
    Task SaveAsync(Order order, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> ListAllAsync(CancellationToken ct = default);
}

// Implementação em Infrastructure — o domínio não conhece EF Core
public class EfOrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    public EfOrderRepository(AppDbContext context) => _context = context;

    public async Task<Order?> FindAsync(Guid id, CancellationToken ct) =>
        await _context.Orders.FindAsync(new object[] { id }, ct);

    public async Task SaveAsync(Order order, CancellationToken ct)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(ct);
    }
    // ...
}
```

---

### Decorator
**Quando sugerir:** Comportamento transversal (logging, cache, validação) misturado com lógica central.

```csharp
// ✅ Decorador adiciona logging sem modificar OrderRepository
public class LoggingOrderRepository : IOrderRepository
{
    private readonly IOrderRepository _inner;
    private readonly ILogger<LoggingOrderRepository> _logger;

    public LoggingOrderRepository(IOrderRepository inner, ILogger<LoggingOrderRepository> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task SaveAsync(Order order, CancellationToken ct)
    {
        _logger.LogInformation("Saving order {OrderId}", order.Id);
        await _inner.SaveAsync(order, ct);
        _logger.LogInformation("Order {OrderId} saved", order.Id);
    }
    // demais métodos delegam para _inner
}
```

---

## Behavioral Patterns

### Strategy
**Quando sugerir:** `if/else` ou `switch` baseado em tipo que cresceria ao adicionar novos casos (viola OCP).

```csharp
public interface IDiscountStrategy
{
    bool Applies(Order order);
    decimal Apply(decimal subtotal);
}

public class FullComboDiscount : IDiscountStrategy
{
    public bool Applies(Order order) =>
        order.HasSandwich && order.HasFries && order.HasDrink;
    public decimal Apply(decimal subtotal) => subtotal * 0.80m;
}

public class DrinkComboDiscount : IDiscountStrategy
{
    public bool Applies(Order order) => order.HasSandwich && order.HasDrink;
    public decimal Apply(decimal subtotal) => subtotal * 0.85m;
}

// Calculadora usa a lista de strategies — não precisa mudar ao adicionar nova regra
public class DiscountCalculator
{
    private readonly IEnumerable<IDiscountStrategy> _strategies;

    public DiscountCalculator(IEnumerable<IDiscountStrategy> strategies)
        => _strategies = strategies;

    public decimal Calculate(Order order)
    {
        var strategy = _strategies.FirstOrDefault(s => s.Applies(order));
        return strategy?.Apply(order.Subtotal) ?? order.Subtotal;
    }
}
```

---

### Result Pattern (não GoF, mas essencial em .NET moderno)
**Quando sugerir:** Exceções usadas como controle de fluxo para erros de negócio esperados.

```csharp
public record Result<T>
{
    public T? Value { get; }
    public string? Error { get; }
    public bool IsSuccess => Error is null;

    private Result(T value) => Value = value;
    private Result(string error) => Error = error;

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error);
}

// Uso em serviço
public Result<Order> CreateOrder(CreateOrderRequest request)
{
    if (_repository.Exists(request.Id))
        return Result<Order>.Failure("Order already exists");

    var order = Order.Create(request);
    _repository.Save(order);
    return Result<Order>.Success(order);
}
```

---

### Mediator (via MediatR)
**Quando sugerir:** Controllers com muitas dependências injetadas; lógica de aplicação acoplada ao controller.

```csharp
// Command
public record CreateOrderCommand(Guid SandwichId, Guid? FriesId, Guid? DrinkId)
    : IRequest<Result<OrderResponse>>;

// Handler — único responsável pela operação
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
    private readonly IOrderRepository _repository;
    private readonly DiscountCalculator _calculator;

    public async Task<Result<OrderResponse>> Handle(
        CreateOrderCommand request, CancellationToken ct)
    {
        // lógica de criação
    }
}

// Controller limpo — apenas roteia
[HttpPost]
public async Task<IActionResult> Create(CreateOrderCommand command)
{
    var result = await _mediator.Send(command);
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
}
```

---

### Observer / Domain Events
**Quando sugerir:** Efeitos colaterais (envio de email, auditoria) dentro da lógica de negócio principal.

```csharp
// Evento de domínio
public record OrderCreatedEvent(Guid OrderId, DateTime CreatedAt);

// Entidade dispara evento
public class Order
{
    private readonly List<IDomainEvent> _events = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _events.AsReadOnly();

    public static Order Create(...)
    {
        var order = new Order(...);
        order._events.Add(new OrderCreatedEvent(order.Id, DateTime.UtcNow));
        return order;
    }
}

// Handler de evento cuida do side effect
public class SendOrderConfirmationHandler : INotificationHandler<OrderCreatedEvent>
{
    public Task Handle(OrderCreatedEvent notification, CancellationToken ct)
        => _emailService.SendConfirmationAsync(notification.OrderId, ct);
}
```

---

## Checklist de Patterns

| Sintoma no código | Pattern a sugerir |
|---|---|
| `new` de infraestrutura em domínio | DIP + Repository |
| `if/switch` crescente por tipo | Strategy + Factory |
| Construtor com 4+ parâmetros | Builder |
| Logging/cache misturado com lógica | Decorator |
| Controller com muitas dependências | Mediator |
| Side effects dentro do aggregate | Domain Events |
| Exceção para erro de negócio esperado | Result Pattern |
| Criação condicional de objeto | Factory Method |
