namespace GoodHamburger.Application.DTOs;

public interface IOrderRequest
{
    int SandwichId { get; }
    int? FriesId { get; }
    int? DrinkId { get; }
}
