namespace GoodHamburger.Application.DTOs;

public class CreateOrderRequest : IOrderRequest
{
    public int SandwichId { get; set; }
    public int? FriesId { get; set; }
    public int? DrinkId { get; set; }
}
