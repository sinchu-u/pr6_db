using RestaurantOrders.Models;

namespace RestaurantOrders.Data.Repository;

public interface IMenuRepository
{
    Task<List<MenuPosition>> GetAllAsync();
    Task<MenuPosition?> GetByIdAsync(int id);
    Task<MenuPosition> CreateAsync(string name, double amount, decimal price);
    Task<MenuPosition?> UpdateAsync(int id, string name, double amount, decimal price);
    Task<bool> DeleteAsync(int id);

    Task<List<MenuPosition>> GetMoreExpensiveThanRawAsync(decimal price);
    List<MenuPosition> SearchByNameUnsafe(string name, out string executedSql);
    List<MenuPosition> SearchByNameSafeRaw(string name);
    List<MenuPosition> SearchByNameOrm(string name);
}
