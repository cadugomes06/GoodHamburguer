using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Interfaces;

public interface IMenuRepository
{
    MenuItem? FindById(int id);
    IReadOnlyList<MenuItem> GetAll();
}
