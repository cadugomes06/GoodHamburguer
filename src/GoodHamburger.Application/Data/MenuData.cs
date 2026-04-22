using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Application.Data;

public static class MenuData
{
    public static readonly IReadOnlyList<MenuItem> Items = new List<MenuItem>
    {
        new() { Id = 1, Name = "X Burger", Price = 5.00m, Type = ItemType.Sandwich },
        new() { Id = 2, Name = "X Egg",    Price = 4.50m, Type = ItemType.Sandwich },
        new() { Id = 3, Name = "X Bacon",  Price = 7.00m, Type = ItemType.Sandwich },
        new() { Id = 4, Name = "Batata frita",  Price = 2.00m, Type = ItemType.Fries },
        new() { Id = 5, Name = "Refrigerante",  Price = 2.50m, Type = ItemType.Drink },
    };

    public static MenuItem? FindById(int id) => Items.FirstOrDefault(i => i.Id == id);
}
