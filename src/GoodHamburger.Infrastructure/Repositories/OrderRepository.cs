using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task<IEnumerable<Order>> GetAllAsync() =>
        await context.Orders.AsNoTracking().ToListAsync();

    public async Task<Order?> GetByIdAsync(Guid id) =>
        await context.Orders.FindAsync(id);

    public async Task<Order> CreateAsync(Order order)
    {
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        context.Orders.Update(order);
        await context.SaveChangesAsync();
        return order;
    }

    public async Task DeleteAsync(Order order)
    {
        context.Orders.Remove(order);
        await context.SaveChangesAsync();
    }
}
