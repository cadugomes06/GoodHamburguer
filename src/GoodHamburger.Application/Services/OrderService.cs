using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.ValueObjects;

namespace GoodHamburger.Application.Services;

public class OrderService(
    IOrderRepository repository,
    IMenuRepository menu,
    DiscountCalculator discountCalculator) : IOrderService
{
    public async Task<IEnumerable<OrderResponse>> GetAllAsync()
    {
        var orders = await repository.GetAllAsync();
        return orders.Select(MapToResponse);
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id)
    {
        var order = await repository.GetByIdAsync(id);
        return order is null ? null : MapToResponse(order);
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
    {
        var order = new Order();
        SetOrderItems(order, request.SandwichId, request.FriesId, request.DrinkId);
        var created = await repository.CreateAsync(order);
        return MapToResponse(created);
    }

    public async Task<OrderResponse?> UpdateAsync(Guid id, UpdateOrderRequest request)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null) return null;

        SetOrderItems(existing, request.SandwichId, request.FriesId, request.DrinkId);
        var saved = await repository.UpdateAsync(existing);
        return MapToResponse(saved);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var order = await repository.GetByIdAsync(id);
        if (order is null) return false;
        await repository.DeleteAsync(order);
        return true;
    }

    private void SetOrderItems(Order order, int sandwichId, int? friesId, int? drinkId)
    {
        var sandwich = menu.FindById(sandwichId)!;
        var fries    = friesId.HasValue ? menu.FindById(friesId.Value) : null;
        var drink    = drinkId.HasValue ? menu.FindById(drinkId.Value) : null;

        var pricing = CalculatePricing(sandwich.Price, fries?.Price, drink?.Price);
        order.ApplyItems(sandwichId, friesId, drinkId, pricing);
    }

    private OrderPricing CalculatePricing(decimal sandwichPrice, decimal? friesPrice, decimal? drinkPrice)
    {
        var subtotal      = sandwichPrice + (friesPrice ?? 0) + (drinkPrice ?? 0);
        var discountPct   = discountCalculator.Calculate(friesPrice.HasValue, drinkPrice.HasValue);
        var discountAmount = Math.Round(subtotal * discountPct / 100, 2);
        return new OrderPricing(subtotal, discountPct, discountAmount);
    }

    private OrderResponse MapToResponse(Order order)
    {
        var sandwich = menu.FindById(order.SandwichId)!;
        var fries    = order.FriesId.HasValue ? menu.FindById(order.FriesId.Value) : null;
        var drink    = order.DrinkId.HasValue ? menu.FindById(order.DrinkId.Value) : null;

        return new OrderResponse
        {
            Id                 = order.Id,
            Sandwich           = ToItemResponse(sandwich),
            Fries              = fries is null ? null : ToItemResponse(fries),
            Drink              = drink is null ? null : ToItemResponse(drink),
            Subtotal           = order.Subtotal,
            DiscountPercentage = order.DiscountPercentage,
            DiscountAmount     = order.DiscountAmount,
            Total              = order.Total,
            CreatedAt          = order.CreatedAt
        };
    }

    private static MenuItemResponse ToItemResponse(MenuItem item) =>
        new() { Id = item.Id, Name = item.Name, Price = item.Price };
}
