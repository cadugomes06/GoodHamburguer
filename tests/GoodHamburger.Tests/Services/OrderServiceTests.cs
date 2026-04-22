using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.ValueObjects;
using NSubstitute;

namespace GoodHamburger.Tests.Services;

public class OrderServiceTests
{
    private static readonly MenuItem XBurger = new() { Id = 1, Name = "X Burger",     Price = 5.00m, Type = ItemType.Sandwich };
    private static readonly MenuItem Fries   = new() { Id = 4, Name = "Batata frita", Price = 2.00m, Type = ItemType.Fries };
    private static readonly MenuItem Drink   = new() { Id = 5, Name = "Refrigerante", Price = 2.50m, Type = ItemType.Drink };

    private readonly IOrderRepository _repository;
    private readonly IMenuRepository  _menu;
    private readonly OrderService     _sut;

    public OrderServiceTests()
    {
        _repository = Substitute.For<IOrderRepository>();
        _menu       = Substitute.For<IMenuRepository>();

        _menu.FindById(XBurger.Id).Returns(XBurger);
        _menu.FindById(Fries.Id).Returns(Fries);
        _menu.FindById(Drink.Id).Returns(Drink);

        _repository.CreateAsync(Arg.Any<Order>()).Returns(ci => ci.Arg<Order>());
        _repository.UpdateAsync(Arg.Any<Order>()).Returns(ci => ci.Arg<Order>());

        _sut = new OrderService(_repository, _menu, new DiscountCalculator());
    }

    // ── Desconto ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1, 4, 5,    20)]
    [InlineData(1, null, 5, 15)]
    [InlineData(1, 4, null, 10)]
    [InlineData(1, null, null, 0)]
    public async Task CreateAsync_AppliesCorrectDiscountPercentage(
        int sandwichId, int? friesId, int? drinkId, decimal expectedDiscountPct)
    {
        var request = new CreateOrderRequest { SandwichId = sandwichId, FriesId = friesId, DrinkId = drinkId };

        var result = await _sut.CreateAsync(request);

        Assert.Equal(expectedDiscountPct, result.DiscountPercentage);
    }

    // ── Subtotal ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1, 4, 5,    9.50)]
    [InlineData(1, null, 5, 7.50)]
    [InlineData(1, 4, null, 7.00)]
    [InlineData(1, null, null, 5.00)]
    public async Task CreateAsync_CalculatesCorrectSubtotal(
        int sandwichId, int? friesId, int? drinkId, decimal expectedSubtotal)
    {
        var request = new CreateOrderRequest { SandwichId = sandwichId, FriesId = friesId, DrinkId = drinkId };

        var result = await _sut.CreateAsync(request);

        Assert.Equal(expectedSubtotal, result.Subtotal);
    }

    // ── Total ───────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1, 4, 5,    7.60)]
    [InlineData(1, null, 5, 6.38)] // 7.50 − 1.12 (banker's rounding: 1.125 → 1.12)
    [InlineData(1, 4, null, 6.30)]
    [InlineData(1, null, null, 5.00)]
    public async Task CreateAsync_CalculatesCorrectTotal(
        int sandwichId, int? friesId, int? drinkId, decimal expectedTotal)
    {
        var request = new CreateOrderRequest { SandwichId = sandwichId, FriesId = friesId, DrinkId = drinkId };

        var result = await _sut.CreateAsync(request);

        Assert.Equal(expectedTotal, result.Total);
    }

    // ── Valor do desconto ───────────────────────────────────────────────────

    [Theory]
    [InlineData(1, 4, 5,    1.90)]
    [InlineData(1, null, 5, 1.12)]
    [InlineData(1, 4, null, 0.70)]
    [InlineData(1, null, null, 0.00)]
    public async Task CreateAsync_CalculatesCorrectDiscountAmount(
        int sandwichId, int? friesId, int? drinkId, decimal expectedDiscountAmount)
    {
        var request = new CreateOrderRequest { SandwichId = sandwichId, FriesId = friesId, DrinkId = drinkId };

        var result = await _sut.CreateAsync(request);

        Assert.Equal(expectedDiscountAmount, result.DiscountAmount);
    }

