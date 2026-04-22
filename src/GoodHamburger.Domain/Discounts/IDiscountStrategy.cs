namespace GoodHamburger.Domain.Discounts;

public interface IDiscountStrategy
{
    bool Applies(bool hasFries, bool hasDrink);
    decimal Percentage { get; }
}
