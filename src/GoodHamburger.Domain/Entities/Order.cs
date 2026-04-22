using GoodHamburger.Domain.ValueObjects;

namespace GoodHamburger.Domain.Entities;

public class Order
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int SandwichId { get; private set; }
    public int? FriesId { get; private set; }
    public int? DrinkId { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal Total { get; private set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public void ApplyItems(int sandwichId, int? friesId, int? drinkId, OrderPricing pricing)
    {
        SandwichId         = sandwichId;
        FriesId            = friesId;
        DrinkId            = drinkId;
        Subtotal           = pricing.Subtotal;
        DiscountPercentage = pricing.DiscountPercentage;
        DiscountAmount     = pricing.DiscountAmount;
        Total              = pricing.Total;
    }
}
