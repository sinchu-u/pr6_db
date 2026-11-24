using Microsoft.EntityFrameworkCore;
using RestaurantOrders.Models;

namespace RestaurantOrders.Data.Repository;

public class OrderRepository(AppDbContext db) : IOrderRepository
{
    private IQueryable<Order> OrdersWithDetails() =>
        db.Orders
            .AsNoTracking()
            .Include(o => o.Server)
            .Include(o => o.Items)
            .ThenInclude(i => i.Position);

    public Task<List<Order>> GetAllAsync() =>
        OrdersWithDetails().OrderBy(o => o.OrderId).ToListAsync();

    public Task<Order?> GetByIdAsync(int id) =>
        OrdersWithDetails().FirstOrDefaultAsync(o => o.OrderId == id);

    public async Task<Order> CreateAsync(int tableNumber, int serverId)
    {
        if (!await db.Servers.AnyAsync(s => s.ServerId == serverId))
            throw new InvalidOperationException($"Офіціанта з ID {serverId} не існує.");

        var order = new Order { TableNumber = tableNumber, ServerId = serverId };
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateAsync(int id, int tableNumber, int serverId)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
        if (order is null) return null;

        if (!await db.Servers.AnyAsync(s => s.ServerId == serverId))
            throw new InvalidOperationException($"Офіціанта з ID {serverId} не існує.");

        order.TableNumber = tableNumber;
        order.ServerId = serverId;
        await db.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderId == id);
        if (order is null) return false;

        // У схемі немає ON DELETE CASCADE, тому спочатку видаляємо позиції замовлення
        db.OrderItems.RemoveRange(order.Items);
        db.Orders.Remove(order);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<OrderItem> AddItemAsync(int orderId, int positionId, int quantity)
    {
        if (!await db.Orders.AnyAsync(o => o.OrderId == orderId))
            throw new InvalidOperationException($"Замовлення з ID {orderId} не існує.");
        if (!await db.MenuPositions.AnyAsync(m => m.PositionId == positionId))
            throw new InvalidOperationException($"Позиції меню з ID {positionId} не існує.");

        // Якщо така страва вже є в замовленні — збільшуємо кількість
        var existing = await db.OrderItems
            .FirstOrDefaultAsync(i => i.OrderId == orderId && i.PositionId == positionId);

        if (existing is not null)
        {
            existing.Quantity += quantity;
            await db.SaveChangesAsync();
            return existing;
        }

        var item = new OrderItem { OrderId = orderId, PositionId = positionId, Quantity = quantity };
        db.OrderItems.Add(item);
        await db.SaveChangesAsync();
        return item;
    }

    public async Task<bool> UpdateItemQuantityAsync(int itemId, int quantity)
    {
        var item = await db.OrderItems.FirstOrDefaultAsync(i => i.ItemId == itemId);
        if (item is null) return false;

        item.Quantity = quantity;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveItemAsync(int itemId)
    {
        var item = await db.OrderItems.FirstOrDefaultAsync(i => i.ItemId == itemId);
        if (item is null) return false;

        db.OrderItems.Remove(item);
        await db.SaveChangesAsync();
        return true;
    }
}
