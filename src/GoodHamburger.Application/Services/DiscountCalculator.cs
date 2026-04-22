using GoodHamburger.Domain.Discounts;

namespace GoodHamburger.Application.Services;

public class DiscountCalculator
{
    private static readonly IReadOnlyList<IDiscountStrategy> Strategies =
    [
        new FullComboDiscount(),
        new SandwichDrinkDiscount(),
        new SandwichFriesDiscount(),
        new NoDiscount()
    ];

    public decimal Calculate(bool hasFries, bool hasDrink) =>
        Strategies.First(s => s.Applies(hasFries, hasDrink)).Percentage;
}
