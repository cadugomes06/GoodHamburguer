namespace GoodHamburger.Domain.Discounts;

public sealed class SandwichFriesDiscount : IDiscountStrategy
{
    public bool Applies(bool hasFries, bool hasDrink) => hasFries && !hasDrink;
    public decimal Percentage => 10m;
}
