namespace GoodHamburger.Domain.Discounts;

public sealed class FullComboDiscount : IDiscountStrategy
{
    public bool Applies(bool hasFries, bool hasDrink) => hasFries && hasDrink;
    public decimal Percentage => 20m;
}
