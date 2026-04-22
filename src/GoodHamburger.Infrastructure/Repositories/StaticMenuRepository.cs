using GoodHamburger.Application.Data;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Infrastructure.Repositories;

public class StaticMenuRepository : IMenuRepository
{
    public MenuItem? FindById(int id) => MenuData.FindById(id);
    public IReadOnlyList<MenuItem> GetAll() => MenuData.Items;
}
