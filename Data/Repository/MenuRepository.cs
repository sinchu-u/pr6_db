using Microsoft.EntityFrameworkCore;
using RestaurantOrders.Models;

namespace RestaurantOrders.Data.Repository;

public class MenuRepository(AppDbContext db) : IMenuRepository
{
    public Task<List<MenuPosition>> GetAllAsync() =>
        db.MenuPositions.AsNoTracking().OrderBy(m => m.PositionId).ToListAsync();

    public Task<MenuPosition?> GetByIdAsync(int id) =>
        db.MenuPositions.AsNoTracking().FirstOrDefaultAsync(m => m.PositionId == id);

    public async Task<MenuPosition> CreateAsync(string name, double amount, decimal price)
    {
        var position = new MenuPosition { PositionName = name, PositionAmount = amount, PositionPrice = price };
        db.MenuPositions.Add(position);
        await db.SaveChangesAsync();
        return position;
    }

    public async Task<MenuPosition?> UpdateAsync(int id, string name, double amount, decimal price)
    {
        var position = await db.MenuPositions.FirstOrDefaultAsync(m => m.PositionId == id);
        if (position is null) return null;

        position.PositionName = name;
        position.PositionAmount = amount;
        position.PositionPrice = price;
        await db.SaveChangesAsync();
        return position;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var position = await db.MenuPositions.FirstOrDefaultAsync(m => m.PositionId == id);
        if (position is null) return false;

        if (await db.OrderItems.AnyAsync(i => i.PositionId == id))
            throw new InvalidOperationException("Неможливо видалити позицію: вона є в замовленнях.");

        db.MenuPositions.Remove(position);
        await db.SaveChangesAsync();
        return true;
    }

    // ------------------------------------------------------------------
    // Сирі SQL-запити
    // ------------------------------------------------------------------

    public async Task<List<MenuPosition>> GetMoreExpensiveThanRawAsync(decimal price) =>
        // {price} в інтерполяції FromSql перетворюється на SQL-параметр, а не підставляється в текст
        await db.MenuPositions
            .FromSql($"SELECT * FROM menu_position WHERE position_price > {price}")
            .AsNoTracking()
            .OrderBy(m => m.PositionPrice)
            .ToListAsync();

    public List<MenuPosition> SearchByNameUnsafe(string name, out string executedSql)
    {
        // !!! НЕБЕЗПЕЧНО: введений користувачем текст стає частиною SQL-коду.
        executedSql = "SELECT * FROM menu_position WHERE position_name = '" + name + "'";

#pragma warning disable EF1002 // попередження аналізатора про ризик SQL-ін'єкції — це навмисна демонстрація
        return db.MenuPositions
            .FromSqlRaw(executedSql)
            .AsNoTracking()
            .ToList();
#pragma warning restore EF1002
    }

    public List<MenuPosition> SearchByNameSafeRaw(string name) =>
        db.MenuPositions
            .FromSql($"SELECT * FROM menu_position WHERE position_name = {name}")
            .AsNoTracking()
            .ToList();

    public List<MenuPosition> SearchByNameOrm(string name) =>
        db.MenuPositions
            .AsNoTracking()
            .Where(m => m.PositionName == name)
            .ToList();
}
