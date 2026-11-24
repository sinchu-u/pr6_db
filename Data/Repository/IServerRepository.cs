using RestaurantOrders.Models;

namespace RestaurantOrders.Data.Repository;

public interface IServerRepository
{
    Task<List<Server>> GetAllAsync();
    Task<Server?> GetByIdAsync(int id);
    Task<Server> CreateAsync(string name);
    Task<Server?> UpdateNameAsync(int id, string name);
    Task<bool> DeleteAsync(int id);
}