    // ── Mapeamento da resposta ──────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_MapsCorrectSandwichDataToResponse()
    {
        var request = new CreateOrderRequest { SandwichId = 1 };

        var result = await _sut.CreateAsync(request);

        Assert.Equal(XBurger.Id,    result.Sandwich.Id);
        Assert.Equal(XBurger.Name,  result.Sandwich.Name);
        Assert.Equal(XBurger.Price, result.Sandwich.Price);
    }

    [Fact]
    public async Task CreateAsync_WithAllItems_MapsCorrectOptionalItemsToResponse()
    {
        var request = new CreateOrderRequest { SandwichId = 1, FriesId = 4, DrinkId = 5 };

        var result = await _sut.CreateAsync(request);

        Assert.NotNull(result.Fries);
        Assert.Equal(Fries.Name, result.Fries.Name);
        Assert.NotNull(result.Drink);
        Assert.Equal(Drink.Name, result.Drink.Name);
    }

    [Fact]
    public async Task CreateAsync_WithoutOptionalItems_ReturnsNullFriesAndDrink()
    {
        var request = new CreateOrderRequest { SandwichId = 1 };

        var result = await _sut.CreateAsync(request);

        Assert.Null(result.Fries);
        Assert.Null(result.Drink);
    }

    // ── GetAll ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllOrders()
    {
        var order1 = MakeOrder(1, null, null, new OrderPricing(5.00m, 0m, 0m));
        var order2 = MakeOrder(1, 4,    5,    new OrderPricing(9.50m, 20m, 1.90m));
        _repository.GetAllAsync().Returns(new List<Order> { order1, order2 });

        var result = await _sut.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    // ── GetById ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ReturnsMappedResponse()
    {
        var id    = Guid.NewGuid();
        var order = MakeOrder(1, null, null, new OrderPricing(5.00m, 0m, 0m), id);
        _repository.GetByIdAsync(id).Returns(order);

        var result = await _sut.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderNotFound_ReturnsNull()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>()).Returns((Order?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    // ── Update ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WhenOrderNotFound_ReturnsNull()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>()).Returns((Order?)null);

        var result = await _sut.UpdateAsync(Guid.NewGuid(), new UpdateOrderRequest { SandwichId = 1 });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderFound_ReturnsResponseWithNewValues()
    {
        var id       = Guid.NewGuid();
        var existing = MakeOrder(1, null, null, new OrderPricing(5.00m, 0m, 0m), id);
        _repository.GetByIdAsync(id).Returns(existing);

        var request = new UpdateOrderRequest { SandwichId = 1, FriesId = 4, DrinkId = 5 };
        var result  = await _sut.UpdateAsync(id, request);

        Assert.NotNull(result);
        Assert.Equal(20m,   result.DiscountPercentage);
        Assert.Equal(9.50m, result.Subtotal);
        Assert.Equal(7.60m, result.Total);
    }

    // ── Delete ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_ReturnsTrueAndCallsRepository()
    {
        var id    = Guid.NewGuid();
        var order = MakeOrder(1, null, null, new OrderPricing(5.00m, 0m, 0m), id);
        _repository.GetByIdAsync(id).Returns(order);

        var result = await _sut.DeleteAsync(id);

        Assert.True(result);
        await _repository.Received(1).DeleteAsync(order);
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderNotFound_ReturnsFalse()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>()).Returns((Order?)null);

        var result = await _sut.DeleteAsync(Guid.NewGuid());

        Assert.False(result);
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<Order>());
    }

    // ── Helper ──────────────────────────────────────────────────────────────

    private static Order MakeOrder(int sandwichId, int? friesId, int? drinkId,
        OrderPricing pricing, Guid? id = null)
    {
        var order = id.HasValue ? new Order { Id = id.Value } : new Order();
        order.ApplyItems(sandwichId, friesId, drinkId, pricing);
        return order;
    }
}
