namespace GoodHamburger.Domain.Discounts;

public sealed class SandwichDrinkDiscount : IDiscountStrategy
{
    public bool Applies(bool hasFries, bool hasDrink) => !hasFries && hasDrink;
    public decimal Percentage => 15m;
}
