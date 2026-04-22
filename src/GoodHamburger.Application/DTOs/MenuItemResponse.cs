namespace GoodHamburger.Application.DTOs;

public class MenuItemResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Type { get; init; } = string.Empty;
}
