namespace GoodHamburger.Application.DTOs;

public class OrderResponse
{
    public Guid Id { get; init; }
    public MenuItemResponse Sandwich { get; init; } = null!;
    public MenuItemResponse? Fries { get; init; }
    public MenuItemResponse? Drink { get; init; }
    public decimal Subtotal { get; init; }
    public decimal DiscountPercentage { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal Total { get; init; }
    public DateTime CreatedAt { get; init; }
}
