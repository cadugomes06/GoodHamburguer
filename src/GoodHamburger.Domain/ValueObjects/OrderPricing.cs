namespace GoodHamburger.Domain.ValueObjects;

public record OrderPricing(
    decimal Subtotal,
    decimal DiscountPercentage,
    decimal DiscountAmount)
{
    public decimal Total => Subtotal - DiscountAmount;
}
