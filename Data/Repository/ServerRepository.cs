using Microsoft.EntityFrameworkCore;
using RestaurantOrders.Models;

namespace RestaurantOrders.Data.Repository;

public class ServerRepository(AppDbContext db) : IServerRepository
{
    public Task<List<Server>> GetAllAsync() =>
        db.Servers.AsNoTracking().OrderBy(s => s.ServerId).ToListAsync();

    public Task<Server?> GetByIdAsync(int id) =>
        db.Servers.AsNoTracking().FirstOrDefaultAsync(s => s.ServerId == id);

    public async Task<Server> CreateAsync(string name)
    {
        var server = new Server { ServerName = name };
        db.Servers.Add(server);
        await db.SaveChangesAsync();
        return server;
    }

    public async Task<Server?> UpdateNameAsync(int id, string name)
    {
        var server = await db.Servers.FirstOrDefaultAsync(s => s.ServerId == id);
        if (server is null) return null;

        server.ServerName = name;
        await db.SaveChangesAsync();
        return server;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var server = await db.Servers.FirstOrDefaultAsync(s => s.ServerId == id);
        if (server is null) return false;

        if (await db.Orders.AnyAsync(o => o.ServerId == id))
            throw new InvalidOperationException("Неможливо видалити офіціанта: у нього є замовлення.");

        db.Servers.Remove(server);
        await db.SaveChangesAsync();
        return true;
    }
}
