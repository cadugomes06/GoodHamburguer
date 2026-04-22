namespace GoodHamburger.Domain.Discounts;

public sealed class NoDiscount : IDiscountStrategy
{
    public bool Applies(bool hasFries, bool hasDrink) => true;
    public decimal Percentage => 0m;
}
