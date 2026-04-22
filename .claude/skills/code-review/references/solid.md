# Referência: Princípios SOLID em C#

---

## S — Single Responsibility Principle (SRP)

**Definição:** Uma classe deve ter apenas um motivo para mudar.

### Como detectar violação
- Classe com mais de uma "família" de métodos (ex: lógica + persistência + notificação)
- Nome da classe usa "And", "Manager", "Helper", "Util" — geralmente sinal de responsabilidade múltipla
- Construtor injeta dependências de domínios muito diferentes

### Exemplo em C#
```csharp
// ❌ Viola SRP — OrderService faz persistência E envio de email
public class OrderService
{
    public void CreateOrder(Order order)
    {
        _dbContext.Orders.Add(order);
        _dbContext.SaveChanges();

        var message = new MailMessage("from@x.com", order.Customer.Email);
        new SmtpClient().Send(message);
    }
}

// ✅ Responsabilidades separadas
public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IOrderNotifier _notifier;

    public void CreateOrder(Order order)
    {
        _repository.Save(order);
        _notifier.NotifyCreated(order);
    }
}
```

---

## O — Open/Closed Principle (OCP)

**Definição:** Entidades devem estar abertas para extensão, mas fechadas para modificação.

### Como detectar violação
- `if/else` ou `switch` baseado em tipo/enum que cresceria com novos casos
- Adicionar um novo tipo de item requer alterar múltiplos métodos existentes

### Exemplo em C#
```csharp
// ❌ Viola OCP — adicionar novo desconto requer modificar este método
public decimal CalculateDiscount(Order order, string type)
{
    if (type == "sandwich+fries+drink") return order.Subtotal * 0.20m;
    if (type == "sandwich+drink") return order.Subtotal * 0.15m;
    return order.Subtotal;
}

// ✅ Extensível via Strategy
public interface IDiscountStrategy
{
    bool Applies(Order order);
    decimal Calculate(decimal subtotal);
}

public class FullComboDiscount : IDiscountStrategy
{
    public bool Applies(Order order) =>
        order.HasSandwich && order.HasFries && order.HasDrink;

    public decimal Calculate(decimal subtotal) => subtotal * 0.80m;
}
```

---

## L — Liskov Substitution Principle (LSP)

**Definição:** Subtipos devem ser substituíveis por seus tipos base sem alterar a correção do programa.

### Como detectar violação
- Override de método que lança `NotImplementedException`
- Subclasse que enfraquece pré-condições ou fortalece pós-condições da base
- Casting de tipo base para derivado dentro da lógica (`is`, `as` em fluxo de controle)

### Exemplo em C#
```csharp
// ❌ Viola LSP — ReadOnlyRepository não pode cumprir o contrato de IRepository
public class ReadOnlyRepository : IRepository
{
    public void Save(Order order) =>
        throw new NotImplementedException(); // quebra o contrato
}

// ✅ Segregar a interface (ver ISP abaixo)
public interface IReadRepository { Order Find(Guid id); }
public interface IWriteRepository { void Save(Order order); }
```

---

## I — Interface Segregation Principle (ISP)

**Definição:** Nenhum cliente deve depender de métodos que não usa.

### Como detectar violação
- Interface "faz tudo" com 8+ métodos
- Implementações que deixam métodos da interface com corpo vazio ou lançando exceção

### Exemplo em C#
```csharp
// ❌ Viola ISP — nem todo repositório precisa de BulkInsert
public interface IOrderRepository
{
    Order Find(Guid id);
    void Save(Order order);
    void Delete(Guid id);
    void BulkInsert(IEnumerable<Order> orders);
    IEnumerable<Order> GetReport(DateRange range);
}

// ✅ Interfaces coesas e focadas
public interface IOrderReader { Order Find(Guid id); }
public interface IOrderWriter { void Save(Order order); void Delete(Guid id); }
public interface IOrderBulkWriter { void BulkInsert(IEnumerable<Order> orders); }
public interface IOrderReporter { IEnumerable<Order> GetReport(DateRange range); }
```

---

## D — Dependency Inversion Principle (DIP)

**Definição:** Módulos de alto nível não devem depender de módulos de baixo nível. Ambos devem depender de abstrações.

### Como detectar violação
- `new` de infraestrutura dentro de serviço de domínio (ex: `new SqlConnection`, `new HttpClient`, `new SmtpClient`)
- Dependência de classe concreta no construtor em vez de interface
- `static` calls para infraestrutura (ex: `DateTime.Now` hardcoded, `File.ReadAllText` direto)

### Exemplo em C#
```csharp
// ❌ Viola DIP — alto acoplamento com infraestrutura concreta
public class OrderService
{
    public void Save(Order order)
    {
        var conn = new SqlConnection("Server=...");
        // ...
    }
}

// ✅ Depende de abstração, injetada via construtor
public class OrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public void Save(Order order) => _repository.Save(order);
}
```

---

## Checklist Rápido de SOLID

- [ ] **S**: Cada classe tem um único motivo para mudar?
- [ ] **O**: Adicionar comportamento requer nova classe, não modificar existente?
- [ ] **L**: Subclasses honram o contrato da classe base completamente?
- [ ] **I**: Interfaces são focadas? Implementações não têm métodos vazios?
- [ ] **D**: Dependências são sempre injetadas como abstrações (interfaces)?
