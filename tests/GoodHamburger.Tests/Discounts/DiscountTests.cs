using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Discounts;

namespace GoodHamburger.Tests.Discounts;

public class DiscountCalculatorTests
{
    private readonly DiscountCalculator _sut = new();

    [Theory]
    [InlineData(true,  true,  20)]
    [InlineData(false, true,  15)]
    [InlineData(true,  false, 10)]
    [InlineData(false, false,  0)]
    public void Calculate_ReturnsCorrectPercentageForEachCombination(
        bool hasFries, bool hasDrink, decimal expectedPct)
    {
        Assert.Equal(expectedPct, _sut.Calculate(hasFries, hasDrink));
    }
}

public class FullComboDiscountTests
{
    private readonly IDiscountStrategy _sut = new FullComboDiscount();

    [Fact] public void Percentage_Is20() => Assert.Equal(20m, _sut.Percentage);

    [Theory]
    [InlineData(true,  true,  true)]
    [InlineData(true,  false, false)]
    [InlineData(false, true,  false)]
    [InlineData(false, false, false)]
    public void Applies_OnlyWhenBothFriesAndDrinkPresent(bool hasFries, bool hasDrink, bool expected)
        => Assert.Equal(expected, _sut.Applies(hasFries, hasDrink));
}

public class SandwichDrinkDiscountTests
{
    private readonly IDiscountStrategy _sut = new SandwichDrinkDiscount();

    [Fact] public void Percentage_Is15() => Assert.Equal(15m, _sut.Percentage);

    [Theory]
    [InlineData(false, true,  true)]
    [InlineData(true,  true,  false)]
    [InlineData(false, false, false)]
    [InlineData(true,  false, false)]
    public void Applies_OnlyWhenDrinkWithoutFries(bool hasFries, bool hasDrink, bool expected)
        => Assert.Equal(expected, _sut.Applies(hasFries, hasDrink));
}

public class SandwichFriesDiscountTests
{
    private readonly IDiscountStrategy _sut = new SandwichFriesDiscount();

    [Fact] public void Percentage_Is10() => Assert.Equal(10m, _sut.Percentage);

    [Theory]
    [InlineData(true,  false, true)]
    [InlineData(true,  true,  false)]
    [InlineData(false, false, false)]
    [InlineData(false, true,  false)]
    public void Applies_OnlyWhenFriesWithoutDrink(bool hasFries, bool hasDrink, bool expected)
        => Assert.Equal(expected, _sut.Applies(hasFries, hasDrink));
}

public class NoDiscountTests
{
    private readonly IDiscountStrategy _sut = new NoDiscount();

    [Fact] public void Percentage_Is0() => Assert.Equal(0m, _sut.Percentage);

    [Theory]
    [InlineData(true,  true)]
    [InlineData(true,  false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void Applies_Always(bool hasFries, bool hasDrink)
        => Assert.True(_sut.Applies(hasFries, hasDrink));
}
