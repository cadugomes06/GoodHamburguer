namespace GoodHamburger.Web.Models;

public class OrderResponse
{
    public Guid Id { get; set; }
    public MenuItemModel Sandwich { get; set; } = null!;
    public MenuItemModel? Fries { get; set; }
    public MenuItemModel? Drink { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateOrderRequest
{
    public int SandwichId { get; set; }
    public int? FriesId { get; set; }
    public int? DrinkId { get; set; }
}
